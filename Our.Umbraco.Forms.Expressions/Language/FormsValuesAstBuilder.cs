using System;
using Irony.Ast;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Our.Umbraco.Forms.Expressions.Language
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
