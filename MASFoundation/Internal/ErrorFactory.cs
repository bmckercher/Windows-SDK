using System;
using System.Text;

namespace MASFoundation.Internal
{
    internal static class ErrorFactory
    {
        public static void ThrowError(ErrorCode error, Exception innerException = null)
        {
            throw new MASException(error, GetErrorText(error, innerException), innerException);
        }

        static string GetErrorText(ErrorCode error, Exception innerException)
        {
            switch (error)
            {
                //
                // Application
                //

                case ErrorCode.ApplicationAlreadyRegistered: return @"The application is already registered with valid credentials";
                case ErrorCode.ApplicationInvalid: return @"The application has invalid credentials";
                case ErrorCode.ApplicationNotRegistered: return @"The application is not registered";
                case ErrorCode.ApplicationInvalidMagIdentifer: return @"Given mag-identifer is invalid.";

                //
                // Configuration
                //
                case ErrorCode.ConfigurationLoadingFailedFileNotFound: return @"The configuration file could not be found";
                case ErrorCode.ConfigurationLoadingFailedJsonSerialization: return @"The configuration file was found but the contents could not be loaded";
                case ErrorCode.ConfigurationLoadingFailedJsonValidation:
                    {
                        StringBuilder sb = new StringBuilder("The configuration was successfully loaded, but the configuration is invalid for the following reason");
                        sb.AppendLine();
                        var configException = innerException as MASConfigException;
                        configException?.Errors.ForEach(e => sb.AppendLine(e));

                        return sb.ToString();
                    }
                case ErrorCode.ConfigurationInvalidEndpoint: return @"Invalid endpoint";

                //
                // Device
                //

                case ErrorCode.DeviceAlreadyRegistered: return @"This device has already been registered and has not been configured to accept updates";
                case ErrorCode.DeviceAlreadyRegisteredWithDifferentFlow: return @"This device has already been registered within a different flow";
                case ErrorCode.DeviceCouldNotBeDeregistered: return @"This device could not be deregistered on the Gateway";
                case ErrorCode.DeviceNotRegistered: return @"This device is not registered";
                case ErrorCode.DeviceNotLoggedIn: return @"This device is not logged in";
                case ErrorCode.DeviceRecordIsNotValid: return @"The registered device record is invalid";
                case ErrorCode.DeviceRegistrationAttemptedWithUnregisteredScope: return @"Attempted to register the device with a Scope that isn't registered in the application record on the Gateway";

                //
                // Network
                //
                case ErrorCode.NetworkNotReachable: return @"The network host is not currently reachable";
                case ErrorCode.ResponseSerializationFailedToParseResponse: return @"Invalid response format - failed to parse response";

                //
                // User
                //
                case ErrorCode.UserAlreadyAuthenticated: return @"A user is already authenticated";
                case ErrorCode.UserBasicCredentialsNotValid: return @"Username or password invalid";
                case ErrorCode.UserNotAuthenticated: return @"A user is not authenticated";

                //
                // Token
                //                
                case ErrorCode.AccessTokenNotGrantedScope: return @"Given access token is not granted for required scope.";
                case ErrorCode.AccessTokenDisabled: return @"Given access token is disabled";
                case ErrorCode.AccessTokenInvalid: return @"Invalid access token";

                //
                // Default
                //
                default: return string.Format(@"Unrecognized error code of value: {0}", error);
            }
        }
    }
}
