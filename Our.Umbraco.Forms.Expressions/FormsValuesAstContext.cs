using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Our.Umbraco.Forms.Expressions
{
    public class FormsValuesAstContext : InterpreterAstContext
    {
        public FormsValuesAstContext(LanguageData language, OperatorHandler operatorHandler = null) : base(language, operatorHandler)
        {
        }
    }
}
