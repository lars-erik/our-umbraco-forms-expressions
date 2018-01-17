using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Assigning
{
    [TestFixture]
    public class When_Assigning_String_To_Variable : FormsValuesExpressionTest
    {
        [Test]
        [TestCase("Hello")]
        [TestCase("World")]
        [TestCase("Hello World!")]
        public void Value_Goes_Into_Runtime_Values(string expectedValue)
        {
            var program = $"x = \"{expectedValue}\"";
            var value = EvaluateValue(program);
            Assert.That(value, Is.EqualTo(expectedValue));
        }
    }
}
