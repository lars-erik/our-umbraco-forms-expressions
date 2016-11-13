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

            EvaluateValue(program);

            Assert.That(FieldValue("field a"), Is.EqualTo(6));
        }

        [Test]
        public void Set_Fields_Are_Reported()
        {
            var program = @"
                x = 5
                [field a] = x + 1    
                [field b] = power([field a], 2)    
            ";

            AddField("field a", 0);
            AddField("field b", 0);

            var result = EvaluateResult(program);

            Assert.That(result.SetFields,
                Has.Exactly(1).With.Property("Key").EqualTo("field a").And.Property("Value").EqualTo(6) &
                Has.Exactly(1).With.Property("Key").EqualTo("field b").And.Property("Value").EqualTo(36)
                );
        }
    }
}
