using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Math
{
    [TestFixture]
    public class When_Prioritizing_Operators : FormsValuesExpressionTest
    {
        [Test]
        public void Multiplication_Goes_Before_Addition()
        {
            var program = "x = [a field] * 2 + 1";

            AddField("a field", 5);

            var result = EvaluateValue(program);

            Assert.That(result, Is.EqualTo(11));
        }

        [Test]
        public void Parenthesis_Takes_Precedence()
        {
            var program = "x = ([a field] + 1) * 2";

            AddField("a field", 5);

            var result = EvaluateValue(program);

            Assert.That(result, Is.EqualTo(12));
        }
    }
}
