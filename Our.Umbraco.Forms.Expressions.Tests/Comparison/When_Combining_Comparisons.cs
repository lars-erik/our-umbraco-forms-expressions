using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Comparison
{
    [TestFixture]
    public class When_Combining_Comparisons : FormsValuesExpressionTest
    {
        [Test]
        public void Using_And()
        {
            var program = "1 equals 1 and \"a\" does not equal \"b\"";
            var result = EvaluateValue(program);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Using_Or()
        {
            var program = "1 equals 2 or 5 equals 5";
            var result = EvaluateValue(program);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Grouped()
        {
            var program = "1 == 1 and (1 == 2 or 2 != 3)";
            var result = EvaluateValue(program);
            Assert.That(result, Is.True);
        }
    }
}
