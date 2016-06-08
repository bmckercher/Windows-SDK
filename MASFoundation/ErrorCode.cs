namespace MASFoundation
{
    /// <summary>
    /// MAS SDK errors
    /// </summary>
    public enum ErrorCode : int
    {
        /// <summary>
        /// Unknown error
        /// </summary>
        Unknown = -1,

        //
        // Application
        //

        /// <summary>
        /// The application is already registered with valid credentials
        /// </summary>
        ApplicationAlreadyRegistered,

        /// <summary>
        /// The application has invalid credentials
        /// </summary>
        ApplicationInvalid,

        /// <summary>
        /// The application is not registered
        /// </summary>
        ApplicationNotRegistered,

        /// <summary>
        /// Given mag-identifer is invalid.
        /// </summary>
        ApplicationInvalidMagIdentifer,

        //
        // Configuration
        //

        /// <summary>
        /// The configuration was null or empty
        /// </summary>
        ConfigurationLoadingFailedNullOrEmpty,

        /// <summary>
        /// The configuration file could not be found
        /// </summary>
        ConfigurationLoadingFailedFileNotFound,

        /// <summary>
        /// The configuration file was found but the contents could not be loaded
        /// </summary>
        ConfigurationLoadingFailedJsonSerialization,

        /// <summary>
        /// The configuration was successfully loaded, but the configuration is invalid
        /// </summary>
        ConfigurationLoadingFailedJsonValidation,

        /// <summary>
        /// Invalid endpoint
        /// </summary>
        ConfigurationInvalidEndpoint,

        //
        // Device
        //

        /// <summary>
        /// This device has already been registered and has not been configured to accept updates
        /// </summary>
        DeviceAlreadyRegistered,

        /// <summary>
        /// This device has already been registered within a different flow
        /// </summary>
        DeviceAlreadyRegisteredWithDifferentFlow,

        /// <summary>
        /// This device could not be deregistered on the Gateway
        /// </summary>
        DeviceCouldNotBeDeregistered,

        /// <summary>
        /// This device is not registered
        /// </summary>
        DeviceNotRegistered,

        /// <summary>
        /// This device is not logged in
        /// </summary>
        DeviceNotLoggedIn,

        /// <summary>
        /// The registered device record is invalid
        /// </summary>
        DeviceRecordIsNotValid,

        /// <summary>
        /// Attempted to register the device with a Scope that isn't registered in the application record on the Gateway
        /// </summary>
        DeviceRegistrationAttemptedWithUnregisteredScope,

        //
        // Network
        //

        /// <summary>
        /// The network host is not currently reachable
        /// </summary>
        NetworkNotReachable,

        /// <summary>
        /// Invalid response format - failed to parse response
        /// </summary>
        ResponseSerializationFailedToParseResponse,

        //
        // User
        //

        /// <summary>
        /// A user is already authenticated
        /// </summary>
        UserAlreadyAuthenticated,

        /// <summary>
        /// Username or password invalid
        /// </summary>
        UserBasicCredentialsNotValid,

        /// <summary>
        /// A user is not authenticated
        /// </summary>
        UserNotAuthenticated,

        //
        // Token
        //

        /// <summary>
        /// Access token has expired
        /// </summary>
        TokenAccessExpired,

        /// <summary>
        /// Given access token is not granted for required scope.
        /// </summary>
        AccessTokenInvalid,

        /// <summary>
        /// Given access token is disabled
        /// </summary>
        AccessTokenDisabled,

        /// <summary>
        /// Invalid access token
        /// </summary>
        AccessTokenNotGrantedScope,

        /// <summary>
        /// Number of errors
        /// </summary>
        Count,
    }
}
