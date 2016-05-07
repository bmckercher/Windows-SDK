using MASFoundation.Internal;
using MASFoundation.Internal.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage;

namespace MASFoundation
{
    public static class MAS
    {
        static MAS()
        {
            ConfigFileName = "msso_config.json";

            // User is the default registration flow
            RegistrationKind = RegistrationKind.User;

            // No logging by default
            LogLevel = LogLevel.None;

            MASApplication.Current.LoginRequested += Application_LoginRequested;
        }

        #region Public Properties

        /// <summary>
        /// Name of the configuration file.  This gives the ability to set the file's name to a custom value.
        /// </summary>
        public static string ConfigFileName { get; set; }

        /// <summary>
        /// Requested registration flow (client or user)
        /// </summary>
        public static RegistrationKind RegistrationKind { get; set; }

        /// <summary>
        /// What to log: full, errors only, or none
        /// </summary>
        public static LogLevel LogLevel { get; set; }

        #endregion

        #region Public Events

        /// <summary>
        /// Login is requested.  Application should present a login dialog or page to the user to enter their credentials.
        /// </summary>
        public static event EventHandler<object> LoginRequested;

        /// <summary>
        /// A debug log message has been logged.
        /// </summary>
        public static event EventHandler<string> LogMessage;

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the lifecycle of the MAS processes.  This includes the registration of the application to the Gateway, if the network is available.
        /// </summary>
        /// <returns></returns>
        public static IAsyncAction StartAsync()
        {
            return StartInternalAsync(null).AsAsyncAction();
        }

        /// <summary>
        /// Starts the lifecycle of the MAS processes.  This includes the registration of the application to the Gateway, if the network is available.
        /// </summary>
        /// <param name="configText">The contents of the configuration file as an alternative to ConfigFileName.</param>
        /// <returns></returns>
        public static IAsyncAction StartWithConfigAsync(string configText)
        {
            return StartInternalAsync(configText).AsAsyncAction();
        }

        /// <summary>
        /// Reset all application, device, and user credentials in memory, or in the local and shared storage.
        /// </summary>
        /// <returns></returns>
        public static IAsyncAction ResetAsync()
        {
            return MASApplication.Current.ResetAsync().AsAsyncAction();
        }

        /// <summary>
        /// This method makes HTTP DELETE calls to an endpoint.
        /// </summary>
        /// <param name="endPointPath"></param>
        /// <param name="parameterInfo"></param>
        /// <param name="headerInfo"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public static IAsyncOperation<TextResponse> DeleteFromAsync(string endPointPath, 
            IDictionary<string, string> parameterInfo, 
            IDictionary<string, string> headerInfo,
            ResponseType responseType)
        {
            return RequestHttpAsync(HttpMethod.DELETE, endPointPath, parameterInfo, 
                headerInfo, RequestType.None, responseType).AsAsyncOperation<TextResponse>();
        }

        /// <summary>
        /// This method makes HTTP GET calls to an endpoint.
        /// </summary>
        /// <param name="endPointPath"></param>
        /// <param name="parameterInfo"></param>
        /// <param name="headerInfo"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public static IAsyncOperation<TextResponse> GetFromAsync(string endPointPath, 
            IDictionary<string, string> parameterInfo, 
            IDictionary<string, string> headerInfo,
            ResponseType responseType)
        {
            return RequestHttpAsync(HttpMethod.GET, endPointPath, parameterInfo,
                headerInfo, RequestType.None, responseType).AsAsyncOperation<TextResponse>();
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
        [DefaultOverload]
        public static IAsyncOperation<TextResponse> PostToAsync(string endPointPath,
            string body,
            IDictionary<string, string> headerInfo,
            RequestType requestType,
            ResponseType responseType)
        {
            var parameterInfo = new Dictionary<string, string>
            {
                { "Body", body }
            };

            return RequestHttpAsync(HttpMethod.POST, endPointPath, parameterInfo,
                headerInfo, requestType, responseType).AsAsyncOperation<TextResponse>();
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
        public static IAsyncOperation<TextResponse> PostToAsync(string endPointPath, 
            IDictionary<string, string> parameterInfo, 
            IDictionary<string, string> headerInfo,
            RequestType requestType,
            ResponseType responseType)
        {
            return RequestHttpAsync(HttpMethod.POST, endPointPath, parameterInfo,
                headerInfo, requestType, responseType).AsAsyncOperation<TextResponse>();
        }

        /// <summary>
        /// This method makes HTTP PATCH calls to an endpoint.
        /// </summary>
        /// <param name="endPointPath"></param>
        /// <param name="body"></param>
        /// <param name="headerInfo"></param>
        /// <param name="requestType"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        [DefaultOverload]
        public static IAsyncOperation<TextResponse> PatchToAsync(string endPointPath,
            string body,
            IDictionary<string, string> headerInfo,
            RequestType requestType,
            ResponseType responseType)
        {
            var parameterInfo = new Dictionary<string, string>
            {
                { "Body", body }
            };

            return RequestHttpAsync(HttpMethod.PATCH, endPointPath, parameterInfo,
                headerInfo, requestType, responseType).AsAsyncOperation<TextResponse>();
        }

        /// <summary>
        /// This method makes HTTP PATCH calls to an endpoint.
        /// </summary>
        /// <param name="endPointPath"></param>
        /// <param name="parameterInfo"></param>
        /// <param name="headerInfo"></param>
        /// <param name="requestType"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public static IAsyncOperation<TextResponse> PatchToAsync(string endPointPath,
            IDictionary<string, string> parameterInfo,
            IDictionary<string, string> headerInfo,
            RequestType requestType,
            ResponseType responseType)
        {
            return RequestHttpAsync(HttpMethod.PATCH, endPointPath, parameterInfo,
                headerInfo, requestType, responseType).AsAsyncOperation<TextResponse>();
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
        [DefaultOverload]
        public static IAsyncOperation<TextResponse> PutToAsync(string endPointPath,
            string body,
            IDictionary<string, string> headerInfo,
            RequestType requestType,
            ResponseType responseType)
        {
            var parameterInfo = new Dictionary<string, string>
            {
                { "Body", body }
            };

            return RequestHttpAsync(HttpMethod.PUT, endPointPath, parameterInfo,
                headerInfo, requestType, responseType).AsAsyncOperation<TextResponse>();
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
        public static IAsyncOperation<TextResponse> PutToAsync(string endPointPath, 
            IDictionary<string, string> parameterInfo, 
            IDictionary<string, string> headerInfo,
            RequestType requestType,
            ResponseType responseType)
        {
            return RequestHttpAsync(HttpMethod.PUT, endPointPath, parameterInfo,
                headerInfo, requestType, responseType).AsAsyncOperation<TextResponse>();
        }

        #endregion

        #region Internal Methods

        internal static void RaiseLogMessage(string message)
        {
            LogMessage?.Invoke(null, message);
        }

        #endregion

        #region Private Methods

        static async Task StartInternalAsync(string configContent)
        {
            //string configContent = ConfigText;
            if (configContent == null)
            {
                if (string.IsNullOrEmpty(ConfigFileName))
                {
                    ErrorFactory.ThrowError(ErrorCode.ConfigurationLoadingFailedFileNotFound);
                }

                try
                {
                    var dataUri = new Uri("ms-appx:///" + ConfigFileName, UriKind.Absolute);
                    var file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                    configContent = await FileIO.ReadTextAsync(file);
                }
                catch (Exception e)
                {
                    ErrorFactory.ThrowError(ErrorCode.ConfigurationLoadingFailedFileNotFound, e);
                }
            }

            await MASApplication.Current.StartAsync(configContent, RegistrationKind);
        }

        static async Task<TextResponse> RequestHttpAsync(HttpMethod method, string endPointPath,
            IDictionary<string, string> parameterInfo,
            IDictionary<string, string> headerInfo,
            RequestType requestType,
            ResponseType responseType)
        {
            if (MASDevice.Current == null || !MASDevice.Current.IsApplicationRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            if (!MASDevice.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.DeviceNotRegistered);
            }

            string url = endPointPath;
            string body = null;

            if (method == HttpMethod.GET || method == HttpMethod.DELETE)
            {
                var builder = new HttpUrlBuilder(endPointPath);
                if (parameterInfo != null)
                {
                    foreach (var paramInfo in parameterInfo)
                    {
                        builder.Add(paramInfo.Key, paramInfo.Value);
                    }
                }

                url = builder.ToString();
            }
            else
            {
                body = FormatBody(requestType, parameterInfo);
            }

            var headers = await SetupRequestHeaders(headerInfo, requestType, responseType);

            return ToMASResponse(await HttpRequestFactory.RequestTextAsync(new HttpRequestInfo()
            {
                Url = url,
                Method = method,
                Headers = headers,
                Body = body,
                Certificate = MASDevice.Current.Certificate
            }));
        }


        static void Application_LoginRequested(object sender, EventArgs e)
        {
            LoginRequested?.Invoke(null, e);
        }

        static string FormatBody(RequestType type, IDictionary<string, string> parameterInfo)
        {
            string body = string.Empty;

            if (parameterInfo != null)
            {
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
            }

            return body;
        }

        static async Task<Dictionary<string, string>> SetupRequestHeaders(IDictionary<string, string> givenHeaders, RequestType requestType, ResponseType responseType)
        {
            var deviceMagId = MASDevice.Current.MagId;
            var headers = new Dictionary<string, string>
            {
                { "mag-identifier", deviceMagId }
            };

            if (MASUser.Current != null && MASUser.Current.IsLoggedIn)
            {
                var accessTokenHeaderValue = await MASUser.Current.GetAccessHeaderValueAsync();
                if (accessTokenHeaderValue != null)
                {
                    headers[HttpHeaders.Authorization] = accessTokenHeaderValue;
                }
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
    }
}
