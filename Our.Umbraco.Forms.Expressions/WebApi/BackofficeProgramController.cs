using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Irony;
using Irony.Interpreter;
using Irony.Parsing;
using Our.Umbraco.Forms.Expressions.Language;
using Umbraco.Forms.Core;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.Forms.Expressions.WebApi
{
    public class BackofficeProgramController : UmbracoAuthorizedApiController
    {
        private Dictionary<string, Guid> mappings;
        private Record record;

        [HttpPost]
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
            var evaluator = new FormsValuesEvaluator(program);
            return evaluator.Evaluate(record, mappings);
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
