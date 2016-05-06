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

            LogLevel = LogLevel.None;

            Application.Current.LoginRequested += Application_LoginRequested;
        }

        private static void Application_LoginRequested(object sender, EventArgs e)
        {
            LoginRequested?.Invoke(null, e);
        }

        /// <summary>
        /// Name of the configuration file.  This gives the ability to set the file's name to a custom value.
        /// </summary>
        public static string ConfigFileName { get; set; }

        /// <summary>
        /// The contents of the configuration file as an alternative to ConfigFileName.  If this is set, SDK will not load a configuration 
        /// file with the name given by ConfigFileName.
        /// </summary>
        public static string ConfigText { get; set; }

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
        /// Login is requested.  Application should present a login dialog or page to the user to enter their credentials.
        /// </summary>
        public static event EventHandler<object> LoginRequested;

        /// <summary>
        /// A debug log message has been logged.
        /// </summary>
        public static event EventHandler<string> LogMessage;

        /// <summary>
        /// Starts the lifecycle of the MAS processes.  This includes the registration of the application to the Gateway, if the network is available.
        /// </summary>
        /// <returns></returns>
        public static IAsyncAction StartAsync()
        {
            return StartInternalAsync().AsAsyncAction();
        } 

        internal static void RaiseLogMessage(string message)
        {
            LogMessage?.Invoke(null, "Info: " + message);
        }

        static async Task StartInternalAsync()
        {
            string configContent = ConfigText;

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

            await Application.Current.StartAsync(configContent, RegistrationKind);
        }

        /// <summary>
        /// Reset all application, device, and user credentials in memory, or in the local and shared storage.
        /// </summary>
        /// <returns></returns>
        public static IAsyncAction ResetAsync()
        {
            return Application.Current.ResetAsync().AsAsyncAction();
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
            if (!Device.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            return RequestHttpAsync(HttpMethod.DELETE, endPointPath, parameterInfo, 
                headerInfo, RequestType.None, responseType).AsAsyncOperation<TextResponse>();
        }

        static async Task<TextResponse> RequestHttpAsync(HttpMethod method, string endPointPath,
            IDictionary<string, string> parameterInfo,
            IDictionary<string, string> headerInfo,
            RequestType requestType,
            ResponseType responseType)
        {
            if (!Device.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
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
                Certificate = Device.Current.Certificate
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
        public static IAsyncOperation<TextResponse> GetFromAsync(string endPointPath, 
            IDictionary<string, string> parameterInfo, 
            IDictionary<string, string> headerInfo,
            ResponseType responseType)
        {
            if (!Device.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

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
            if (!Device.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

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
            if (!Device.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

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
            if (!Device.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

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
            if (!Device.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

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
            if (!Device.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

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
            if (!Device.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            return RequestHttpAsync(HttpMethod.PUT, endPointPath, parameterInfo,
                headerInfo, requestType, responseType).AsAsyncOperation<TextResponse>();
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
            var deviceMagId = Device.Current.MagId;
            var headers = new Dictionary<string, string>
            {
                { "mag-identifier", deviceMagId }
            };

            if (User.Current != null && User.Current.IsLoggedIn)
            {
                var accessTokenHeaderValue = await User.Current.GetAccessHeaderValueAsync();
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
