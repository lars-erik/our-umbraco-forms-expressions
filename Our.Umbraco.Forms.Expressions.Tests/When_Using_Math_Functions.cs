using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests
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
    }
}
