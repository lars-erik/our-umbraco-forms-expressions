using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Our.Umbraco.Forms.Expressions.Language;
using Umbraco.Core.Logging;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;

namespace Our.Umbraco.Forms.Expressions.Workflows
{
    public class UmbracoFormsExpressionWorkflowType : WorkflowType
    {
        [Setting("Program", alias = "program", description = "Program to execute for the set value", view = "~/App_Plugins/UmbracoFormsExpressions/settings/program.html")]
        public string Program { get; set; }

        public UmbracoFormsExpressionWorkflowType()
        {
            Id = new Guid("1C07752F-34F4-4E92-A711-BF920E6524E3");
            Name = "UFX Calculation";
            Description = "Calculate a value for a hidden field based on user input";
            Icon = "icon-brackets";
        }

        public override WorkflowExecutionStatus Execute(Record record, RecordEventArgs e)
        {
            var mappings = record.RecordFields.ToDictionary(f => f.Value.Field.Caption.ToLower(), f => f.Key);
            var evaluator = new FormsValuesEvaluator(Program);
            var result = evaluator.Evaluate(record, mappings);
            if (result.Errors != null)
            {
                LogHelper.Error<UmbracoFormsExpressionWorkflowType>(String.Join(", ", result.Errors), new Exception("Failed to execute program")); 
                return WorkflowExecutionStatus.Failed;
            }

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
