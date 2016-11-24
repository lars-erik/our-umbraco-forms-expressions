using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Comparison
{
    public class ComparisonTest : FormsValuesExpressionTest
    {
        protected void Compare(string program, object literal, bool expected)
        {
            AddField("field", literal);
            var result = EvaluateResult(program);
            Assert.That(result.Value, Is.EqualTo(expected));
        }
    }
}