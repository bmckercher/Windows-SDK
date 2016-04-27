using System.Collections.Generic;
using Windows.Security.Cryptography.Certificates;


namespace MASFoundation.Internal.Http
{
    internal class HttpRequestInfo
    {
        public HttpMethod Method { get; set; }
        public string Url { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public string Body { get; set; }
        public Certificate Certificate { get; set; }
    }
}
