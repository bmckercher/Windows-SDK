/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class OAuthProtectedEndpointsConfigData
    {
        public OAuthProtectedEndpointsConfigData(JsonObject jsonObject)
        {
            UserInfo = jsonObject.GetNamedString("userinfo_endpoint_path");
        }

        public string UserInfo { get; private set; }
    }
}
