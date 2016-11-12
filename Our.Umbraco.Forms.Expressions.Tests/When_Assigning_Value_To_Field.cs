using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Our.Umbraco.Forms.Expressions.Tests
{
    [TestFixture]
    public class When_Assigning_Value_To_Field : FormsValuesExpressionTest
    {
        [Test]
        public void Record_Is_Assigned_The_Value()
        {
            var program = @"
                x = 5
                [field a] = x + 1    
            ";

            AddField("field a", 0);

            Evaluate(program);

            Assert.That(FieldValue("field a"), Is.EqualTo(6));
        }
    }
}
