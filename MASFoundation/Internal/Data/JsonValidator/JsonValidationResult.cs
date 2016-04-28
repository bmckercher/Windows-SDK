using System.Collections.Generic;

namespace MASFoundation.Internal.Data
{
    internal class JsonValidationResult
    {
        internal void AddError(JsonValidationError error)
        {
            _errors.Add(error);
        }

        public bool HasErrors { get { return _errors.Count > 0; } }

        public IReadOnlyList<JsonValidationError> Errors { get { return _errors.AsReadOnly(); } }

        List<JsonValidationError> _errors = new List<JsonValidationError>();
    }
}
