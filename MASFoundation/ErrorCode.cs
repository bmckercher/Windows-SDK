namespace MASFoundation
{
    /// <summary>
    /// MAS SDK errors
    /// </summary>
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

        //
        // Network
        //
        NetworkNotReachable,
        ResponseSerializationFailedToParseResponse,

        //
        // User
        //
        UserAlreadyAuthenticated,
        UserBasicCredentialsNotValid,
        UserNotAuthenticated,

        //
        // Token
        //
        AccessTokenInvalid,
        AccessTokenDisabled,
        AccessTokenNotGrantedScope,
    }
}
