/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class JsonValidationRule
    {
        public JsonValidationRule()
        {
        }

        public JsonValidationRule(string path, JsonValueType expectedType)
        {
            Path = path;
            ExpectedType = expectedType;
        }

        public string Path { get; set; }
        public JsonValueType ExpectedType { get; set; }
        public bool IsAcceptingNull { get; set;}
    }
}
