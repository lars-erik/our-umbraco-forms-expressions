using System.Collections.Generic;

namespace Our.Umbraco.Forms.Expressions.Language
{
    public class FormsValuesResult
    {
        public object Value { get; set; }
        public string Errors { get; set; }
        public Dictionary<string, object> SetFields { get; private set; }

        public FormsValuesResult()
        {
            SetFields = new Dictionary<string, object>();
        }
    }
}