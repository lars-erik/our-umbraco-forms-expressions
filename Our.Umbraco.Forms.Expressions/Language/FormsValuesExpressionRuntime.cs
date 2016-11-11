using System;
using System.Collections.Generic;
using Irony.Interpreter;
using Irony.Parsing;
using Umbraco.Forms.Core;

namespace Our.Umbraco.Forms.Expressions.Language
{
    public class FormsValuesExpressionRuntime : LanguageRuntime
    {
        public Dictionary<string, Guid> Mappings;
        public Record Record;

        Dictionary<string, object> variables = new Dictionary<string, object>();

        public FormsValuesExpressionRuntime(LanguageData language) : base(language)
        {
        }

        public override Binding Bind(BindingRequest request)
        {
            return base.Bind(request);
        }

        public override Binding BindSymbolForRead(BindingRequest request)
        {
            var lowerKey = request.Symbol.ToLower();
            if (Mappings.ContainsKey(lowerKey))
            {
                var fieldId = Mappings[lowerKey];
                var field = Record.GetRecordField(fieldId);
                var value = Convert.ToInt32(field.ValuesAsString());
                return new ConstantBinding(value, new BindingTargetInfo(request.Symbol, BindingTargetType.ClrInterop));
            }

            return base.BindSymbolForRead(request);
        }
    }
}
