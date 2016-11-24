using Irony.Ast;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Our.Umbraco.Forms.Expressions.Language
{
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