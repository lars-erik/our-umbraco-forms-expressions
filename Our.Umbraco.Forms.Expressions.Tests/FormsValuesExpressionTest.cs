using System;
using System.Collections.Generic;
using System.Linq;
using Irony.Interpreter;
using Irony.Parsing;
using NUnit.Framework;
using Our.Umbraco.Forms.Expressions.Language;
using Umbraco.Forms.Core;

namespace Our.Umbraco.Forms.Expressions.Tests
{
    public class FormsValuesExpressionTest
    {
        private Dictionary<string, Guid> mappings;
        private Record record;

        protected object EvaluateValue(string program)
        {
            var result = EvaluateResult(program);
            return result.Value;
        }

        protected FormsValuesResult EvaluateResult(string program)
        {
            var evaluator = new FormsValuesEvaluator(program);
            var result = evaluator.Evaluate(record, mappings);
            Assert.That(result.Errors, Is.Null, result.Errors);
            return result;
        }

        protected FormsValuesResult EvaluateResultWithError(string program)
        {
            var evaluator = new FormsValuesEvaluator(program);
            var result = evaluator.Evaluate(record, mappings);
            return result;
        }

        [SetUp]
        public void Setup()
        {
            record = new Record();
            mappings = new Dictionary<string, Guid>();
        }

        protected void AddField(string fieldName, params object[] values)
        {
            var fieldId = CreateMapping(fieldName);
            AddField(fieldId, values);
        }

        private Guid CreateMapping(string fieldName)
        {
            var fieldId = Guid.NewGuid();
            mappings.Add(fieldName.ToLower(), fieldId);
            return fieldId;
        }

        private void AddField(Guid fieldId, params object[] values)
        {
            record.RecordFields.Add(fieldId, new RecordField {Values = values.ToList()});
        }

        protected object FieldValue(string fieldName)
        {
            return record.GetRecordField(mappings[fieldName]).Values[0];
        }
    }
}