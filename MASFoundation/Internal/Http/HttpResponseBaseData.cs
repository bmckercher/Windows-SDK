﻿using Windows.Data.Json;

namespace MASFoundation.Internal.Http
{
    internal class HttpResponseBaseData
    {
        public HttpResponseBaseData(HttpTextResponse response, ResponseType responseType = ResponseType.Unknown)
        {
            try
            {
                _response = response;
                _responseJson = JsonObject.Parse(response.Text);

                _error = _responseJson.GetStringOrNull("error");
                _errorDescription = _responseJson.GetStringOrNull("error_description");
            }
            catch
            {
            }

            string errorCodeText = _response.Headers["x-ca-err"];
            if (errorCodeText != null)
            {
                int code;
                if (int.TryParse(errorCodeText, out code))
                {
                    _apiErrorCode = code;
                }
            }

            if (!_response.IsSuccessful)
            {
                ThrowError(response);
            }

            if ((responseType == ResponseType.Json || responseType == ResponseType.ScimJson) && _responseJson == null)
            {
                ErrorFactory.ThrowError(ErrorCode.ResponseSerializationFailedToParseResponse,
                    new HttpRequestException(response, null, null));
            }
        }
        
        public int HttpStatusCode
        {
            get
            {
                return _response.StatusCode;
            }
        }

        protected void ThrowError(HttpTextResponse response)
        {
            var code = (ApiErrorCode)_apiErrorCode.GetValueOrDefault((int)ApiErrorCode.Unknown);

            // We could not get the code from the header, some other network error has occured
            if (code != ApiErrorCode.Unknown)
            {
                ErrorFactory.ThrowError(ToErrorCode(code),
                    new HttpRequestException(response, _error, _errorDescription));
            }
            else
            {
                ErrorFactory.ThrowError(ErrorCode.NetworkNotReachable,
                    new HttpRequestException(response, _error, _errorDescription));
            }
        }

        protected ErrorCode ToErrorCode(ApiErrorCode code)
        {
            switch (code)
            {

                //
                // ClientId / ClientSecret
                //
                case ApiErrorCode.RequestAuthorizationInitClientCredentialsInvalid:
                    return ErrorCode.ApplicationInvalid;

                //
                // MAG Identifier
                //
                case ApiErrorCode.RequestClientCredentialsMagIdentifierInvalid:
                    return ErrorCode.ApplicationInvalidMagIdentifer;

                case ApiErrorCode.RemoveDeviceMagIdentifierMissingOrUnknown:
                case ApiErrorCode.RemoveDeviceMagIdentifierInvalid:
                case ApiErrorCode.RemoveDeviceMagIdentifierInvalid2:
                    return ErrorCode.DeviceCouldNotBeDeregistered;

                //
                // Device is already registered
                //
                case ApiErrorCode.RegisterDeviceAlreadyRegistered:
                case ApiErrorCode.RegisterDeviceAlreadyRegistered2:
                    return ErrorCode.DeviceAlreadyRegistered;

                //
                // Device record is not valid
                //
                case ApiErrorCode.RequestTokenMagIdentifierInvalid:
                case ApiErrorCode.RequestTokenMagIdentifierInvalid2:
                    return ErrorCode.DeviceRecordIsNotValid;

                //
                // Basic user credentials missing or invalid
                //
                case ApiErrorCode.RegisterDeviceUserCredentialsInvalid:
                case ApiErrorCode.RequestAuthorizationLoginUserCredentialsInvalid:
                case ApiErrorCode.RequestTokenResourceOwnerCredentialsInvalid:
                case ApiErrorCode.RequestTokenResourceOwnerCredentialsInvalid2:
                    return ErrorCode.UserBasicCredentialsNotValid;

                //
                // Token
                //
                case ApiErrorCode.TokenInvalidAccessTokenSuffix:
                    return ErrorCode.AccessTokenInvalid;
                case ApiErrorCode.TokenDisabledSuffix:
                    return ErrorCode.AccessTokenDisabled;
                case ApiErrorCode.TokenNotGrantedForScopeSuffix:
                    return ErrorCode.AccessTokenNotGrantedScope;

                default:
                    return ErrorCode.Unknown;
            }
        }

        protected HttpTextResponse _response;
        protected JsonObject _responseJson;

        protected string _error = null;
        protected string _errorDescription = null;
        protected int? _apiErrorCode = null;
    }
}
