using System;
using System.Text;

namespace MASFoundation.Internal
{
    internal static class ErrorFactory
    {
        public static void ThrowError(ErrorCode error, Exception innerException = null)
        {
            throw new MASException(error, error.ToErrorString(innerException), innerException);
        }
    }
}
