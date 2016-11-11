using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests
{
    [TestFixture]
    public class When_Evaluating_Multiple_Lines : FormsValuesExpressionTest
    {
        [Test]
        public void Last_Line_Is_The_Result()
        {
            var program = @"
                x = [first field]
                y = [second field] * 2
                z = (x + y) / 2
            ";

            AddField("first field", 2);
            AddField("second field", 3);

            var result = Evaluate(program);

            Assert.That(result, Is.EqualTo(4));
        }
    }
}
