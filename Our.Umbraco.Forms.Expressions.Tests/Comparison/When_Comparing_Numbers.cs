using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Our.Umbraco.Forms.Expressions.Tests.Comparison
{
    [TestFixture]
    public class When_Comparing_Numbers : ComparisonTest
    {
        [OneTimeSetUp]
        public static void SetupCulture()
        {
            // TODO: Figure out why , messes up when decimal separator
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [Test]
        [TestCase(1, "==", 1, true)]
        [TestCase(123.1, "==", 123.1, true)]
        [TestCase(1.5, "equals", 1.5, true)]
        [TestCase(123.1, "==", 123.2, false)]
        [TestCase(.5, "equals", .25, false)]
        public void Using_Equals(double fieldValue, string op, double literal, bool expected)
        {
            var program = $"[field] {op} {fieldValue}";
            Compare(program, literal, expected);
        }

        [Test]
        [TestCase(123.1, "!=", 123.1, false)]
        [TestCase(.5, "does not equal", .5, false)]
        [TestCase(123.1, "!=", 123.2, true)]
        [TestCase(.5, "does not equal", .25, true)]
        public void Using_Not_Equals(double fieldValue, string op, double literal, bool expected)
        {
            var program = $"[field] {op} {fieldValue}";
            Compare(program, literal, expected);
        }
    }
}
