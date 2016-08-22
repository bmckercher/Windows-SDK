/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System.Collections.Generic;
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
            var foundValue = obj.GetValueFromPath(rule.Path);
  
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
