using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class MagBleConfigData
    {
        public MagBleConfigData(JsonObject jsonObject)
        {
            ServiceUuid = jsonObject.GetNamedString("msso_ble_service_uuid");
            CharacteristicUuid = jsonObject.GetNamedString("msso_ble_characteristic_uuid");
            Rssi = (int)jsonObject.GetNamedNumber("msso_ble_rssi");
        }

        public string ServiceUuid { get; set; }
        public string CharacteristicUuid { get; set; }
        public int Rssi { get; set; }
    }
}
