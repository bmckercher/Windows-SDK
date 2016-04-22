using MASFoundation.Internal.Http;

namespace MASFoundation.Internal.Data
{
    internal class RegisterResponseData : HttpResponseBaseData
    {
        public RegisterResponseData(HttpTextResponse response) :
            base(response, ResponseType.PlainText)
        {
            Certificate = response.Text;
            DeviceIdentifier = response.Headers["device-identifier"];
        }

        public string Certificate { get; private set; }
        public string DeviceIdentifier { get; private set; }
    }
}
