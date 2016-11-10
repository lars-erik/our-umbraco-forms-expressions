using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Interpreter;
using Irony.Parsing;

namespace Our.Umbraco.Forms.Expressions
{
    public class FormsValuesExpressionRuntime : LanguageRuntime
    {
        Dictionary<string, object> variables = new Dictionary<string, object>();

        public FormsValuesExpressionRuntime(LanguageData language) : base(language)
        {
        }

        public override Binding Bind(BindingRequest request)
        {
            return base.Bind(request);
        }
    }
}
