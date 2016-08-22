/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using MASFoundation.Internal.Http;

namespace MASFoundation.Internal.Data
{
    internal class RegisterResponseData : HttpResponseBaseData
    {
        public RegisterResponseData(HttpTextResponse response) :
            base(response, ResponseType.PlainText)
        {
            Certificate = response.Text;

            string deviceId;
            if (response.Headers.TryGetValue("device-identifier", out deviceId))
            {
                DeviceIdentifier = deviceId;
            }
            else if (response.Headers.TryGetValue("mag-identifier", out deviceId))
            {
                DeviceIdentifier = deviceId;
            }

            string deviceStatus;
            if (response.Headers.TryGetValue("device-status", out deviceStatus))
            {
                DeviceStatus = deviceStatus;
            }
        }

        public string Certificate { get; private set; }
        public string DeviceIdentifier { get; private set; }
        public string DeviceStatus { get; private set; }
    }
}
