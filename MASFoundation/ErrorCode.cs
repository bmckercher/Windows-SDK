namespace MASFoundation
{
    public enum ErrorCode : int
    {
        Unknown = -1,

        //
        // Application
        //
        ApplicationAlreadyRegistered,
        ApplicationInvalid,
        ApplicationNotRegistered,
        ApplicationInvalidMagIdentifer,

        //
        // Configuration
        //
        ConfigurationLoadingFailedFileNotFound,
        ConfigurationLoadingFailedJsonSerialization,
        ConfigurationLoadingFailedJsonValidation,
        ConfigurationInvalidEndpoint,

        //
        // Device
        //
        DeviceAlreadyRegistered,
        DeviceAlreadyRegisteredWithDifferentFlow,
        DeviceCouldNotBeDeregistered,
        DeviceNotRegistered,
        DeviceNotLoggedIn,
        DeviceRecordIsNotValid,
        DeviceRegistrationAttemptedWithUnregisteredScope,
        DeviceRegistrationWithoutRequiredParameters,

        //
        // Flow
        //
        FlowIsNotActive,
        FlowIsNotImplemented,
        FlowTypeUnsupported,

        //
        // Geolocation
        //
        GeolocationIsInvalid,
        GeolocationIsMissing,
        GeolocationServicesAreUnauthorized,

        //
        // Network
        //
        NetworkUnacceptableContentType,
        NetworkIsOffline,
        NetworkNotReachable,
        NetworkNotStarted,
        NetworkRequestTimedOut,
        NetworkSSLConnectionCannotBeMade,

        ResponseSerializationFailedToParseResponse,

        //
        // User
        //
        UserAlreadyAuthenticated,
        UserBasicCredentialsNotValid,
        UserDoesNotExist,
        UserNotAuthenticated,

        //
        // Token
        //
        TokenInvalidIdToken,
        TokenIdTokenExpired,
        TokenIdTokenInvalidAud,
        TokenIdTokenInvalidAzp,
        TokenIdTokenInvalidSignature,

        AccessTokenInvalid,
        AccessTokenDisabled,
        AccessTokenNotGrantedScope,

        //
        // Enterprise Browser
        //
        EnterpriseBrowserWebAppInvalidURL,
        EnterpriseBrowserNativeAppDoesNotExist,
        EnterpriseBrowserNativeAppCannotOpen,
        EnterpriseBrowserAppDoesNotExist,

        //
        // BLE
        //
        BLEUnknownState,
        BLEAuthorizationFailed,
        BLEAuthorizationPollingFailed,
        BLECentralDeviceNotFound,
        BLEDelegateNotDefined,
        BLEInvalidAuthenticationProvider,
        BLEPoweredOff,
        BLERestting,
        BLERSSINotInRange,
        BLEUnSupported,
        BLEUnauthorized,
        BLEUserDeclined,
        BLECentral,
        BLEPeripheral,
        BLEPeripheralServices,
        BLEPeripheralCharacteristics,

        //
        // Session Sharing
        //
        SessionSharingAuthorizationInProgress,
        SessionSharingInvalidAuthenticationURL,
        QRCodeSessionSharingAuthorizationPollingFailed,

        Count
    }
}
