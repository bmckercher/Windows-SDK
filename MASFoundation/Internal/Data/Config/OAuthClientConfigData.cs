/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System.Collections.Generic;
using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class OAuthClientConfigData
    {
        public OAuthClientConfigData(JsonObject jsonObject)
        {
            Organization = jsonObject.GetNamedString("organization");
            Description = jsonObject.GetNamedString("description");
            ClientName = jsonObject.GetNamedString("client_name");
            ClientType = jsonObject.GetNamedString("client_type");
            RegisteredBy = jsonObject.GetNamedString("registered_by");

            var clientIds = new List<OAuthClientIdConfigData>();
            var jsonClientIds = jsonObject.GetNamedArray("client_ids");
            foreach (var jsonClientId in jsonClientIds)
            {
                clientIds.Add(new OAuthClientIdConfigData(jsonClientId.GetObject()));
            }
            ClientIds = clientIds.AsReadOnly();
        }

        public string Organization { get; private set; }
        public string Description { get; private set; }
        public string ClientName { get; private set; }
        public string ClientType { get; private set; }
        public string RegisteredBy { get; private set; }
        public IReadOnlyList<OAuthClientIdConfigData> ClientIds { get; private set; }
    }
}
