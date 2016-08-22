/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using MASFoundation.Internal.Http;

namespace MASFoundation.Internal.Data
{
    internal class ClientCredentialsResponseData : HttpResponseBaseData
    {
        public ClientCredentialsResponseData(HttpTextResponse response) :
            base(response, ResponseType.Json)
        {
            ClientId = _responseJson.GetNamedString("client_id");
            ClientSecret = _responseJson.GetNamedString("client_secret");
            Expiration = (long)_responseJson.GetNamedNumber("client_expiration");
        }

        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }
        public long Expiration { get; private set; }
    }
}
