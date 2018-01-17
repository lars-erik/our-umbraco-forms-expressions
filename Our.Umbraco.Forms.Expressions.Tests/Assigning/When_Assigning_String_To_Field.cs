using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Assigning
{
    [TestFixture]
    public class When_Assigning_String_To_Field : FormsValuesExpressionTest
    {
        [Test]
        [TestCase("Hello")]
        [TestCase("World")]
        [TestCase("Hello World!")]
        public void Then_Value_Goes_Into_Field(string expectedValue)
        {
            var program = $"x = \"{expectedValue}\"\n[field]=x";
            AddField("field", "");
            var result = EvaluateResult(program);
            Assert.That(FieldValue("field"), Is.EqualTo(expectedValue));
        }
    }
}
