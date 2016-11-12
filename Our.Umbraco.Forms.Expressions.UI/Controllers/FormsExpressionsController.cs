using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Irony;
using Irony.Interpreter;
using Irony.Parsing;
using Our.Umbraco.Forms.Expressions.Language;
using Umbraco.Forms.Core;

namespace Our.Umbraco.Forms.Expressions.UI.Controllers
{
    public class FormsExpressionsController : ApiController
    {
        private Dictionary<string, Guid> mappings;
        private Record record;

        [HttpPost]
        [Route("api/formsexpressions/run")]
        public FormsValuesResult Run([FromBody]ProgramRequest programRequest)
        {
            record = new Record();
            mappings = new Dictionary<string, Guid>();

            foreach (var value in programRequest.Values)
                AddField(value.Key, value.Value);

            return Evaluate(programRequest.Program);
        }

        protected FormsValuesResult Evaluate(string program)
        {
            var evaluator = new FormsValuesEvaluator(record, mappings, program);
            return evaluator.Evaluate();
        }

        protected void AddField(string fieldName, int value)
        {
            var fieldId = CreateMapping(fieldName);
            AddField(fieldId, value);
        }

        private Guid CreateMapping(string fieldName)
        {
            var fieldId = Guid.NewGuid();
            mappings.Add(fieldName, fieldId);
            return fieldId;
        }

        private void AddField(Guid fieldId, int value)
        {
            record.RecordFields.Add(fieldId, new RecordField { Values = new List<object> { value } });
        }
    }

    public class ProgramRequest
    {
        public string Program { get; set; }
        public Dictionary<string, int> Values { get; set; }
    }
}