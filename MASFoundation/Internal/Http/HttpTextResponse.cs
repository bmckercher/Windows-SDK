/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System.Collections.Generic;

namespace MASFoundation.Internal.Http
{
    internal class HttpTextResponse
    {
        public IDictionary<string, string> Headers { get; set; }
        public string Text { get; set; }
        public int StatusCode { get; set; }       
        public bool IsSuccessful { get; set; }
    }
}
