using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests
{
    [TestFixture]
    public class When_Calculating_Decimals : FormsValuesExpressionTest
    {
        [Test]
        public void Then_Result_Has_Decimals()
        {
            var program = "[input] / 2.0";
            AddField("input", 1);
            var result = EvaluateValue(program);
            Assert.That(result, Is.EqualTo(0.5m));
        }
    }
}
