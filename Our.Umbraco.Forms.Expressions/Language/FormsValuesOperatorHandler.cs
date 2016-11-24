using System.Linq.Expressions;
using System.Reflection;
using Irony.Interpreter.Ast;

namespace Our.Umbraco.Forms.Expressions.Language
{
    public class FormsValuesOperatorHandler : OperatorHandler
    {
        public FormsValuesOperatorHandler(bool caseSensitive) : base(caseSensitive)
        {
            OperatorInfoDictionary dict = (OperatorInfoDictionary)
                typeof (OperatorHandler).GetField("_registeredOperators",
                    BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);

            dict.Add("equals", ExpressionType.Equal, 20);
            dict.Add("does not equal", ExpressionType.NotEqual, 20);
        }
    }
}