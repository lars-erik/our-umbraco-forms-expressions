using System;
using System.Collections.Generic;
using System.Linq;
using Irony.Interpreter;
using Irony.Parsing;
using NUnit.Framework;
using Umbraco.Forms.Core;

namespace Our.Umbraco.Forms.Expressions.Tests
{
    public class FormsValuesExpressionTest
    {
        protected static object Evaluate(string program, Dictionary<string, Guid> mappings = null, Record record = null)
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
    }
}