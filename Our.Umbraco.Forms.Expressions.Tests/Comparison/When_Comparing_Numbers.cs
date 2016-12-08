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
            Compare(fieldValue, op, literal, expected);
        }

        [Test]
        [TestCase(123.1, "!=", 123.1, false)]
        [TestCase(.5, "does not equal", .5, false)]
        [TestCase(123.1, "!=", 123.2, true)]
        [TestCase(.5, "does not equal", .25, true)]
        public void Using_Not_Equals(double fieldValue, string op, double literal, bool expected)
        {
            Compare(fieldValue, op, literal, expected);
        }

        [Test]
        [TestCase(123.1, "<", 123.3, true)]
        [TestCase(.5, "is less than", .6, true)]
        [TestCase(123.1, "<", 123.1, false)]
        [TestCase(.5, "is less than", .5, false)]
        [TestCase(.6, "is less than", .5, false)]
        public void Using_Less_Than(double fieldValue, string op, double literal, bool expected)
        {
            Compare(fieldValue, op, literal, expected);
        }

        [Test]
        [TestCase(123.1, ">", 123.2, false)]
        [TestCase(.5, "is greater than", .6, false)]
        [TestCase(123.1, ">", 123.1, false)]
        [TestCase(.5, "is greater than", .5, false)]
        [TestCase(.6, ">", .5, true)]
        [TestCase(.6, "is greater than", .5, true)]
        public void Using_Greater_Than(double fieldValue, string op, double literal, bool expected)
        {
            Compare(fieldValue, op, literal, expected);
        }

        [Test]
        [TestCase(123.1, "<=", 123.2, true)]
        [TestCase(.5, "is less than or equal to", .6, true)]
        [TestCase(123.1, "<=", 123.1, true)]
        [TestCase(.5, "is less than or equal to", .5, true)]
        [TestCase(.6, "is less than or equal to", .5, false)]
        public void Using_Less_Than_Or_Equal(double fieldValue, string op, double literal, bool expected)
        {
            Compare(fieldValue, op, literal, expected);
        }

        [Test]
        [TestCase(123.1, ">=", 123.2, false)]
        [TestCase(.5, "is greater than or equal to", .6, false)]
        [TestCase(123.1, ">=", 123.1, true)]
        [TestCase(.5, "is greater than or equal to", .5, true)]
        [TestCase(.6, ">=", .5, true)]
        [TestCase(.6, "is greater than or equal to", .5, true)]
        public void Using_Greater_Than_Or_Equal(double fieldValue, string op, double literal, bool expected)
        {
            Compare(fieldValue, op, literal, expected);
        }

        private void Compare(double fieldValue, string op, double literal, bool expected)
        {
            var program = $"[field] {op} {literal}";
            Compare(program, fieldValue, expected);
        }
    }
}
