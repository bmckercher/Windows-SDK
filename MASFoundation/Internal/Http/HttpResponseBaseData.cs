using System.Net;
using Windows.Data.Json;

namespace MASFoundation.Internal.Http
{
    internal class HttpResponseBaseData
    {
        public HttpResponseBaseData(HttpTextResponse response, ResponseType responseType = ResponseType.Unknown)
        {
            string error = null;
            string errorDescription = null;

            try
            {
                _response = response;
                _responseJson = JsonObject.Parse(response.Text);

                error = _responseJson.GetNamedString("error", null);
                errorDescription = _responseJson.GetNamedString("error_description", null);
            }
            catch
            {
            }

            if (!_response.IsSuccessful)
            {
                // TODO return a better error here
                ErrorFactory.ThrowError(ErrorCode.NetworkRequestTimedOut, 
                    new HttpRequestException(response, error, errorDescription));
            }
            else
            {
                if ((responseType == ResponseType.Json || responseType == ResponseType.ScimJson) && _responseJson == null)
                {
                    ErrorFactory.ThrowError(ErrorCode.ResponseSerializationFailedToParseResponse,
                        new HttpRequestException(response, null, null));
                }

                if (error != null || errorDescription != null)
                {
                    // TODO return a better error here
                    ErrorFactory.ThrowError(ErrorCode.NetworkRequestTimedOut,
                        new HttpRequestException(response, error, errorDescription));
                }
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
