using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Our.Umbraco.Forms.Expressions.Language
{
    public class FormsValuesAstContext : InterpreterAstContext
    {
        public FormsValuesAstContext(LanguageData language, OperatorHandler operatorHandler = null) : base(language, operatorHandler)
        {
        }
    }
}
