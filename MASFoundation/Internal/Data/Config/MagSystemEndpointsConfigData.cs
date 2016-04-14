using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class MagSystemEndpointsConfigData
    {
        public MagSystemEndpointsConfigData(JsonObject jsonObject)
        {
            DeviceRegister = jsonObject.GetNamedString("device_register_endpoint_path");
            DeviceClientRegister = jsonObject.GetNamedString("device_client_register_endpoint_path");
            DeviceRemove = jsonObject.GetNamedString("device_remove_endpoint_path");
            ClientCredentialInit = jsonObject.GetNamedString("client_credential_init_endpoint_path");
        }

        public string DeviceRegister { get; private set; }
        public string DeviceClientRegister { get; private set; }
        public string DeviceRemove { get; private set; }
        public string ClientCredentialInit { get; private set; }

    }
}
