using System;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Our.Umbraco.Forms.Expressions.Language
{
    public class FormsValuesExpressionGrammar : InterpretedLanguageGrammar
    {
        public FormsValuesExpressionGrammar() : base(false)
        {
            var program = new NonTerminal("program", typeof(StatementListNode));
            var statement = new NonTerminal("statement");

            var assignment = new NonTerminal("assignment", typeof(AssignmentNode));
            var expression = new NonTerminal("expression");

            var assignable = new NonTerminal("assignable");
            var term = new NonTerminal("term");
            var unExpr = new NonTerminal("unExpr", typeof(UnaryOperationNode));
            var binExpr = new NonTerminal("binExpr", typeof(BinaryOperationNode));
            var groupExpr = new NonTerminal("groupExpr");

            var argList = new NonTerminal("argList", typeof(ExpressionListNode));
            var functionCall = new NonTerminal("functionCall", typeof(FunctionCallNode));
            var functionName = new NonTerminal("functionName", typeof(FunctionNameNode));

            var binOp = new NonTerminal("binOp", "operator");
            var unOp = new NonTerminal("unOp");
            var equals = new NonTerminal("equals", "assignment op");

            var field = new FieldTerminal("field");
            var identifier = new IdentifierTerminal("identifier");
            var number = new NumberLiteral("number");
            number.DefaultFloatType = TypeCode.Double;
            number.DecimalSeparator = '.';

            StringLiteral fieldQuoted = new StringLiteral("field_quoted");
            fieldQuoted.AddStartEnd("[", "]", StringOptions.NoEscapes);
            fieldQuoted.AddStartEnd("\"", StringOptions.NoEscapes);
            fieldQuoted.SetOutputTerminal(this, field);

            program.Rule = MakePlusRule(program, NewLine, statement);

            statement.Rule = assignment | expression | Empty;

            assignment.Rule = assignable + equals + expression;
            expression.Rule = term | binExpr | unExpr;
            groupExpr.Rule = "(" + expression + ")";

            assignable.Rule = identifier | field | functionCall;
            term.Rule = number | assignable | groupExpr; //  
            binExpr.Rule = expression + binOp + expression;
            unExpr.Rule = unOp + term + ReduceHere();

            argList.Rule = MakeStarRule(argList, ToTerm(","), expression);
            functionCall.Rule = functionName + "(" + argList + ")";
            functionName.Rule = ToTerm("power") | "round";

            unOp.Rule = ToTerm("+") | "-";
            binOp.Rule = ToTerm("+") | "-" | "*" | "/";
            equals.Rule = ToTerm("=");

            Root = program;

            RegisterOperators(30, "+", "-");
            RegisterOperators(40, "*", "/");

            MarkTransient(statement, expression, groupExpr, term, equals, unOp, binOp, assignable);

            MarkPunctuation("(", ")", "[", "]");
            RegisterBracePair("(", ")");
            RegisterBracePair("[", "]");

            AddToNoReportGroup(NewLine);
            AddTermsReportGroup("assignment op", "=");

        }

        public override LanguageRuntime CreateRuntime(LanguageData language)
        {
            return new FormsValuesExpressionRuntime(language);
        }

        public override void BuildAst(LanguageData language, ParseTree parseTree)
        {
            var opHandler = new OperatorHandler(language.Grammar.CaseSensitive);
            Util.Check(!parseTree.HasErrors(), "ParseTree has errors, cannot build AST.");
            var astContext = new FormsValuesAstContext(language, opHandler);
            var astBuilder = new FormsValuesAstBuilder(astContext);
            astBuilder.BuildAst(parseTree);
        }
    }

    public class FunctionNameNode : IdentifierNode
    {
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            this.Term = treeNode.Term;
            Span = treeNode.Span;
            ErrorAnchor = this.Location;
            treeNode.AstNode = this;
            AsString = (Term == null ? this.GetType().Name : Term.Name);

            Symbol = treeNode.ChildNodes[0].Token.ValueString;
            AsString = Symbol;
        }
    }

}
