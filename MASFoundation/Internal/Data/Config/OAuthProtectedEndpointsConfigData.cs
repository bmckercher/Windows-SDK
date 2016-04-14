using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class OAuthProtectedEndpointsConfigData
    {
        public OAuthProtectedEndpointsConfigData(JsonObject jsonObject)
        {
            UserInfo = jsonObject.GetNamedString("userinfo_endpoint_path");
        }

        public string UserInfo { get; private set; }
    }
}
