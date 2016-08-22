/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class OAuthConfigData
    {
        public OAuthConfigData(JsonObject jsonObject)
        {
            Client = new OAuthClientConfigData(jsonObject.GetNamedObject("client"));
            SystemEndpoints = new OAuthSystemEndpointsConfigData(jsonObject.GetNamedObject("system_endpoints"));
            ProjectedEndpoints = new OAuthProtectedEndpointsConfigData(jsonObject.GetNamedObject("oauth_protected_endpoints"));
        }

        public OAuthClientConfigData Client { get; private set; }
        public OAuthSystemEndpointsConfigData SystemEndpoints { get; private set; }
        public OAuthProtectedEndpointsConfigData ProjectedEndpoints { get; private set; }
    }
}
