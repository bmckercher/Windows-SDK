using MASFoundation.Internal.Data;
using MASFoundation.Internal.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace MASFoundation.Internal
{

    // MAG API requests: https://docops.ca.com/ca-mobile-api-gateway/3-0/mobile-api-gateway-configuration/mag-api-endpoints
    //

    internal static class MAGRequests
    {
        public static Task<ClientCredentialsResponseData> GetClientCredentialsAsync(Configuration config, string deviceIdBase64)
        {
            var url = config.GetEndpointPath(config.Mag.SystemEndpoints.ClientCredentialInit);

            var clientId = config.OAuth.Client.ClientIds[0];

            var builder = new HttpUrlBuilder();
            builder.Add("client_id", clientId.Id);
            builder.AddNonce();

            var headers = new Dictionary<string, string>
            {
                { HttpHeaders.Authorization, clientId.BasicAuthValue },
                { "device-id", deviceIdBase64 },
                { HttpHeaders.ContentType, HttpContentTypes.UrlEncoded },
                { HttpHeaders.Accept, HttpContentTypes.Json }
            };

            return HttpRequester.RequestAsync<ClientCredentialsResponseData>(new HttpRequestInfo()
            {
                Method = HttpMethod.Post,
                Url = url,
                Headers = headers,
                Body = builder.ToString()
            });
        }

        public static async Task<AuthorizationProvidersResponseData> GetAuthorizationProviders(Configuration config, Device device)
        {
            var url = config.GetEndpointPath(config.OAuth.SystemEndpoints.Authorization);

            var scope = config.DefaultClientId.Scope;

            var urlBuilder = new HttpUrlBuilder(url);
            urlBuilder.Add("client_id", device.ClientId);
            urlBuilder.Add("redirect_uri", "");
            urlBuilder.Add("scope", scope);
            urlBuilder.Add("response_type", "code");
            urlBuilder.Add("display", "social_login");

            var headers = new Dictionary<string, string>
            {
                { HttpHeaders.Accept, HttpContentTypes.Json }
            };

            return await HttpRequester.RequestAsync<AuthorizationProvidersResponseData>(new HttpRequestInfo()
            {
                Method = HttpMethod.Get,
                Url = urlBuilder.ToString(),
                Headers = headers,
            });
        }

        public static async Task<RegisterResponseData> RegisterDeviceAsync(Configuration config, Device device, string csr)
        {
            var url = config.GetEndpointPath(config.Mag.SystemEndpoints.DeviceClientRegister);

            var headers = new Dictionary<string, string>
            {
                { "client-authorization", device.AuthHeaderValue },
                { "device-id", device.Id.ToBase64() },
                { "device-name", device.Name.ToBase64() },
                { "cert-format", "base64" },
                { HttpHeaders.ContentType, HttpContentTypes.Plain },
                { HttpHeaders.Accept, HttpContentTypes.Plain }
            };

            return await HttpRequester.RequestAsync<RegisterResponseData>(new HttpRequestInfo()
            {
                Method = HttpMethod.Post,
                Url = url,
                Headers = headers,
                Body = csr
            });
        }

        public static async Task<RegisterResponseData> RegisterDeviceForUserAsync(Configuration config, Device device, string username, string password, string csr)
        {
            var url = config.GetEndpointPath(config.Mag.SystemEndpoints.DeviceRegister);

            var headers = new Dictionary<string, string>
            {
                { "client-authorization", device.AuthHeaderValue },
                { HttpHeaders.Authorization, string.Format("Basic {0}", string.Format("{0}:{1}", username, password).ToBase64()) },
                { "device-id", device.Id.ToBase64() },
                { "device-name", device.Name.ToBase64() },
                { "cert-format", "base64" },
                { "create-session", "true" },
                { HttpHeaders.ContentType, HttpContentTypes.Plain },
                { HttpHeaders.Accept, HttpContentTypes.Plain }
            };

            return await HttpRequester.RequestAsync<RegisterResponseData>(new HttpRequestInfo()
            {
                Method = HttpMethod.Post,
                Url = url,
                Headers = headers,
                Body = csr
            });
        }

        public static Task<UnregisterResponseData> UnregisterDevice(Configuration config, Device device)
        {
            var url = config.GetEndpointPath(config.Mag.SystemEndpoints.DeviceRemove);

            return HttpRequester.RequestAsync<UnregisterResponseData>(new HttpRequestInfo()
            {
                Method = HttpMethod.Delete,
                Url = url,
                Certificate = device.Certificate
            });
        }

        public static async Task RevokeAccessTokenAsync(Configuration config, Device device, User user)
        {
            var url = config.GetEndpointPath(config.OAuth.SystemEndpoints.TokenRevocation);

            var accessToken = await user.GetAccessTokenAsync();

            HttpUrlBuilder builder = new HttpUrlBuilder(url);
            builder.Add("token", accessToken);
            builder.Add("token_type_hint", "access_token");

            var headers = new Dictionary<string, string>
            {
                { HttpHeaders.Authorization, device.AuthHeaderValue },
                { HttpHeaders.Accept, HttpContentTypes.Json }
            };

            var response = await HttpRequester.RequestTextAsync(new HttpRequestInfo()
            {
                Method = HttpMethod.Delete,
                Headers = headers,
                Url = builder.ToString(),
            });
        }

        public static Task<RequestTokenResponseData> RequestAccessTokenAsync(Configuration config, Device device, string username, string password)
        {
            var url = config.GetEndpointPath(config.OAuth.SystemEndpoints.Token);

            var headers = new Dictionary<string, string>
            {
                { HttpHeaders.Authorization, device.AuthHeaderValue },
                { "mag-identifier", device.MagId },
                { HttpHeaders.ContentType, HttpContentTypes.UrlEncoded },
                { HttpHeaders.Accept, HttpContentTypes.Json }
            };

            var scope = config.DefaultClientId.Scope;

            var builder = new HttpUrlBuilder();
            builder.Add("scope", scope);
            builder.Add("username", username);
            builder.Add("password", password);
            builder.Add("grant_type", "password");
            
            return HttpRequester.RequestAsync<RequestTokenResponseData>(new HttpRequestInfo()
            {
                Method = HttpMethod.Post,
                Url = url,
                Headers = headers,
                Body = builder.ToString()
            });
        }

        public static Task<RequestTokenResponseData> RequestAccessTokenFromIdTokenAsync(Configuration config, Device device, string idToken, string idTokenType)
        {
            var url = config.GetEndpointPath(config.OAuth.SystemEndpoints.Token);
            
            var headers = new Dictionary<string, string>
            {
                { HttpHeaders.Authorization, device.AuthHeaderValue },
                { "mag-identifier", device.MagId },
                { HttpHeaders.ContentType, HttpContentTypes.UrlEncoded },
                { HttpHeaders.Accept, HttpContentTypes.Json }
            };

            var scope = config.DefaultClientId.Scope;

            var builder = new HttpUrlBuilder();
            builder.Add("scope", scope);
            builder.Add("grant_type", idTokenType);
            builder.Add("assertion", idToken);

            return HttpRequester.RequestAsync<RequestTokenResponseData>(new HttpRequestInfo()
            {
                Method = HttpMethod.Post,
                Url = url,
                Headers = headers,
                Body = builder.ToString()
            });
        }

        public static Task<RequestTokenResponseData> RequestAccessTokenAnonymouslyAsync(Configuration config, Device device)
        {
            var url = config.GetEndpointPath(config.OAuth.SystemEndpoints.Token);

            var headers = new Dictionary<string, string>
            {
                { HttpHeaders.Authorization, device.AuthHeaderValue },
                { "mag-identifier", device.MagId },
                { HttpHeaders.ContentType, HttpContentTypes.UrlEncoded },
                { HttpHeaders.Accept, HttpContentTypes.Json }
            };

            var scope = config.DefaultClientId.Scope;

            var builder = new HttpUrlBuilder();
            builder.Add("scope", scope);
            builder.Add("client_id", device.ClientId);
            builder.Add("client_secret", device.ClientSecret);
            builder.Add("grant_type", "client_credentials");

            return HttpRequester.RequestAsync<RequestTokenResponseData>(new HttpRequestInfo()
            {
                Method = HttpMethod.Post,
                Url = url,
                Headers = headers,
                Body = builder.ToString()
            });
        }

        public static Task<RequestTokenResponseData> RefreshAccessTokenAsync(Configuration config, Device device, string refreshToken)
        {
            var url = config.GetEndpointPath(config.OAuth.SystemEndpoints.Token);
            var headers = new Dictionary<string, string>
            {
                { HttpHeaders.Authorization, device.AuthHeaderValue },
                { "mag-identifier", device.MagId },
                { HttpHeaders.ContentType, HttpContentTypes.UrlEncoded },
                { HttpHeaders.Accept, HttpContentTypes.Json }
            };

            var scope = config.DefaultClientId.Scope;

            var builder = new HttpUrlBuilder();
            builder.Add("scope", scope);
            builder.Add("refresh_token", refreshToken);
            builder.Add("grant_type", "refresh_token");

            return HttpRequester.RequestAsync<RequestTokenResponseData>(new HttpRequestInfo()
            {
                Method = HttpMethod.Post,
                Url = url,
                Headers = headers,
                Body = builder.ToString()
            });
        }

        public static async Task<UserInfoResponseData> GetUserInfoAsync(Configuration config, Device device, User user)
        {
            var url = config.GetEndpointPath(config.OAuth.ProjectedEndpoints.UserInfo);

            var accessTokenHeaderValue = await user.GetAccessHeaderValueAsync();

            var headers = new Dictionary<string, string>
            {
                { HttpHeaders.Authorization, accessTokenHeaderValue },
                { "mag-identifier", device.MagId },
                { HttpHeaders.Accept, HttpContentTypes.Json }
            };

            return await HttpRequester.RequestAsync<UserInfoResponseData>(new HttpRequestInfo()
            {
                Url = url,
                Method = HttpMethod.Get,
                Headers = headers
            });
        }
    }
}
