/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using MASFoundation.Internal.Http;

namespace MASFoundation.Internal.Data
{
    internal class UnregisterResponseData : HttpResponseBaseData
    {
        public UnregisterResponseData(HttpTextResponse response) :
            base(response, ResponseType.Json)
        {
        }
    }
}
