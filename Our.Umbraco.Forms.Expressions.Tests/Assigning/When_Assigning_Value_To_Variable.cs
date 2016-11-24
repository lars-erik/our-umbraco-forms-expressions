using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Assigning
{
    [TestFixture]
    public class When_Assigning_Value_To_Variable : FormsValuesExpressionTest
    {
        [Test]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(7)]
        public void Value_Goes_Into_Runtime_Values(int value)
        {
            var program = @"x = " + value;

            var result = EvaluateValue(program);

            Assert.That(result, Is.EqualTo(value));
        }
    }
}
