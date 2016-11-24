using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Comparison
{
    [TestFixture]
    public class When_Comparing_Strings : ComparisonTest
    {
        [Test]
        [TestCase("abc", "==", "abc", true)]
        [TestCase("abc", "equals", "abc", true)]
        [TestCase("x", "==", "y", false)]
        [TestCase("x", "equals", "y", false)]
        public void Using_Equals(string fieldValue, string op, string literal, bool expected)
        {
            var program = $"[field] {op} \"{fieldValue}\"";
            Compare(program, literal, expected);
        }

        [Test]
        [TestCase("abc", "!=", "abc", false)]
        [TestCase("abc", "does not equal", "abc", false)]
        [TestCase("x", "!=", "y", true)]
        [TestCase("x", "does not equal", "y", true)]
        public void Using_Not_Equals(string fieldValue, string op, string literal, bool expected)
        {
            var program = $"[field] {op} \"{fieldValue}\"";
            Compare(program, literal, expected);
        }
    }
}
