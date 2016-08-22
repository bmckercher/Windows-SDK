/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿namespace MASFoundation.Internal.Data
{
    internal class JsonValidationError
    {
        public JsonValidationError(JsonValidationRule rule, JsonValidationErrorKind kind)
        {
            Rule = rule;
            Kind = kind;
        }

        public JsonValidationRule Rule { get; set; }
        public JsonValidationErrorKind Kind { get; set; }

        public override string ToString()
        {
            switch (Kind)
            {
                case JsonValidationErrorKind.IncorrectType:
                    return string.Format("{0} has an unexpected value. {1} is expected", Rule.Path, Rule.ExpectedType.ToString());
                case JsonValidationErrorKind.Missing:
                    return string.Format("{0} could not be found", Rule.Path);
                case JsonValidationErrorKind.None:
                    return "No error detected";
                case JsonValidationErrorKind.NullValue:
                    return string.Format("{0} has a null value", Rule.Path);
                default:
                    return "Unknown error";
            }
        }
    }
}
