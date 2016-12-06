using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Encoding
{
    [TestFixture]
    public class When_Reading_Fields_With_Special_Characters : FormsValuesExpressionTest
    {
        [Test]
        public void From_Norwegian_Alphabet()
        {
            const string program = "x = [æ]";
            AddField("æ", 5);
            var result = EvaluateValue(program);
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void With_Question_Mark()
        {
            const string program = "x = [æ oh?]";
            AddField("Æ oh?", 5);
            var result = EvaluateValue(program);
            Assert.That(result, Is.EqualTo(5));
        }
    }
}
