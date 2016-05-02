namespace MASFoundation.Internal
{
    internal enum ApiErrorCode
    {
        Unknown = -1,

        // Register Device
        RegisterDeviceRequestInvalid = 1000000,
        RegisterDeviceCertificateInvalid = 1000101,
        RegisterDeviceCertificateFormatUnsupported = 1000102,
        RegisterDeviceHeadersOrParametersInvalid = 1000103,
        RegisterDeviceCRSInvalid = 1000104,
        RegisterDeviceAlreadyRegistered = 1000105,
        RegisterDeviceAlreadyRegistered2 = 1007105,
        RegisterDeviceGrantInvalid = 1000113,
        RegisterDeviceClientCredentialsInvalid = 1000201,
        RegisterDeviceUserCredentialsInvalid = 1000202,

        // Remove Device
        RemoveDeviceRequestInvalid = 1001000,
        RemoveDeviceMagIdentifierMissingOrUnknown = 1001101,
        RemoveDeviceMagIdentifierInvalid = 1001103,
        RemoveDeviceMagIdentifierInvalid2 = 1001107,
        RemoveDeviceTokenInvalid = 1001106,
        RemoveDeviceClientCredentialsInvalid = 1001201,

        // Request Client Credentials
        RequestClientCredentialsServerError = 1002000,
        RequestClientCredentialsHeadersOrParametersInvalid = 1002103,
        RequestClientCredentialsMagIdentifierInvalid = 1002107,
        RequestClientCredentialsDeviceIsNotActive = 1002108,
        RequestClientCredentialsClientIdIsNotMasterKey = 1002109,
        RequestClientCredentialsClientCrenditalsInvalid = 1002201,
        RequestClientCredentialsUrlPrefixInvalid = 1002203,

        // Request Authorization Init
        RequestAuthorizationInitServerError = 3000000,
        RequestAuthorizationInitParametersInvalid = 3000103,
        RequestAuthorizationInitMagIdentifierInvalid = 3000107,
        RequestAuthorizationInitDeviceIsNotActive = 3000108,
        RequestAuthorizationInitRedirectUriInvalid = 3000114,
        RequestAuthorizationInitScopeInvalid = 3000115,
        RequestAuthorizationInitResponseTypeInvalid = 3000116,
        RequestAuthorizationInitClientTypeInvalid = 3000117,
        RequestAuthorizationInitNoRedirectUri = 3000130,
        RequestAuthorizationInitClientCredentialsInvalid = 3000201,
        RequestAuthorizationInitUrlPrefixInvalid = 3000203,

        // Request Authorization Login
        RequestAuthorizationLoginServerError = 3001000,
        RequestAuthorizationLoginParametersInvalid = 3001103,
        RequestAuthorizationLoginSessionInvalid = 3001110,
        RequestAuthorizationLoginRedirectUrlInvalid = 3001114,
        RequestAuthorizationLoginAuthenticationDenied = 3001123,
        RequestAuthorizationLoginUserCredentialsInvalid = 3001202,
        RequestAuthorizationLoginUrlPrefixInvalid = 3001203,

        // Request Authorization Login
        RequestAuthorizationConsentServerError = 3002000,
        RequestAuthorizationConsentParametersInvalid = 3002103,
        RequestAuthorizationConsentSessionInvalid = 3002110,
        RequestAuthorizationConsentAuthorizationDenied = 3002124,
        RequestAuthorizationConsentUrlPrefixInvalid = 3002203,

        // Request Token
        RequestTokenServerError = 3003000,
        RequestTokenMagIdentifierInvalid = 3003101,
        RequestTokenMissingOrDuplicateParameters = 3003103,
        RequestTokenMagIdentifierInvalid2 = 3003107,
        RequestTokenGrantInvalid = 3003113,
        RequestTokenClientHasNoRegisteredScopeRequested = 3003115,
        RequestTokenClientIsNotAuthorizedForRequest = 3003117,
        RequestTokenGrantTypeIsNotSupported = 3003119,
        RequestTokenClientCredentialsInvalid = 3003201,
        RequestTokenResourceOwnerCredentialsInvalid = 3003302,
        RequestTokenResourceOwnerCredentialsInvalid2 = 3003202,
        RequestTokenPrefixIsInvalid = 3003203,
        RequestTokenTokenDisabled = 3003993,

        // Revoke Token
        RevokeTokenServerError = 3004000,
        RevokeTokenClientIsNotAuthorizedForRequest = 3004117,
        RevokeTokenUnsupportedType = 3004118,
        RevokeTokenClientCredentialsInvalid = 3004201,
        RevokeAuthorizationConsentUrlPrefixInvalid = 3004203,

        TokenNotGrantedForScopeSuffix = 991,
        TokenInvalidAccessTokenSuffix = 992,
        TokenDisabledSuffix = 993
    }
}
