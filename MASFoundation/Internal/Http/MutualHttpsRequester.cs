using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace MASFoundation.Internal.Http
{
    internal class MutualHttpsRequester : HttpRequesterBase
    {
        public override async Task<HttpTextResponse> RequestTextAsync(HttpRequestInfo requestInfo)
        {
            HttpTextResponse response = new HttpTextResponse();

            Session.Instance.Log.Info("Requesting Mutual SSL " + requestInfo.Method + " url " + requestInfo.Url);
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            filter.ClientCertificate = requestInfo.Certificate;
            HttpClient client = new HttpClient(filter);

            HttpRequestMessage httpRequest = new HttpRequestMessage(ToRequestMethod(requestInfo.Method), new Uri(requestInfo.Url));

            if (requestInfo.Method == HttpMethod.GET || requestInfo.Method == HttpMethod.DELETE)
            {
                if (requestInfo.Headers != null)
                {
                    foreach (var header in requestInfo.Headers)
                    {
                        httpRequest.Headers.TryAppendWithoutValidation(header.Key, header.Value);
                    }
                }
            }
            else if (requestInfo.Method == HttpMethod.POST || requestInfo.Method == HttpMethod.PUT)
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

        public override void CancelAll()
        {
            _activeTasks.ForEach(a => a.Cancel());
        }

        Windows.Web.Http.HttpMethod ToRequestMethod(HttpMethod method)
        {
            switch (method)
            {
                case HttpMethod.GET:
                    return Windows.Web.Http.HttpMethod.Get;
                case HttpMethod.DELETE:
                    return Windows.Web.Http.HttpMethod.Delete;
                case HttpMethod.POST:
                    return Windows.Web.Http.HttpMethod.Post;
                case HttpMethod.PUT:
                    return Windows.Web.Http.HttpMethod.Put;
                default:
                    return Windows.Web.Http.HttpMethod.Get;
            }
        }

        List<IAsyncInfo> _activeTasks = new List<IAsyncInfo>();
    }
}
