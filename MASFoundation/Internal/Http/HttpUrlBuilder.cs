using System.Net;
using System.Text;
using Windows.Security.Cryptography;

namespace MASFoundation.Internal.Http
{
    internal class HttpUrlBuilder
    {
        string _baseUrl;

        public HttpUrlBuilder()
        {
        }

        public HttpUrlBuilder(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public void Add(string name, string value)
        {
            if (_sb.Length > 0)
            {
                _sb.Append("&");
            }

            _sb.AppendFormat("{0}={1}", name, WebUtility.UrlEncode(value));
        }

        public void AddNonce()
        {
            var buffer = CryptographicBuffer.GenerateRandom(10);

            string randomHex = CryptographicBuffer.EncodeToHexString(buffer);

            Add("nonce", randomHex);
        }

        public override string ToString()
        {
            if (_baseUrl != null)
            {
                return string.Format("{0}?{1}", _baseUrl, _sb.ToString());
            }
            else
            {
                return _sb.ToString();
            }
        }

        StringBuilder _sb = new StringBuilder();
    }
}
