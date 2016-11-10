using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Interpreter;
using Irony.Parsing;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests
{
    [TestFixture]
    public class When_Assigning_Values_To_Variable
    {
        [Test]
        public void Value_Goes_Into_Runtime_Values()
        {
            const string program = @"
x = 5
y = x * 2
";

            var grammar = new FormsValuesExpressionGrammar();
            var lng = new LanguageData(grammar);
            Assert.That(lng.Errors, Is.Empty, String.Join(", ", lng.Errors.Select(e => e.Message)));

            var parser = new Parser(lng);
            var tree = parser.Parse(program);
            Assert.That(tree.HasErrors(), Is.False, String.Join(", ", tree.ParserMessages.Select(m => m.Message)));

            var runtime = grammar.CreateRuntime(lng);
            var scriptApp = new ScriptApp(runtime);
            var result = scriptApp.Evaluate(program);
            
            Assert.That(result, Is.EqualTo(10));
        }
    }
}
