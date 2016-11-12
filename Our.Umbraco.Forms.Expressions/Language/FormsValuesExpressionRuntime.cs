﻿using System;
using System.Collections.Generic;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
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
                try
                {
                    var fieldId = Mappings[lowerKey];
                    var field = Record.GetRecordField(fieldId);
                    var value = Convert.ToDouble(field.ValuesAsString());
                    return new ConstantBinding(value, new BindingTargetInfo(request.Symbol, BindingTargetType.ClrInterop));
                }
                catch
                {
                    throw new Exception("Could not bind field name " + lowerKey + " to a floating point value.");
                }
            }

            return base.BindSymbolForRead(request);
        }

        public override Binding BindSymbolForWrite(BindingRequest request)
        {
            var lowerKey = request.Symbol.ToLower();
            if (Mappings.ContainsKey(lowerKey))
            {
                return new FieldBinding(request.Symbol, BindingTargetType.ClrInterop, SetField);
            }

            return base.BindSymbolForWrite(request);
        }

        private void SetField(ScriptThread thread, object value)
        {
            var lowerKey = thread.CurrentNode.AsString.ToLower();
            try
            {
                var fieldId = Mappings[lowerKey];
                var field = Record.GetRecordField(fieldId);
                field.Values = new List<object> { value };
            }
            catch
            {
                throw new Exception("Could not bind field name " + lowerKey + " to a floating point value.");
            }
        }
    }

    public class FieldBinding : Binding
    {
        public FieldBinding(string symbol, BindingTargetType targetType, ValueSetterMethod setter) : base(symbol, targetType)
        {
            this.SetValueRef = setter;
        }
    }
}
