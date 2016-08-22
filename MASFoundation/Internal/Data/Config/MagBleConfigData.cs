/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class MagBleConfigData
    {
        public MagBleConfigData(JsonObject jsonObject)
        {
            ServiceUuid = jsonObject.GetNamedString("msso_ble_service_uuid");
            CharacteristicUuid = jsonObject.GetNamedString("msso_ble_characteristic_uuid");
            Rssi = (int)jsonObject.GetNamedNumber("msso_ble_rssi");
        }

        public string ServiceUuid { get; set; }
        public string CharacteristicUuid { get; set; }
        public int Rssi { get; set; }
    }
}
