using System;

namespace MASFoundation.Internal
{
    internal static class ErrorFactory
    {
        public static void ThrowError(ErrorCode error, Exception innerException = null)
        {
            throw new MASException(error, GetErrorText(error), innerException);
        }

        static string GetErrorText(ErrorCode error)
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
                case ErrorCode.ConfigurationLoadingFailedFileNotFound: return @"The configuration file %@ could not be found";
                case ErrorCode.ConfigurationLoadingFailedJsonSerialization: return @"The configuration file %@ was found but the contents could not be loaded with description\n\n\'%@\'";
                case ErrorCode.ConfigurationLoadingFailedJsonValidation: return @"The configuration was successfully loaded, but the configuration is invalid for the following reason\n\n'%@'";
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
                case ErrorCode.DeviceRegistrationWithoutRequiredParameters: return @"The device registration does not have the required parameters";

                //
                // Flow
                //

                case ErrorCode.FlowIsNotActive: return @"There is not a currently active flow";
                case ErrorCode.FlowIsNotImplemented: return @"This flow type has not yet been implemented";
                case ErrorCode.FlowTypeUnsupported: return @"This flow type is not yet supported";

                //
                // Geolocation
                //

                case ErrorCode.GeolocationIsMissing: return @"No location coordinates found and they are required.";
                case ErrorCode.GeolocationIsInvalid: return @"The current location is not valid.";
                case ErrorCode.GeolocationServicesAreUnauthorized: return @"The geolocation services are unauthorized.";

                //
                // Network
                //

                case ErrorCode.NetworkUnacceptableContentType: return @"The network detected an unacceptable content-type";
                case ErrorCode.NetworkIsOffline: return @"The network appears to be offline";
                case ErrorCode.NetworkSSLConnectionCannotBeMade: return @"An SSL error has occurred, this may be caused by attempting to connect to a server using TLS version below 1.2.\n\n";
                case ErrorCode.NetworkNotStarted: return @"The network is not started";
                case ErrorCode.NetworkNotReachable: return @"The network host is not currently reachable";
                case ErrorCode.NetworkRequestTimedOut: return @"The network request has timed out";

                case ErrorCode.ResponseSerializationFailedToParseResponse: return @"Invalid response format - failed to parse response";

                //
                // User
                //
                case ErrorCode.UserAlreadyAuthenticated: return @"A user is already authenticated";
                case ErrorCode.UserBasicCredentialsNotValid: return @"Username or password invalid";
                case ErrorCode.UserDoesNotExist: return @"A user does not exist";
                case ErrorCode.UserNotAuthenticated: return @"A user is not authenticated";

                //
                // Token
                //
                case ErrorCode.TokenInvalidIdToken: return @"JWT Validation: id_token is invalid";
                case ErrorCode.TokenIdTokenExpired: return @"JWT Validation: id_token is expired";
                case ErrorCode.TokenIdTokenInvalidAud: return @"JWT Validation: aud value does not match";
                case ErrorCode.TokenIdTokenInvalidAzp: return @"JWT Validation: azp value does not match";
                case ErrorCode.TokenIdTokenInvalidSignature: return @"JWT Validation: signature does not match";
                 
                case ErrorCode.AccessTokenNotGrantedScope: return @"Given access token is not granted for required scope.";
                case ErrorCode.AccessTokenDisabled: return @"Given access token is disabled";
                case ErrorCode.AccessTokenInvalid: return @"Invalid access token";

                //
                // EnterpriseBrowser
                //
                case ErrorCode.EnterpriseBrowserWebAppInvalidURL: return @"Invalid webapp auth URL";
                case ErrorCode.EnterpriseBrowserNativeAppDoesNotExist: return @"Native app does not exist";
                case ErrorCode.EnterpriseBrowserNativeAppCannotOpen: return @"Error loading the native app";
                case ErrorCode.EnterpriseBrowserAppDoesNotExist: return @"Enterprise Browser App does not exist";

                //
                // BLE
                //
                case ErrorCode.BLEUnknownState: return @"Unknown error occured while enabling BLE Central";
                case ErrorCode.BLEPoweredOff: return @"Bluetooth is currently off";
                case ErrorCode.BLERestting: return @"Bluetooth connection is momentarily lost; restting the connection";
                case ErrorCode.BLEUnauthorized: return @"Bluetooth feature is not authorized for this application";
                case ErrorCode.BLEUnSupported: return @"Bluetooth feature is not supported";
                case ErrorCode.BLEDelegateNotDefined: return @"MASDevice's BLE delegate is not defined. Delegate is mandatory to acquire permission from the user.";
                case ErrorCode.BLEAuthorizationFailed: return @"BLE authorization failed due to invalid or expired authorization request.";
                case ErrorCode.BLECentralDeviceNotFound: return @"BLE authorization failed due to no subscribed central device.";
                case ErrorCode.BLERSSINotInRange: return @"BLE RSSI is not in range.  Please refer to msso_config.json for BLE RSSI configuration.";
                case ErrorCode.BLEAuthorizationPollingFailed: return @"BLE authorization failed while polling authorization code from gateway.";
                case ErrorCode.BLEInvalidAuthenticationProvider: return @"BLE authorization failed due to invalid authentication provider.";
                case ErrorCode.BLECentral: return @"BLE Central error encountered in CBCentral with specific reason in userInfo.";
                case ErrorCode.BLEPeripheral: return @"BLE Peripheral error encountered while discovering, or connecting central device with specific reason in userInfo.";
                case ErrorCode.BLEPeripheralServices: return @"BLE Peripheral error encountered while discovering or connecting peripheral services with specific reason in userInfo.";
                case ErrorCode.BLEPeripheralCharacteristics: return @"BLE Peripheral error encountered while discovering, connecting, or writing peripheral service's characteristics with specific reason in userInfo.";

                //
                // Session Sharing
                //
                case ErrorCode.SessionSharingAuthorizationInProgress: return @"Authorization is currently in progress through session sharing.";
                case ErrorCode.QRCodeSessionSharingAuthorizationPollingFailed: return @"QR Code session sharing authentication failed with specific information on userInfo.";
                case ErrorCode.SessionSharingInvalidAuthenticationURL: return @"Invalid authentication URL is provided for session sharing.";

                //
                // Default
                //
                default: return string.Format(@"Unrecognized error code of value: {0}", error);
            }
        }
    }
}
