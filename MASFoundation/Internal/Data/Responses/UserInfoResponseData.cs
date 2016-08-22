/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using MASFoundation.Internal.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class UserInfoResponseData : HttpResponseBaseData, IUserInfo
    {
        public UserInfoResponseData(HttpTextResponse response) :
            base(response, ResponseType.Json)
        {
            Sub = _responseJson.GetNamedString("sub");
            Name = _responseJson.GetStringOrNull("name");
            if (Name == null)
            {
                Name = _responseJson.GetStringOrNull("given_name");
            }
            FamilyName = _responseJson.GetStringOrNull("family_name");
            Nickname = _responseJson.GetStringOrNull("nickname");
            PerferredUsername = _responseJson.GetStringOrNull("preferred_username");
            Email = _responseJson.GetStringOrNull("email");
            Phone = _responseJson.GetStringOrNull("phone");

            var addressObj = _responseJson.GetNamedObject("address");
            if (addressObj != null)
            {
                Address = new AddressResponseData(addressObj);
            }
        }

        public string Sub { get; private set; }
        public string Name { get; private set; }
        public string FamilyName { get; private set; }
        public string Nickname { get; private set; }
        public string PerferredUsername { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public IAddressInfo Address { get; private set; }
    }

    internal class AddressResponseData : IAddressInfo
    {
        public AddressResponseData(JsonObject jsonObject)
        {
            Region = jsonObject.GetStringOrNull("region");
            Country = jsonObject.GetStringOrNull("country");
            StreetAddress = jsonObject.GetStringOrNull("street_address");
            Locality = jsonObject.GetStringOrNull("locality");
            PostalCode = jsonObject.GetStringOrNull("postal_code");
        }

        public string Region { get; private set; }
        public string Country { get; private set; }
        public string StreetAddress { get; private set; }
        public string Locality { get; private set; }
        public string PostalCode { get; private set; }
    }

}
