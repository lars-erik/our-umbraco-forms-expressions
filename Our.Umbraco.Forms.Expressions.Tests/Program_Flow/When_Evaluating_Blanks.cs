using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Program_Flow
{
    [TestFixture]
    public class When_Evaluating_Blanks : FormsValuesExpressionTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Assigns_Alternative_Value_For_Blanks(object value)
        {
            AddField("x", value);
            const string program = "x = ifblank([x], 5)";
            const int expected = 5;
            var result = EvaluateValue(program);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Assigns_Value_For_Non_Blanks()
        {
            const string program = "x = ifblank([x], 5)";
            const int expected = 10;
            AddField("x", expected);
            var result = EvaluateValue(program);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
