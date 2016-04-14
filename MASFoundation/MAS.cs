using MASFoundation.Internal;
using MASFoundation.Internal.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace MASFoundation
{
    public static class MAS
    {
        static MAS()
        {
            ConfigFileName = "msso_config.json";
        }

        /// <summary>
        /// Name of the configuration file.  This gives the ability to set the file's name to a custom value.
        /// </summary>
        public static string ConfigFileName
        {
            get;
            set;
        }
    
        /// <summary>
        /// Starts the lifecycle of the MAS processes.  This includes the registration of the application to the Gateway, if the network is available.
        /// </summary>
        /// <returns></returns>
        public static Task StartAsync()
        {
            return Session.Instance.StartAsync(ConfigFileName);
        }

        /// <summary>
        /// Stop all processes in the framework.
        /// </summary>
        /// <returns></returns>
        public static Task StopAsync()
        {
            return Session.Instance.StopAsync();
        }

        /// <summary>
        /// Reset all application, device, and user credentials in memory, or in the local and shared storage.
        /// </summary>
        /// <returns></returns>
        public static Task ResetAsync()
        {
            return Session.Instance.ResetAsync();
        }

        /// <summary>
        /// Remove the device’s record from the MAG.
        /// </summary>
        /// <returns></returns>
        public static async Task DeregisterCurrentDeviceAsync()
        {
            await Session.Instance.UnregisterDevice();
        }

        /// <summary>
        /// Authenticate a user with basic credentials.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task AuthenticateUserAsync(string username, string password)
        {
            await Session.Instance.LoginUserAsync(username, password);
        }

        /// <summary>
        /// Logoff a currently-authenticated user.
        /// </summary>
        /// <returns></returns>
        public static async Task LogoffUserAsync()
        {
            await Session.Instance.LogoffUserAsync();
        }

        /// <summary>
        /// This method makes HTTP DELETE calls to an endpoint.
        /// </summary>
        /// <param name="endPointPath"></param>
        /// <param name="parameterInfo"></param>
        /// <param name="headerInfo"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public static async Task<MASTextResponse> DeleteFromAsync(string endPointPath, 
            IDictionary<string, string> parameterInfo, 
            IDictionary<string, string> headerInfo,
            MASResponseType responseType)
        {
            if (Session.Instance.Device == null || !Session.Instance.Device.IsRegistered)
            {
                // TODO throw device not registered error
            }

            var builder = new HttpUrlBuilder(endPointPath);

            if (parameterInfo != null)
            {
                foreach (var paramInfo in parameterInfo)
                {
                    builder.Add(paramInfo.Key, paramInfo.Value);
                }
            }

            var headers = await SetupRequestHeaders(headerInfo, MASRequestType.None, responseType);

            return ToMASResponse(await HttpRequester.RequestTextAsync(new HttpRequestInfo()
            {
                Url = builder.ToString(),
                Method = HttpMethod.Delete,
                Headers = headers,
                Certificate = Session.Instance.Device.Certificate
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
        public static async Task<MASTextResponse> GetFromAsync(string endPointPath, 
            IDictionary<string, string> parameterInfo, 
            IDictionary<string, string> headerInfo,
            MASResponseType responseType)
        {
            if (Session.Instance.Device == null || !Session.Instance.Device.IsRegistered)
            {
                // TODO throw device not registered error
            }

            var builder = new HttpUrlBuilder(endPointPath);

            if (parameterInfo != null)
            {
                foreach (var paramInfo in parameterInfo)
                {
                    builder.Add(paramInfo.Key, paramInfo.Value);
                }
            }

            var headers = await SetupRequestHeaders(headerInfo, MASRequestType.None, responseType);

            return ToMASResponse(await HttpRequester.RequestTextAsync(new HttpRequestInfo()
            {
                Url = builder.ToString(),
                Method = HttpMethod.Get,
                Headers = headers,
                Certificate = Session.Instance.Device.Certificate
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
        public static async Task<MASTextResponse> PostToAsync(string endPointPath,
            string body,
            IDictionary<string, string> headerInfo,
            MASRequestType requestType,
            MASResponseType responseType)
        {
            if (Session.Instance.Device == null || !Session.Instance.Device.IsRegistered)
            {
                // TODO throw device not registered error
            }

            var headers = await SetupRequestHeaders(headerInfo, requestType, responseType);

            return ToMASResponse(await HttpRequester.RequestTextAsync(new HttpRequestInfo()
            {
                Url = endPointPath,
                Method = HttpMethod.Post,
                Headers = headers,
                Certificate = Session.Instance.Device.Certificate,
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
        public static Task<MASTextResponse> PostToAsync(string endPointPath, 
            IDictionary<string, string> parameterInfo, 
            IDictionary<string, string> headerInfo,
            MASRequestType requestType,
            MASResponseType responseType)
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
        public static async Task<MASTextResponse> PutTo(string endPointPath,
            string body,
            IDictionary<string, string> headerInfo,
            MASRequestType requestType,
            MASResponseType responseType)
        {
            if (Session.Instance.Device == null || !Session.Instance.Device.IsRegistered)
            {
                // TODO throw device not registered error
            }

            var headers = await SetupRequestHeaders(headerInfo, requestType, responseType);

            return ToMASResponse(await HttpRequester.RequestTextAsync(new HttpRequestInfo()
            {
                Url = endPointPath,
                Method = HttpMethod.Put,
                Headers = headers,
                Certificate = Session.Instance.Device.Certificate,
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
        public static Task<MASTextResponse> PutTo(string endPointPath, 
            IDictionary<string, string> parameterInfo, 
            IDictionary<string, string> headerInfo,
            MASRequestType requestType,
            MASResponseType responseType)
        {
            var body = FormatBody(requestType, parameterInfo);

            return PutTo(endPointPath, body, headerInfo, requestType, responseType);
        }

        static string FormatBody(MASRequestType type, IDictionary<string, string> parameterInfo)
        {
            string body = string.Empty;

            switch (type)
            {
                case MASRequestType.FormUrlEncoded:
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

        static async Task<Dictionary<string, string>> SetupRequestHeaders(IDictionary<string, string> givenHeaders, MASRequestType requestType, MASResponseType responseType)
        {
            var deviceMagId = Session.Instance.Device.MagId;
            var headers = new Dictionary<string, string>
            {
                { "mag-identifier", deviceMagId }
            };

            var accessTokenHeaderValue = await Session.Instance.User.GetAccessHeaderValueAsync();
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

            if (responseType != MASResponseType.Unknown)
            {
                headers[HttpHeaders.Accept] = ToHeaderValue(responseType);
            }

            if (requestType != MASRequestType.None)
            {
                headers[HttpHeaders.ContentType] = ToHeaderValue(requestType);
            }

            return headers;
        }

        static string ToHeaderValue(MASResponseType type)
        {
            switch(type)
            {
                default:
                case MASResponseType.Unknown:
                    return "";
                case MASResponseType.Json:
                    return HttpContentTypes.Json;
                case MASResponseType.PlainText:
                    return HttpContentTypes.Plain;
                case MASResponseType.ScimJson:
                    return HttpContentTypes.ScimJson;
                case MASResponseType.Xml:
                    return HttpContentTypes.Xml;
            }
        }

        static string ToHeaderValue(MASRequestType type)
        {
            switch (type)
            {
                default:
                case MASRequestType.None:
                    return "";
                case MASRequestType.Json:
                    return HttpContentTypes.Json;
                case MASRequestType.PlainText:
                    return HttpContentTypes.Plain;
                case MASRequestType.ScimJson:
                    return HttpContentTypes.ScimJson;
                case MASRequestType.FormUrlEncoded:
                    return HttpContentTypes.UrlEncoded;
                case MASRequestType.Xml:
                    return HttpContentTypes.Xml;
            }
        }

        static MASTextResponse ToMASResponse(HttpTextResponse response)
        {
            return new MASTextResponse()
            {
                Headers = response.Headers,
                IsSuccessful = response.IsSuccessful,
                StatusCode = response.StatusCode,
                Text = response.Text
            };
        }
    }
}
