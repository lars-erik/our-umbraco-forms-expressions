using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Comparison
{
    [TestFixture]
    public class When_Comparing_Substrings : FormsValuesExpressionTest
    {
        [Test]
        [TestCase("123,234,1234", "123", true)]
        [TestCase("123,234,1234", "345", false)]
        [TestCase("some; csv; record", "csv", true)]
        [TestCase("some; csv; record", "vsc", false)]
        public void Using_Contains(string fieldValue, string criteria, bool expectedResult)
        {
            var program = $"x = contains([field], \"{criteria}\")";
            AddField("field", fieldValue);
            var result = EvaluateResult(program);
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("123", "234", "123", true)]
        [TestCase("123", "234", "234", true)]
        [TestCase("123", "234", "345", false)]
        public void Using_Contains_With_Multivalue_Field(string fieldValueA, string fieldValueB, string criteria, bool expectedResult)
        {
            var program = $"x = contains([field], \"{criteria}\")";
            AddField("field", fieldValueA, fieldValueB);
            var result = EvaluateResult(program);
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }
    }
}
