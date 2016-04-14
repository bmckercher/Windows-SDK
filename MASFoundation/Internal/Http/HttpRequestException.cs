using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASFoundation.Internal.Http
{
    internal class HttpRequestException : Exception
    {
        public HttpRequestException(HttpTextResponse responseData, string error, string errorDescription)
        {
            Response = responseData;
            Error = error;
            ErrorDescription = errorDescription;
        }

        public HttpTextResponse Response { get; private set; }
        public string Error { get; private set; }
        public string ErrorDescription { get; private set; }
    }
}
