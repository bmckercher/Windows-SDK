using System.Net;
using Windows.Data.Json;

namespace MASFoundation.Internal.Http
{
    internal class HttpResponseBaseData
    {
        public HttpResponseBaseData(HttpTextResponse response, bool jsonExpected = true)
        {
            string error = null;
            string errorDescription = null;

            try
            {
                _response = response;
                _responseJson = JsonObject.Parse(response.Text);

                error = _responseJson.GetNamedString("error");
                errorDescription = _responseJson.GetNamedString("error_description");
            }
            catch
            {
            }

            if (!_response.IsSuccessful || (jsonExpected && _responseJson == null) || error != null || errorDescription != null)
            {
                ErrorFactory.throwError(ErrorKind.HttpRequestError, new HttpRequestException(response, error, errorDescription));
            }
        }
        
        public int HttpStatusCode
        {
            get
            {
                return _response.StatusCode;
            }
        }

        protected HttpTextResponse _response;
        protected JsonObject _responseJson;
    }
}
