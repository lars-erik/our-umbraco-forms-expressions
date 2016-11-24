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
        private readonly string program;
        private FormsValuesExpressionGrammar grammar;
        private Parser parser;
        private LanguageData language;

        public FormsValuesEvaluator(string program)
        {
            this.program = program;

            grammar = new FormsValuesExpressionGrammar();
            language = new LanguageData(grammar);
            parser = new Parser(language);
        }

        public FormsValuesResult Evaluate(Record record, Dictionary<string, Guid> mappings)
        {
            var result = Validate();
            if (result.Errors != null)
                return result;

            var runtime = (FormsValuesExpressionRuntime)grammar.CreateRuntime(language);
            runtime.SetFields = result.SetFields;
            runtime.Mappings = mappings;
            runtime.Record = record;

            var scriptApp = new ScriptApp(runtime);
            object evaluatedValue = null;
            try
            {
                evaluatedValue = scriptApp.Evaluate(program);
            }
            catch (Exception ex)
            {
                result.Errors = "Error in program. " + ex.Message;
            }

            result.Value = evaluatedValue;

            return result;
        }

        public FormsValuesResult Validate()
        {
            if (language.Errors.Any())
                return new FormsValuesResult {Errors = String.Join(", ", language.Errors.Select(e => e.Message))};

            var tree = parser.Parse(program);

            if (tree.ParserMessages.Any(m => m.Level == ErrorLevel.Error))
            { 
                return new FormsValuesResult
                {
                    Errors = String.Join(", ", 
                        tree.ParserMessages.Select(m =>
                            $"{m.Location.Line + 1},{m.Location.Column + 1}: {m.Message}"
                        )
                    )
                };
            }

            return new FormsValuesResult();
        }
    }
}
