using System.Collections.Generic;

namespace MASFoundation
{
    public class TextResponse
    {
        public IDictionary<string, string> Headers { get; internal set; }
        public string Text { get; internal set; }
        public int StatusCode { get; internal set; }
        public bool IsSuccessful { get; internal set; }
    }
}
