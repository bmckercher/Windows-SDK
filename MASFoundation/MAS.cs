using MASFoundation.Internal;
using MASFoundation.Internal.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MASFoundation
{
    public static class MAS
    {
        static MAS()
        {
            ConfigFileName = "msso_config.json";
            RegistrationKind = RegistrationKind.Client;

            _session.LoginRequested += _session_LoginRequested;
        }

        private static void _session_LoginRequested(object sender, EventArgs e)
        {
            LoginRequested?.Invoke(null, e);
        }

        /// <summary>
        /// Name of the configuration file.  This gives the ability to set the file's name to a custom value.
        /// </summary>
        public static string ConfigFileName { get; set; }

        /// <summary>
        /// Requested registration flow (client or user)
        /// </summary>
        public static RegistrationKind RegistrationKind { get; set; }

        /// <summary>
        /// Logger to use for debug messages
        /// </summary>
        public static ILogger Logger { get; set; }

        /// <summary>
        /// What to log: full, errors only, or none
        /// </summary>
        public static LogLevel LogLevel { get; set; }

        /// <summary>
        /// Login is requested as appart of user registration flow for this device
        /// </summary>
        public static event EventHandler LoginRequested;

        /// <summary>
        /// Starts the lifecycle of the MAS processes.  This includes the registration of the application to the Gateway, if the network is available.
        /// </summary>
        /// <returns></returns>
        public static Task StartAsync()
        {
            return _session.StartAsync(ConfigFileName, RegistrationKind);
        }

        /// <summary>
        /// Stop all processes in the framework.
        /// </summary>
        /// <returns></returns>
        public static Task StopAsync()
        {
            return _session.StopAsync();
        }

        /// <summary>
        /// Reset all application, device, and user credentials in memory, or in the local and shared storage.
        /// </summary>
        /// <returns></returns>
        public static Task ResetAsync()
        {
            return _session.ResetAsync();
        }

        /// <summary>
        /// Remove the device’s record from the MAG.
        /// </summary>
        /// <returns></returns>
        public static async Task UnregisterDeviceAsync()
        {
            await _session.UnregisterDevice();
        }

        /// <summary>
        /// Authenticate a user with basic credentials.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task AuthenticateUserAsync(string username, string password)
        {
            await _session.LoginUserAsync(username, password);
        }

        /// <summary>
        /// Logoff a currently-authenticated user.
        /// </summary>
        /// <returns></returns>
        public static async Task LogoffUserAsync()
        {
            await _session.LogoffUserAsync();
        }

        /// <summary>
        /// Logs out the device by removing the device registration from the server and removing the cached id token and access token from the device. The shared keychain contains the id_token, certificate, keys, and device identifier. Using logoutDevice also clears up the id_token on the MAG. 
        /// </summary>
        /// <returns></returns>
        public static async Task LogoutDeviceAsync()
        {
            await _session.LogoutDeviceAsync();
        }

        /// <summary>
        /// This method makes HTTP DELETE calls to an endpoint.
        /// </summary>
        /// <param name="endPointPath"></param>
        /// <param name="parameterInfo"></param>
        /// <param name="headerInfo"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public static async Task<TextResponse> DeleteFromAsync(string endPointPath, 
            IDictionary<string, string> parameterInfo, 
            IDictionary<string, string> headerInfo,
            ResponseType responseType)
        {
            if (!_session.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            var builder = new HttpUrlBuilder(endPointPath);

            if (parameterInfo != null)
            {
                foreach (var paramInfo in parameterInfo)
                {
                    builder.Add(paramInfo.Key, paramInfo.Value);
                }
            }

            var headers = await SetupRequestHeaders(headerInfo, RequestType.None, responseType);

            return ToMASResponse(await HttpRequestFactory.RequestTextAsync(new HttpRequestInfo()
            {
                Url = builder.ToString(),
                Method = HttpMethod.DELETE,
                Headers = headers,
                Certificate = _session.Certificate
            }));
        }

        /// <summary>
        /// This method makes HTTP GET calls to an endpoint.
        /// </summary>
        /// <param name="endPointPath"></param>
        /// <param name="parameterInfo"></param>
        /// <param name="headerInfo"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public static async Task<TextResponse> GetFromAsync(string endPointPath, 
            IDictionary<string, string> parameterInfo, 
            IDictionary<string, string> headerInfo,
            ResponseType responseType)
        {
            if (!_session.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            var builder = new HttpUrlBuilder(endPointPath);

            if (parameterInfo != null)
            {
                foreach (var paramInfo in parameterInfo)
                {
                    builder.Add(paramInfo.Key, paramInfo.Value);
                }
            }

            var headers = await SetupRequestHeaders(headerInfo, RequestType.None, responseType);

            return ToMASResponse(await HttpRequestFactory.RequestTextAsync(new HttpRequestInfo()
            {
                Url = builder.ToString(),
                Method = HttpMethod.GET,
                Headers = headers,
                Certificate = _session.Certificate
            }));
        }

        /// <summary>
        /// This method makes HTTP POST calls to an endpoint.
        /// </summary>
        /// <param name="endPointPath"></param>
        /// <param name="body"></param>
        /// <param name="headerInfo"></param>
        /// <param name="requestType"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public static async Task<TextResponse> PostToAsync(string endPointPath,
            string body,
            IDictionary<string, string> headerInfo,
            RequestType requestType,
            ResponseType responseType)
        {
            if (!_session.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            var headers = await SetupRequestHeaders(headerInfo, requestType, responseType);

            return ToMASResponse(await HttpRequestFactory.RequestTextAsync(new HttpRequestInfo()
            {
                Url = endPointPath,
                Method = HttpMethod.POST,
                Headers = headers,
                Certificate = _session.Certificate,
                Body = body ?? string.Empty
            }));
        }

        /// <summary>
        /// This method makes HTTP POST calls to an endpoint.
        /// </summary>
        /// <param name="endPointPath"></param>
        /// <param name="parameterInfo"></param>
        /// <param name="headerInfo"></param>
        /// <param name="requestType"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public static Task<TextResponse> PostToAsync(string endPointPath, 
            IDictionary<string, string> parameterInfo, 
            IDictionary<string, string> headerInfo,
            RequestType requestType,
            ResponseType responseType)
        {
            var body = FormatBody(requestType, parameterInfo);

            return PostToAsync(endPointPath, body, headerInfo, requestType, responseType);
        }

        /// <summary>
        /// This method makes HTTP PUT calls to an endpoint.
        /// </summary>
        /// <param name="endPointPath"></param>
        /// <param name="body"></param>
        /// <param name="headerInfo"></param>
        /// <param name="requestType"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public static async Task<TextResponse> PutToAsync(string endPointPath,
            string body,
            IDictionary<string, string> headerInfo,
            RequestType requestType,
            ResponseType responseType)
        {
            if (!_session.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            var headers = await SetupRequestHeaders(headerInfo, requestType, responseType);

            return ToMASResponse(await HttpRequestFactory.RequestTextAsync(new HttpRequestInfo()
            {
                Url = endPointPath,
                Method = HttpMethod.PUT,
                Headers = headers,
                Certificate = _session.Certificate,
                Body = body
            }));
        }

        /// <summary>
        /// This method makes HTTP PUT calls to an endpoint.
        /// </summary>
        /// <param name="endPointPath"></param>
        /// <param name="parameterInfo"></param>
        /// <param name="headerInfo"></param>
        /// <param name="requestType"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public static Task<TextResponse> PutToAsync(string endPointPath, 
            IDictionary<string, string> parameterInfo, 
            IDictionary<string, string> headerInfo,
            RequestType requestType,
            ResponseType responseType)
        {
            var body = FormatBody(requestType, parameterInfo);

            return PutToAsync(endPointPath, body, headerInfo, requestType, responseType);
        }

        #region Private Methods

        static string FormatBody(RequestType type, IDictionary<string, string> parameterInfo)
        {
            string body = string.Empty;

            switch (type)
            {
                case RequestType.FormUrlEncoded:
                    {
                        HttpUrlBuilder builder = new HttpUrlBuilder();
                        foreach (var item in parameterInfo)
                        {
                            builder.Add(item.Key, item.Value);
                        }
                        body = builder.ToString();
                    }
                    break;
                default:
                    body = parameterInfo.Values.FirstOrDefault() ?? string.Empty;
                    break;
            }

            return body;
        }

        static async Task<Dictionary<string, string>> SetupRequestHeaders(IDictionary<string, string> givenHeaders, RequestType requestType, ResponseType responseType)
        {
            var deviceMagId = _session.MagId;
            var headers = new Dictionary<string, string>
            {
                { "mag-identifier", deviceMagId }
            };

            var accessTokenHeaderValue = await _session.GetAccessHeaderValueAsync();
            if (accessTokenHeaderValue != null)
            {
                headers[HttpHeaders.Authorization] = accessTokenHeaderValue;
            }

            if (givenHeaders != null)
            {
                foreach (var header in givenHeaders)
                {
                    headers[header.Key] = header.Value;
                }
            }

            if (responseType != ResponseType.Unknown)
            {
                headers[HttpHeaders.Accept] = ToHeaderValue(responseType);
            }

            if (requestType != RequestType.None)
            {
                headers[HttpHeaders.ContentType] = ToHeaderValue(requestType);
            }

            return headers;
        }

        static string ToHeaderValue(ResponseType type)
        {
            switch(type)
            {
                default:
                case ResponseType.Unknown:
                    return "";
                case ResponseType.Json:
                    return HttpContentTypes.Json;
                case ResponseType.PlainText:
                    return HttpContentTypes.Plain;
                case ResponseType.ScimJson:
                    return HttpContentTypes.ScimJson;
                case ResponseType.Xml:
                    return HttpContentTypes.Xml;
            }
        }

        static string ToHeaderValue(RequestType type)
        {
            switch (type)
            {
                default:
                case RequestType.None:
                    return "";
                case RequestType.Json:
                    return HttpContentTypes.Json;
                case RequestType.PlainText:
                    return HttpContentTypes.Plain;
                case RequestType.ScimJson:
                    return HttpContentTypes.ScimJson;
                case RequestType.FormUrlEncoded:
                    return HttpContentTypes.UrlEncoded;
                case RequestType.Xml:
                    return HttpContentTypes.Xml;
            }
        }

        static TextResponse ToMASResponse(HttpTextResponse response)
        {
            return new TextResponse()
            {
                Headers = response.Headers,
                IsSuccessful = response.IsSuccessful,
                StatusCode = response.StatusCode,
                Text = response.Text
            };
        }

        #endregion

        // The one and only session
        static Session _session = new Session();
    }
}
