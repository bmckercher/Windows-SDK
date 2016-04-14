using System.Collections.Generic;

namespace MASFoundation.Internal.Http
{
    internal class HttpTextResponse
    {
        public IDictionary<string, string> Headers { get; set; }
        public string Text { get; set; }
        public int StatusCode { get; set; }       
        public bool IsSuccessful { get; set; }
    }
}
