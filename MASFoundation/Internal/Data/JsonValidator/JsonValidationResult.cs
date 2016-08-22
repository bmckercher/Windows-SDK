/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System.Collections.Generic;

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
