/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System.Collections.Generic;

namespace MASFoundation
{
    /// <summary>
    /// Server HTTP response data
    /// </summary>
    public sealed class TextResponse
    {
        /// <summary>
        /// Server response headers
        /// </summary>
        public ReadonlyPropertyCollection Headers { get; internal set; }

        /// <summary>
        /// Server response content
        /// </summary>
        public string Text { get; internal set; }

        /// <summary>
        /// HTTP status code
        /// </summary>
        public int StatusCode { get; internal set; }

        /// <summary>
        /// True if response is successful
        /// </summary>
        public bool IsSuccessful { get; internal set; }
    }
}
