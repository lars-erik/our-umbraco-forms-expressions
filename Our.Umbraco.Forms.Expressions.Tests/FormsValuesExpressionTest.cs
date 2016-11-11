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

        protected object Evaluate(string program)
        {
            var grammar = new FormsValuesExpressionGrammar();
            var lng = new LanguageData(grammar);
            Assert.That(lng.Errors, Is.Empty, String.Join(", ", lng.Errors.Select(e => e.Message)));

            var parser = new Parser(lng);
            var tree = parser.Parse(program);
            Assert.That(tree.HasErrors(), Is.False, String.Join(", ", tree.ParserMessages.Select(m => m.Message)));

            var runtime = (FormsValuesExpressionRuntime)grammar.CreateRuntime(lng);
            runtime.Mappings = mappings;
            runtime.Record = record;

            var scriptApp = new ScriptApp(runtime);
            var result = scriptApp.Evaluate(program);
            return result;
        }

        [SetUp]
        public void Setup()
        {
            record = new Record();
            mappings = new Dictionary<string, Guid>();
        }

        protected void AddField(string fieldName, int value)
        {
            var fieldId = CreateMapping(fieldName);
            AddField(fieldId, value);
        }

        private Guid CreateMapping(string fieldName)
        {
            var fieldId = Guid.NewGuid();
            mappings.Add(fieldName, fieldId);
            return fieldId;
        }

        private void AddField(Guid fieldId, int value)
        {
            record.RecordFields.Add(fieldId, new RecordField {Values = new List<object> {value}});
        }
    }
}