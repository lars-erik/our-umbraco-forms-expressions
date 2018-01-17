using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Program_Flow
{
    [TestFixture]
    public class When_Executing_Conditional_Logic : FormsValuesExpressionTest
    {
        [Test]
        [TestCase(1, true)]
        [TestCase(2, false)]
        public void Only_Executes_When_Expression_Is_True(int value, bool expectedResult)
        {
            const string program = @"
                result = false
                if [x] == 1
                    result = true
                end
                output = result
            ";

            AddField("x", value);

            var result = EvaluateResult(program);

            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public void Executes_Else_Block_When_Expression_Is_False(int value, bool expectedResult)
        {
            const string program = @"
                if [x] == 2
                    result = true
                else
                    result = false
                end
            ";

            AddField("x", value);

            var result = EvaluateResult(program);

            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }
    }
}
