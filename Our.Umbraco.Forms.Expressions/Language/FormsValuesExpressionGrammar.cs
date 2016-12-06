using System;
using System.Threading;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Our.Umbraco.Forms.Expressions.Language
{
    public class FormsValuesExpressionGrammar : InterpretedLanguageGrammar
    {
        #region Non terminals
        private readonly NonTerminal program = new NonTerminal("program", typeof(StatementListNode));
        private readonly NonTerminal statement = new NonTerminal("statement");

        private readonly NonTerminal ifBlock = new NonTerminal("if block", typeof(IfBlockNode));

        private readonly NonTerminal assignment = new NonTerminal("assignment", typeof(AssignmentNode));
        private readonly NonTerminal expression = new NonTerminal("expression");

        private readonly NonTerminal assignable = new NonTerminal("assignable");
        private readonly NonTerminal term = new NonTerminal("term");
        private readonly NonTerminal unExpr = new NonTerminal("unExpr", typeof(UnaryOperationNode));
        private readonly NonTerminal binExpr = new NonTerminal("binExpr", typeof(BinaryOperationNode));
        private readonly NonTerminal groupExpr = new NonTerminal("groupExpr");

        private readonly NonTerminal argList = new NonTerminal("argList", typeof(ExpressionListNode));
        private readonly NonTerminal functionCall = new NonTerminal("functionCall", typeof(FunctionCallNode));
        private readonly NonTerminal functionName = new NonTerminal("functionName", typeof(FunctionNameNode));

        private readonly NonTerminal binOp = new NonTerminal("binOp", "operator");
        private readonly NonTerminal unOp = new NonTerminal("unOp");
        private readonly NonTerminal assign = new NonTerminal("assign", "assignment operator");
        #endregion

        #region Literals
        private readonly FieldTerminal field = new FieldTerminal("field");
        private readonly IdentifierTerminal identifier = new IdentifierTerminal("identifier");
        private readonly NumberLiteral number = new NumberLiteral("number");
        private readonly StringLiteral fieldQuoted = new StringLiteral("field_quoted");
        private readonly StringLiteral stringQuoted = new StringLiteral("string_quoted");
        private readonly ConstantTerminal boolean = new ConstantTerminal("boolean", typeof(LiteralValueNode));
        #endregion

        public FormsValuesExpressionGrammar() : base(false)
        {
            SetupTerminals();
            BuildLanguage();
            SetProgram();
            Decorate();
        }

        private void BuildLanguage()
        {
            program.Rule = MakePlusRule(program, NewLine, statement);

            statement.Rule = assignment | expression | ifBlock | Empty;

            ifBlock.Rule = 
                ToTerm("if") + 
                    expression + NewLine + 
                    program + NewLine + 
                    "end"
                | 
                "if" + 
                    expression + NewLine + 
                    program + NewLine + 
                    "else" + NewLine + 
                    program + NewLine + 
                    "end";

            assignment.Rule = assignable + assign + expression;
            expression.Rule = term | binExpr | unExpr;
            groupExpr.Rule = "(" + expression + ")";

            assignable.Rule = identifier | field | functionCall;
            term.Rule = number | stringQuoted | assignable | groupExpr | boolean; //  
            binExpr.Rule = expression + binOp + expression;
            unExpr.Rule = unOp + term + ReduceHere();

            argList.Rule = MakeStarRule(argList, ToTerm(","), expression);
            functionCall.Rule = functionName + "(" + argList + ")";
            functionName.Rule = ToTerm("power") | "round" | 
                                "ifblank";

            unOp.Rule = ToTerm("+") | "-";
            binOp.Rule = 
                ToTerm("+") | "-" | "*" | "/" | 
                "==" | "equals" |
                "!=" | "does not equal" |
                "and" | "or";
                
            assign.Rule = ToTerm("=");
            
        }

        private void Decorate()
        {
            RegisterOperators(15, "and", "or");
            RegisterOperators(20, "equals", "==", "does not equal", "!=");
            RegisterOperators(30, "+", "-");
            RegisterOperators(40, "*", "/");

            MarkTransient(statement, expression, groupExpr, term, assign, unOp, binOp, assignable);

            MarkPunctuation("(", ")", "[", "]");
            RegisterBracePair("(", ")");
            RegisterBracePair("[", "]");

            AddToNoReportGroup(NewLine);
            AddTermsReportGroup("assignment", "=");
            AddTermsReportGroup("equality operator", "equals", "==", "does not equal", "!=");
        }

        private void SetProgram()
        {
            Root = program;
        }

        private void SetupTerminals()
        {
            number.DefaultFloatType = TypeCode.Double;
            // TODO: Figure out how to use current culture, see "When_Comparing_Numbers"
            number.DecimalSeparator = '.'; // Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

            fieldQuoted.AddStartEnd("[", "]", StringOptions.NoEscapes);
            fieldQuoted.SetOutputTerminal(this, field);

            stringQuoted.AddStartEnd("\"", StringOptions.NoEscapes);

            boolean.Add("true", true);
            boolean.Add("false", false);
        }

        public override LanguageRuntime CreateRuntime(LanguageData language)
        {
            return new FormsValuesExpressionRuntime(language);
        }

        public override void BuildAst(LanguageData language, ParseTree parseTree)
        {
            var opHandler = new FormsValuesOperatorHandler(language.Grammar.CaseSensitive);
            Util.Check(!parseTree.HasErrors(), "ParseTree has errors, cannot build AST.");
            var astContext = new FormsValuesAstContext(language, opHandler);
            var astBuilder = new FormsValuesAstBuilder(astContext);
            astBuilder.BuildAst(parseTree);
        }

    }

    public class IfBlockNode : AstNode
    {
        public AstNode Test;
        public AstNode IfTrue;
        public AstNode IfFalse;

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            ParseTreeNodeList mappedChildNodes = treeNode.GetMappedChildNodes();
            this.Test = this.AddChild("Test", mappedChildNodes[1]);
            this.IfTrue = this.AddChild("IfTrue", mappedChildNodes[2]);
            if (mappedChildNodes.Count <= 3)
                return;
            this.IfFalse = this.AddChild("IfFalse", mappedChildNodes[4]);
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = (AstNode)this;
            object obj1 = (object)null;
            object obj2 = this.Test.Evaluate(thread);
            if (thread.Runtime.IsTrue(obj2))
            {
                if (this.IfTrue != null)
                    obj1 = this.IfTrue.Evaluate(thread);
            }
            else if (this.IfFalse != null)
                obj1 = this.IfFalse.Evaluate(thread);
            thread.CurrentNode = this.Parent;
            return obj1;
        }

        public override void SetIsTail()
        {
            base.SetIsTail();
            if (this.IfTrue != null)
                this.IfTrue.SetIsTail();
            if (this.IfFalse == null)
                return;
            this.IfFalse.SetIsTail();
        }
    }
}
