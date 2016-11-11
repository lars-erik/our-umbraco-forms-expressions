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
        public Result Run([FromBody]ProgramRequest programRequest)
        {
            record = new Record();
            mappings = new Dictionary<string, Guid>();

            foreach (var value in programRequest.Values)
                AddField(value.Key, value.Value);

            return Evaluate(programRequest.Program);
        }

        protected Result Evaluate(string program)
        {
            var grammar = new FormsValuesExpressionGrammar();
            var lng = new LanguageData(grammar);
            if (lng.Errors.Any())
                return new Result { Errors = String.Join(", ", lng.Errors.Select(e => e.Message)) };

            var parser = new Parser(lng);
            var tree = parser.Parse(program);
            if (tree.ParserMessages.Any(m => m.Level == ErrorLevel.Error))
                return new Result { Errors = String.Join(", ", tree.ParserMessages.Select(m => m.Message)) };

            var runtime = (FormsValuesExpressionRuntime)grammar.CreateRuntime(lng);
            runtime.Mappings = mappings;
            runtime.Record = record;

            var scriptApp = new ScriptApp(runtime);
            try
            {
                var result = scriptApp.Evaluate(program);
                return new Result { Value = (int)result };
            }
            catch (Exception ex)
            {
                return new Result { Errors = ex.Message };
            }
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

    public class Result
    {
        public int Value { get; set; }
        public string Errors { get; set; }
    }

    public class ProgramRequest
    {
        public string Program { get; set; }
        public Dictionary<string, int> Values { get; set; }
    }
}