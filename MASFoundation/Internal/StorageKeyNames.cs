/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿namespace MASFoundation.Internal
{
    /// <summary>
    /// Contains all of our secure storage keys.
    /// </summary>
    internal static class StorageKeyNames
    {
        public static readonly string RegisteredCertSubject = "registeredCertSubject";
        public static readonly string DeviceInfo = "deviceInfo";
        public static readonly string ClientInfo = "clientInfo";
        public static readonly string ClientAccessInfo = "clientAccessInfo";
        public static readonly string UserAccessInfo = "userAccessInfo";
        public static readonly string PrivateKey = "privateKey";
    }
}
