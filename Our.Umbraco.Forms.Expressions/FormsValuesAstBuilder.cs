using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Our.Umbraco.Forms.Expressions
{
    public class FormsValuesAstBuilder : AstBuilder
    {
        public FormsValuesAstBuilder(AstContext context) : base(context)
        {
        }

        protected override Type GetDefaultNodeType(BnfTerm term)
        {
            if (term is FieldTerminal)
                return typeof (IdentifierNode);

            return base.GetDefaultNodeType(term);
        }
    }
}
