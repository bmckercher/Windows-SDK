using MASFoundation.Internal.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class UserInfoResponseData : HttpResponseBaseData
    {
        public UserInfoResponseData(HttpTextResponse response) :
            base(response)
        {
            Sub = _responseJson.GetNamedString("sub");
            Name = _responseJson.GetNamedString("name");
            FamilyName = _responseJson.GetNamedString("family_name");
            Nickname = _responseJson.GetNamedString("nickname");
            PerferredUsername = _responseJson.GetNamedString("preferred_username");
            Email = _responseJson.GetNamedString("email");
            Phone = _responseJson.GetNamedString("phone");
            Address = new AddressResponseData(_responseJson.GetNamedObject("address"));
        }

        public string Sub { get; private set; }
        public string Name { get; private set; }
        public string FamilyName { get; private set; }
        public string Nickname { get; private set; }
        public string PerferredUsername { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public AddressResponseData Address { get; private set; }
    }

    internal class AddressResponseData
    {
        public AddressResponseData(JsonObject jsonObject)
        {
            Region = jsonObject.GetNamedString("region");
            Country = jsonObject.GetNamedString("country");
        }

        public string Region { get; private set; }
        public string Country { get; private set; }
    }

}
