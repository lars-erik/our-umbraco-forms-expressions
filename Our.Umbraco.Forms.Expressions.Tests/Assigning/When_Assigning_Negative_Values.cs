﻿using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Assigning
{
    [TestFixture]
    public class When_Assigning_Negative_Values : FormsValuesExpressionTest
    {
        [Test]
        public void Result_Is_Negative()
        {
            var program = "x = -1 * 2";
            var result = EvaluateValue(program);
            Assert.That(result, Is.EqualTo(-2));
        }
    }
}
