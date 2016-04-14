using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Cryptography.Certificates;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace MASFoundation.Internal.Http
{    
    internal static class HttpRequester
    {
        public static async Task<HttpTextResponse> RequestTextAsync(HttpRequestInfo requestInfo)
        {
            HttpTextResponse response = new HttpTextResponse();

            HttpClient client;

            if (requestInfo.Certificate != null)
            {
                Session.Instance.Log.Info("Requesting Mutual SSL " + requestInfo.Method + " url " + requestInfo.Url);
                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                filter.ClientCertificate = requestInfo.Certificate;
                filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
                client = new HttpClient(filter);
            }
            else
            {
                Session.Instance.Log.Info("Requesting " + requestInfo.Method + " url " + requestInfo.Url);
                client = new HttpClient();
            }

            HttpRequestMessage httpRequest = new HttpRequestMessage(requestInfo.Method, new Uri(requestInfo.Url));

            if (requestInfo.Method == HttpMethod.Get || requestInfo.Method == HttpMethod.Delete)
            {
                if (requestInfo.Headers != null)
                {
                    foreach (var header in requestInfo.Headers)
                    {
                        httpRequest.Headers.TryAppendWithoutValidation(header.Key, header.Value);
                    }
                }
            }
            else if (requestInfo.Method == HttpMethod.Post || requestInfo.Method == HttpMethod.Put)
            {
                var content = new HttpStringContent(requestInfo.Body ?? string.Empty);

                if (requestInfo.Headers != null)
                {
                    foreach (var header in requestInfo.Headers)
                    {
                        if (header.Key == HttpHeaders.ContentType)
                        {
                            content.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue(header.Value);
                        }
                        else
                        {
                            httpRequest.Headers.TryAppendWithoutValidation(header.Key, header.Value);
                        }
                    }
                }

                httpRequest.Content = content;
            }

            var task = client.SendRequestAsync(httpRequest);
            _activeTasks.Add(task);

            var httpResponse = await task;

            _activeTasks.Remove(task);

            response.StatusCode = (int)httpResponse.StatusCode;

            if (httpResponse.Headers != null)
            {
                response.Headers = new Dictionary<string, string>();

                foreach (var header in httpResponse.Headers)
                {
                    response.Headers[header.Key] = header.Value;
                }
            }

            response.Text = await httpResponse.Content.ReadAsStringAsync();
            response.IsSuccessful = httpResponse.IsSuccessStatusCode;

            LogResponse(requestInfo, response);

            return response;
        }

        public static async Task<T> RequestAsync<T>(HttpRequestInfo requestInfo) where T : HttpResponseBaseData
        {
            var response = await RequestTextAsync(requestInfo);

            return (T)Activator.CreateInstance(typeof(T), response);
        }

        public static void CancelAll()
        {
            _activeTasks.ForEach(a => a.Cancel());
        }

        static List<IAsyncInfo> _activeTasks = new List<IAsyncInfo>();

        static void LogResponse(HttpRequestInfo requestInfo, HttpTextResponse response)
        {
            var sb = new StringBuilder();

            if (!response.IsSuccessful)
            {
                sb.AppendLine("Request Failed! " + requestInfo.Method + " " + requestInfo.Url);
            }
            else
            {
                sb.AppendLine("Request Succeeded! " + requestInfo.Method + " " + requestInfo.Url);
            }

            if (requestInfo.Headers != null)
            {
                foreach (var header in requestInfo.Headers)
                {
                    sb.AppendLine(string.Format("{0}: {1}", header.Key, header.Value));
                }
            }

            if (requestInfo.Body != null)
            {
                sb.AppendLine();
                sb.AppendLine(requestInfo.Body);
            }

            sb.AppendLine();
            sb.AppendLine("Response status " + (int)response.StatusCode);

            if (response.Headers != null)
            {
                foreach (var header in response.Headers)
                {
                    sb.AppendLine(string.Format("{0}: {1}", header.Key, header.Value));
                }
            }

            if (response.Text != null)
            {
                sb.AppendLine();
                sb.AppendLine(response.Text);
            }

            if (!response.IsSuccessful)
            {
                Session.Instance.Log.Error(sb.ToString());
            }
            else
            {
                Session.Instance.Log.Info(sb.ToString());
            }
        }
    }    
}
