using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Conversions
{
    [TestFixture]
    public class When_Mixing_Types : FormsValuesExpressionTest
    {
        [Test]
        public void Then_Explains_Problem_And_Suggests_Using_IsBlank()
        {
            var program = "x = [x] + [y]";
            AddField("x", 2);
            AddField("y", "");
            var result = EvaluateResultWithError(program);
            Assert.That(
                result.Errors, 
                Is.EqualTo("Error in program. Are you mixing text and numbers? If a field is blank you should use the IsBlank function to make it a number.")
            );
        }
    }
}
