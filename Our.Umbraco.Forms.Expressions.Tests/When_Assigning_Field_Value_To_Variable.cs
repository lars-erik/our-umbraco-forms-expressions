using System;
using System.Collections.Generic;
using NUnit.Framework;
using Umbraco.Forms.Core;

namespace Our.Umbraco.Forms.Expressions.Tests
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

            var result = Evaluate(program);

            Assert.That(result, Is.EqualTo(value));
        }
    }
}
