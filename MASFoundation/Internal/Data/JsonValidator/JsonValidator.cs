using System.Collections.Generic;
using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class JsonValidator
    {
        public void AddRule(JsonValidationRule rule)
        {
            _rules.Add(rule);
        }

        public JsonValidationResult Validate(JsonObject obj)
        {
            JsonValidationResult result = new JsonValidationResult();

            foreach (var rule in _rules)
            {
                var error = ValidateWithRule(rule, obj);
                if (error != null)
                {
                    result.AddError(error);
                }
            }

            return result;
        }

        JsonValidationError ValidateWithRule(JsonValidationRule rule, JsonObject obj)
        {
            var names = rule.Path.Split('.');

            IJsonValue foundValue = obj;
            foreach (var name in names)
            {
                if (foundValue.ValueType != JsonValueType.Object)
                {
                    foundValue = null;
                    break;
                }

                var parentObj = foundValue.GetObject();

                var arrayStartPos = name.IndexOf('[');
                
                if (arrayStartPos > 0)
                {
                    foundValue = null;
                    var arrayEndPos = name.IndexOf(']', arrayStartPos);

                    if (arrayEndPos > 0)
                    {
                        var arrayName = name.Substring(0, arrayStartPos);
                        var arrayIndexText = name.Substring(arrayStartPos + 1, arrayEndPos - arrayStartPos - 1);

                        uint arrayIndex;
                        if (parentObj.ContainsKey(arrayName) && uint.TryParse(arrayIndexText, out arrayIndex))
                        {
                            var foundArray = parentObj.GetNamedArray(arrayName);
                            if (arrayIndex < foundArray.Count)
                            {
                                foundValue = foundArray.GetObjectAt(arrayIndex);
                            }
                        }
                    }

                    if (foundValue == null)
                    {
                        break;
                    }
                }
                else
                {
                    if (parentObj.ContainsKey(name))
                    {
                        foundValue = parentObj.GetNamedValue(name);
                    }
                }
               
            }

            JsonValidationErrorKind validationError;
            if (foundValue == null)
            {
                validationError = JsonValidationErrorKind.Missing;
            }
            else
            {
                if (foundValue.ValueType == JsonValueType.Null)
                {
                    validationError = JsonValidationErrorKind.NullValue;
                }
                else if (foundValue.ValueType != rule.ExpectedType)
                {
                    validationError = JsonValidationErrorKind.IncorrectType;
                }
                else
                {
                    validationError = JsonValidationErrorKind.None;
                }
            }

            if (validationError == JsonValidationErrorKind.None)
            {
                return null;
            }
            else
            {
                return new JsonValidationError(rule, validationError);
            }
        }

        List<JsonValidationRule> _rules = new List<JsonValidationRule>();
    }
}
