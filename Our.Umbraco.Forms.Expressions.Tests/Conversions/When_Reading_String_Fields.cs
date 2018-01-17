using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Conversions
{
    [TestFixture]
    public class When_Reading_String_Fields : FormsValuesExpressionTest
    {
        [Test]
        public void Attempts_Convert_To_Double()
        {
            var program = "x = [x] / 4";
            AddField("x", "2");
            var value = EvaluateValue(program);
            Assert.That(value, Is.EqualTo(.5));
        }

        [Test]
        public void Keeps_String_If_NaN()
        {
            const string expected = "Hello world!";
            var program = "x = [x]";
            AddField("x", expected);
            var value = EvaluateValue(program);
            Assert.That(value, Is.EqualTo(expected));
        }
    }
}
