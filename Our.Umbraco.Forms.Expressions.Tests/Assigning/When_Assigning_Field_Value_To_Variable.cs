using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Assigning
{
    [TestFixture]
    public class When_Assigning_Field_Value_To_Variable : FormsValuesExpressionTest
    {
        [Test]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(7)]
        public void Then_Value_Is_Field_Value(int value)
        {
            var program = @"x = [field name]";

            AddField("field name", value);

            var result = EvaluateValue(program);

            Assert.That(result, Is.EqualTo(value));
        }
    }
}
