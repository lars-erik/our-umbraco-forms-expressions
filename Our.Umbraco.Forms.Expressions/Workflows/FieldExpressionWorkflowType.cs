using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Our.Umbraco.Forms.Expressions.Language;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;

namespace Our.Umbraco.Forms.Expressions.Workflows
{
    public class FieldExpressionWorkflowType : WorkflowType
    {
        [Setting("Program", alias = "program", description = "Program to execute for the set value", view = "~/App_Plugins/UmbracoFormsExpressions/settings/program.html")]
        public string Program { get; set; }

        [Setting("Field to set", alias = "fieldId", description = "Field to set to result of program", view = "~/App_Plugins/UmbracoFormsExpressions/settings/field.html")]
        public string FieldId { get; set; }

        public FieldExpressionWorkflowType()
        {
            Id = new Guid("1C07752F-34F4-4E92-A711-BF920E6524E3");
            Name = "Field Expression";
            Description = "Calculate a value for a hidden field based on user input";
        }

        public override WorkflowExecutionStatus Execute(Record record, RecordEventArgs e)
        {
            var mappings = record.RecordFields.ToDictionary(f => f.Value.Field.Caption.ToLower(), f => f.Key);
            var evaluator = new FormsValuesEvaluator(Program);
            var result = evaluator.Evaluate(record, mappings);
            if (result.Errors != null)
                return WorkflowExecutionStatus.Failed;

            record.GetRecordField(new Guid(FieldId)).Values = new List<object> {result.Value};

            return WorkflowExecutionStatus.Completed;
        }

        public override List<Exception> ValidateSettings()
        {
            var evaluator = new FormsValuesEvaluator(Program);
            var result = evaluator.Validate();
            if (result.Errors != null)
                return new List<Exception> { new Exception(result.Errors) };
            return new List<Exception>();
        }
    }
}
