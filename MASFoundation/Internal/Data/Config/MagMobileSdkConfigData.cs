/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System.Collections.Generic;
using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class MagMobileSdkConfigData
    {
        public MagMobileSdkConfigData(JsonObject jsonObject)
        {
            IsSSOEnabled = jsonObject.GetNamedBoolean("sso_enabled");
            IsLocationEnabled = jsonObject.GetNamedBoolean("location_enabled");
            LocationProvider = jsonObject.GetNamedString("location_provider");
            IsMsIsdnEnabled = jsonObject.GetNamedBoolean("msisdn_enabled");
            IsPublicKeyPinningEnabled = jsonObject.GetNamedBoolean("enable_public_key_pinning");
            IsTrustedPublicPki = jsonObject.GetNamedBoolean("trusted_public_pki");
            ClientCertRsaKeybits = (int)jsonObject.GetNamedNumber("client_cert_rsa_keybits");

            var hashes = new List<string>();
            var jsonHashes = jsonObject.GetNamedArray("trusted_cert_pinned_public_key_hashes");
            foreach (var jsonHash in jsonHashes)
            {
                hashes.Add(jsonHash.GetString());
            }
            TrustedCertPinnedPublicKeyHashes = hashes.AsReadOnly();
        }

        public bool IsSSOEnabled { get; private set; }
        public bool IsLocationEnabled { get; private set; }
        public string LocationProvider { get; private set; }
        public bool IsMsIsdnEnabled { get; private set; }
        public bool IsPublicKeyPinningEnabled { get; private set; }
        public bool IsTrustedPublicPki { get; private set; }
        public IReadOnlyList<string> TrustedCertPinnedPublicKeyHashes { get; private set; }
        public int ClientCertRsaKeybits { get; private set; }
    }
}
