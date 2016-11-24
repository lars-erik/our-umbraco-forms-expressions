using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Program_Flow
{
    [TestFixture]
    public class When_Evaluating_Multiple_Lines : FormsValuesExpressionTest
    {
        [Test]
        public void Last_Line_Is_The_Result()
        {
            var program = @"
                x = [hvor mange vil du ha]
                y = [second field] * 2
                z = (x + y) / 2
            ";

            AddField("hvor mange vil du ha", 2);
            AddField("second field", 3);

            var result = EvaluateValue(program);

            Assert.That(result, Is.EqualTo(4));
        }
    }
}
