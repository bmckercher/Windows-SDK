using MASFoundation.Internal.Http;

namespace MASFoundation.Internal.Data
{
    internal class RegisterResponseData : HttpResponseBaseData
    {
        public RegisterResponseData(HttpTextResponse response) :
            base(response, ResponseType.PlainText)
        {
            Certificate = response.Text;

            string deviceId;
            if (response.Headers.TryGetValue("device-identifier", out deviceId))
            {
                DeviceIdentifier = deviceId;
            }
            else if (response.Headers.TryGetValue("mag-identifier", out deviceId))
            {
                DeviceIdentifier = deviceId;
            }
        }

        public string Certificate { get; private set; }
        public string DeviceIdentifier { get; private set; }
    }
}
