using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class MagConfigData
    {
        public MagConfigData(JsonObject jsonObject)
        {
            SystemEndpoints = new MagSystemEndpointsConfigData(jsonObject.GetNamedObject("system_endpoints"));
            ProjectedEndpoints = new MagProtectedEndpointsConfigData(jsonObject.GetNamedObject("oauth_protected_endpoints"));
            MobileSdk = new MagMobileSdkConfigData(jsonObject.GetNamedObject("mobile_sdk"));
            Ble = new MagBleConfigData(jsonObject.GetNamedObject("ble"));
        }
        
        public MagSystemEndpointsConfigData SystemEndpoints { get; private set; }
        public MagProtectedEndpointsConfigData ProjectedEndpoints { get; private set; }
        public MagMobileSdkConfigData MobileSdk { get; private set; }
        public MagBleConfigData Ble { get; private set; }
    }
}
