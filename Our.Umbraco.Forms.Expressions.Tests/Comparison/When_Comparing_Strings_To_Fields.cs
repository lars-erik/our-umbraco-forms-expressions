using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Comparison
{
    [TestFixture]
    public class When_Comparing_Strings_To_Fields : FormsValuesExpressionTest
    {
        [Test]
        public void Then_Result_Is_A_Boolean()
        {
            var program = "[field] == \"abc\"";
            AddField("field", "abc");
            var result = EvaluateResult(program);
            Assert.That(result.Value, Is.True);
        }

        [Test]
        public void Using_Equals_Keyword_Then_Result_Is_A_Boolean()
        {
            var program = "[field] equals \"abc\"";
            AddField("field", "abc");
            var result = EvaluateResult(program);
            Assert.That(result.Value, Is.True);
        }
    }
}
