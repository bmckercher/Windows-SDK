/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using MASFoundation.Internal.Http;

namespace MASFoundation.Internal.Data
{
    internal class AuthorizationProvidersResponseData : HttpResponseBaseData
    {
        public AuthorizationProvidersResponseData(HttpTextResponse response) :
            base(response, ResponseType.Json)
        {
            // TODO not supported yet

            // Sample response below
//          {
//              "idp": "all",
//              "providers": [
//                  {
//                      "provider": {
//                          "id": "facebook",
//                          "auth_url": "https://test.pulsenow.co.uk:443/facebook/login?sessionID=44b04c62-2a5b-40f0-aa32-ec10fdf5cf36"
//                      }
//                  },
//                  {
//                      "provider": {
//                          "id": "google",
//                          "auth_url": "https://test.pulsenow.co.uk:443/google/login?sessionID=44b04c62-2a5b-40f0-aa32-ec10fdf5cf36"
//                      }
//                  },
//                  {
//                      "provider": {
//                          "id": "salesforce",
//                          "auth_url": "https://test.pulsenow.co.uk:443/salesforce/login?sessionID=44b04c62-2a5b-40f0-aa32-ec10fdf5cf36"
//                      }
//                  },
//                  {
//                      "provider": {
//                          "id": "linkedin",
//                          "auth_url": "https://test.pulsenow.co.uk:443/linkedin/login?sessionID=44b04c62-2a5b-40f0-aa32-ec10fdf5cf36"
//                      }
//                  },
//                  {
//                      "provider": {
//                          "id": "enterprise",
//                          "auth_url": "https://test.pulsenow.co.uk:443/enterprise/login?sessionID=44b04c62-2a5b-40f0-aa32-ec10fdf5cf36"
//                      }
//                  },
//                  {
//                      "provider": {
//                          "id": "qrcode",
//                          "auth_url": "https://test.pulsenow.co.uk:443/auth/device/authorize?sessionID=44b04c62-2a5b-40f0-aa32-ec10fdf5cf36",
//                          "poll_url": "https://test.pulsenow.co.uk:443/auth/device/authorization/10965ae36d7f4371b84abf840fbfdd49f4f72a259abb406cada6e63fbe3b924f"
//                      }
//                  }
//              ]
//          }
        }
    }
}
