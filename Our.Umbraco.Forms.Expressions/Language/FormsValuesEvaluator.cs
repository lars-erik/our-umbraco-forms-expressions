using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony;
using Irony.Interpreter;
using Irony.Parsing;
using Our.Umbraco.Forms.Expressions.WebApi;
using Umbraco.Forms.Core;

namespace Our.Umbraco.Forms.Expressions.Language
{
    public class FormsValuesEvaluator
    {
        private readonly Dictionary<string, Guid> mappings;
        private readonly Record record;
        private readonly string program;

        public FormsValuesEvaluator(Record record, Dictionary<string, Guid> mappings, string program)
        {
            this.record = record;
            this.mappings = mappings;
            this.program = program;
        }

        public FormsValuesResult Evaluate()
        {
            var grammar = new FormsValuesExpressionGrammar();
            var lng = new LanguageData(grammar);
            if (lng.Errors.Any())
                return new FormsValuesResult { Errors = String.Join(", ", lng.Errors.Select(e => e.Message)) };

            var parser = new Parser(lng);
            var tree = parser.Parse(program);
            if (tree.ParserMessages.Any(m => m.Level == ErrorLevel.Error))
                return new FormsValuesResult { Errors = String.Join(", ", tree.ParserMessages.Select(m => m.Message)) };

            var runtime = (FormsValuesExpressionRuntime)grammar.CreateRuntime(lng);
            runtime.Mappings = mappings;
            runtime.Record = record;

            var scriptApp = new ScriptApp(runtime);
            try
            {
                var result = scriptApp.Evaluate(program);
                return new FormsValuesResult { Value = (int)result };
            }
            catch (Exception ex)
            {
                return new FormsValuesResult { Errors = ex.Message };
            }
        }

    }
}
