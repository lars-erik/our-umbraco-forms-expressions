using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Umbraco.Forms.Core;

namespace Our.Umbraco.Forms.Expressions.Tests
{
    [TestFixture]
    public class When_Prioritizing_Operators : FormsValuesExpressionTest
    {
        [Test]
        public void Multiplication_Goes_Before_Addition()
        {
            var program = "x = [a field] * 2 + 1";

            AddField("a field", 5);

            var result = Evaluate(program);

            Assert.That(result, Is.EqualTo(11));
        }

        [Test]
        public void Parenthesis_Takes_Precedence()
        {
            var program = "x = ([a field] + 1) * 2";

            AddField("a field", 5);

            var result = Evaluate(program);

            Assert.That(result, Is.EqualTo(12));
        }
    }
}
