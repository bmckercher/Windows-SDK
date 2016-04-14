using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class OAuthSystemEndpointsConfigData
    {
        public OAuthSystemEndpointsConfigData(JsonObject jsonObject)
        {
            Authorization = jsonObject.GetNamedString("authorization_endpoint_path");
            Token = jsonObject.GetNamedString("token_endpoint_path");
            TokenRevocation = jsonObject.GetNamedString("token_revocation_endpoint_path");
            UserSessionLogout = jsonObject.GetNamedString("usersession_logout_endpoint_path");
            UserSessionStatus = jsonObject.GetNamedString("usersession_status_endpoint_path");
        }

        public string Authorization { get; private set; }
        public string Token { get; private set; }
        public string TokenRevocation { get; private set; }
        public string UserSessionLogout { get; private set; }
        public string UserSessionStatus { get; private set; }

    }
}
