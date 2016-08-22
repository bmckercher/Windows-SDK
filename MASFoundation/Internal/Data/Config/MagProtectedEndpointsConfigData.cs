/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class MagProtectedEndpointsConfigData
    {
        public MagProtectedEndpointsConfigData(JsonObject jsonObject)
        {
            EnterpriseBrowser = jsonObject.GetNamedString("enterprise_browser_endpoint_path");
            DeviceList = jsonObject.GetNamedString("device_list_endpoint_path");
        }

        public string EnterpriseBrowser { get; private set; }
        public string DeviceList { get; private set; }
    }
}
