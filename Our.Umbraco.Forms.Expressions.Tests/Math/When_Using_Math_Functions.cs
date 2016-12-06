using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Math
{
    [TestFixture]
    public class When_Using_Math_Functions : FormsValuesExpressionTest
    {
        [Test]
        public void Then_Calculates_Power()
        {
            var program = "x = power(2, 10)";
            var result = EvaluateValue(program);
            Assert.That(result, Is.EqualTo(1024));
        }

        [Test]
        public void Then_Rounds_Numbers()
        {
            var program = "x = round(5.123, 2)";
            var result = EvaluateValue(program);
            Assert.That(result, Is.EqualTo(5.12));
        }

        [Test]
        public void Then_Ceils_Numbers()
        {
            var program = "x = ceiling(5.123)";
            var result = EvaluateValue(program);
            Assert.That(result, Is.EqualTo(6));
        }
    }
}
