using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Our.Umbraco.Forms.Expressions
{
    public class FormsValuesExpressionGrammar : InterpretedLanguageGrammar
    {
        public FormsValuesExpressionGrammar() : base(false)
        {
            var program = new NonTerminal("program", typeof(StatementListNode));
            var statement = new NonTerminal("statement");

            var assignment = new NonTerminal("assignment", typeof(AssignmentNode));
            var expression = new NonTerminal("expression");

            var term = new NonTerminal("term");
            var binExpr = new NonTerminal("binExpr", typeof(BinaryOperationNode));

            var binOp = new NonTerminal("binOp", "operator");
            var equals = new NonTerminal("equals", "assignment op");

            var field = new FieldTerminal("field");
            var identifier = new IdentifierTerminal("identifier");
            var number = new NumberLiteral("number");

            StringLiteral fieldQuoted = new StringLiteral("field_quoted");
            fieldQuoted.AddStartEnd("[", "]", StringOptions.NoEscapes);
            fieldQuoted.AddStartEnd("\"", StringOptions.NoEscapes);
            fieldQuoted.SetOutputTerminal(this, field);

            program.Rule = MakePlusRule(program, NewLine, statement);

            statement.Rule = assignment | expression | Empty;

            assignment.Rule = identifier + equals + expression;
            expression.Rule = term | binExpr;

            term.Rule = number | identifier | field; //  
            binExpr.Rule = expression + binOp + expression;

            binOp.Rule = ToTerm("+") | "-" | "*" | "/";
            equals.Rule = ToTerm("=");

            Root = program;

            RegisterOperators(30, "+", "-");
            RegisterOperators(40, "*", "/");

            MarkTransient(statement, expression, term, equals, binOp);

            MarkPunctuation("[", "]");
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

    public class FieldTerminal : Terminal
    {
        public FieldTerminal(string name)
            : base(name)
        {
        }
    }
}
