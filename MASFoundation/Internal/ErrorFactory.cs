using System;
using System.Collections.Generic;

namespace MASFoundation.Internal
{
    internal static class ErrorFactory
    {
        static Dictionary<ErrorKind, string> _errors = new Dictionary<ErrorKind, string>
        {
            { ErrorKind.None, "None" },
            { ErrorKind.UnableToLoadConfigurationFile, "Failed to load configuration file!" },
            { ErrorKind.InvalidConfigurationFile, "Invalid configuration file!" }
        };

        public static void throwError(ErrorKind error, Exception innerException = null)
        {
            string info = null;
            if (_errors.TryGetValue(error, out info))
            {
                throw new MASException(error, info, innerException);
            }
            else
            {
                throw new Exception("Unknown error has occurred", innerException);
            }
        }
    }
}
