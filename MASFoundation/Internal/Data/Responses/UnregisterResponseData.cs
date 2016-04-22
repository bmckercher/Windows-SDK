using MASFoundation.Internal.Http;

namespace MASFoundation.Internal.Data
{
    internal class UnregisterResponseData : HttpResponseBaseData
    {
        public UnregisterResponseData(HttpTextResponse response) :
            base(response, ResponseType.Json)
        {
        }
    }
}
