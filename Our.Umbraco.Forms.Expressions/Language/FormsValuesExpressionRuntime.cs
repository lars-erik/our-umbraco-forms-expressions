﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Umbraco.Forms.Core;

namespace Our.Umbraco.Forms.Expressions.Language
{
    public class FormsValuesExpressionRuntime : LanguageRuntime
    {
        Dictionary<string, string> functionMappings = new Dictionary<string, string>
        {
            { "power", "Pow" },
            { "round", "Round" },
            { "isblank", "IsBlank" }
        }; 

        public Dictionary<string, Guid> Mappings;
        public Record Record;

        Dictionary<string, object> variables = new Dictionary<string, object>();

        public FormsValuesExpressionRuntime(LanguageData language) : base(language)
        {
        }

        public Dictionary<string, object> SetFields { get; set; }

        public override void Init()
        {
            base.Init();
            BuiltIns.ImportStaticMembers(typeof(Math));
            BuiltIns.Add("ifblank", new ClrMethodBindingTargetInfo(GetType(), "IfBlank", this));
        }

        public override void InitBinaryOperatorImplementationsForMatchedTypes()
        {
            base.InitBinaryOperatorImplementationsForMatchedTypes();

            AddBinary(ExpressionType.Equal, typeof (string), (x, y) => (string)x == (string)y);
            AddBinary(ExpressionType.NotEqual, typeof (string), (x, y) => (string)x != (string)y);
        }

        public override Binding Bind(BindingRequest request)
        {
            var sym = request.Symbol.ToLower();
            if (functionMappings.ContainsKey(sym))
                request.Symbol = functionMappings[sym];

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
                    var value = TryConvertToNumber(field);

                    return new ConstantBinding(value, new BindingTargetInfo(request.Symbol, BindingTargetType.ClrInterop));
                }
                catch
                {
                    throw new Exception("Could not bind field name " + lowerKey + " to a floating point value.");
                }
            }

            return base.BindSymbolForRead(request);
        }

        private static object TryConvertToNumber(RecordField field)
        {
            var value = field.Values[0];
            var stringValue = value as string;

            if (!String.IsNullOrWhiteSpace(stringValue))
            {
                double asDouble;
                //int asInt;

                //if (int.TryParse(stringValue, out asInt))
                //    value = asInt;
                //else 
                if (double.TryParse(stringValue, out asDouble))
                    value = asDouble;
            }
            return value;
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

                if (SetFields != null)
                    SetFields.Add(lowerKey, value);
            }
            catch
            {
                throw new Exception("Could not bind field name " + lowerKey + ".");
            }
        }

        private object IfBlank(object value, object alternativeValue)
        {
            if (value == null || "".Equals(value))
            {
                return alternativeValue;
            }
            return value;
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
