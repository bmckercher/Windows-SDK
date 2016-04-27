using MASFoundation.Internal.Http;

namespace MASFoundation.Internal.Data
{
    internal class RequestTokenResponseData : HttpResponseBaseData
    {
        public RequestTokenResponseData(HttpTextResponse response) :
            base(response, ResponseType.Json)
        {
            AccessToken = _responseJson.GetNamedString("access_token");
            TokenType = _responseJson.GetNamedString("token_type");
            ExpiresIn = (int)_responseJson.GetNamedNumber("expires_in");
            RefreshToken = _responseJson.GetStringOrNull("refresh_token");
            Scope = _responseJson.GetNamedString("scope");

            IdToken = _responseJson.GetStringOrNull("id_token");
            IdTokenType = _responseJson.GetStringOrNull("id_token_type");
        }

        public string AccessToken { get; private set; }
        public string TokenType { get; private set; }
        public int ExpiresIn { get; private set; }
        public string RefreshToken { get; private set; }
        public string Scope { get; private set; }
        public string IdToken { get; private set; }
        public string IdTokenType { get; private set; }

    }
}
