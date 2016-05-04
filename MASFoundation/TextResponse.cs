using System.Collections.Generic;

namespace MASFoundation
{
    /// <summary>
    /// Server HTTP response data
    /// </summary>
    public class TextResponse
    {
        /// <summary>
        /// Server response headers
        /// </summary>
        public IDictionary<string, string> Headers { get; internal set; }

        /// <summary>
        /// Server response content
        /// </summary>
        public string Text { get; internal set; }

        /// <summary>
        /// HTTP status code
        /// </summary>
        public int StatusCode { get; internal set; }

        /// <summary>
        /// True if response is successful
        /// </summary>
        public bool IsSuccessful { get; internal set; }
    }
}
