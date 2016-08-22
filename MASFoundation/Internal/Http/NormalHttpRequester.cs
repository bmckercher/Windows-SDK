/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MASFoundation.Internal.Http
{
    internal class NormalHttpRequester : HttpRequesterBase
    {
        public override async Task<HttpTextResponse> RequestTextAsync(HttpRequestInfo requestInfo)
        {
            HttpTextResponse response = new HttpTextResponse();

            HttpClient client;

            Logger.LogInfo("Requesting " + requestInfo.Method + " url " + requestInfo.Url);

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Automatic;

            client = new HttpClient(handler);

            HttpRequestMessage httpRequest = new HttpRequestMessage(ToRequestMethod(requestInfo.Method), new Uri(requestInfo.Url));

            if (requestInfo.Method == HttpMethod.GET || requestInfo.Method == HttpMethod.DELETE)
            {
                if (requestInfo.Headers != null)
                {
                    foreach (var header in requestInfo.Headers)
                    {
                        httpRequest.Headers.Add(header.Key, header.Value);
                    }
                }
            }
            else if (requestInfo.Method == HttpMethod.POST || requestInfo.Method == HttpMethod.PUT)
            {
                var content = new ByteArrayContent((requestInfo.Body ?? string.Empty).ToBytes());

                if (requestInfo.Headers != null)
                {
                    foreach (var header in requestInfo.Headers)
                    {
                        if (header.Key == HttpHeaders.ContentType)
                        {
                            content.Headers.ContentType = new MediaTypeHeaderValue(header.Value);
                        }
                        else
                        {
                            httpRequest.Headers.Add(header.Key, header.Value);
                        }
                    }
                }

                httpRequest.Content = content;
            }

            var httpResponse = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, _tokenSource.Token);
            response.StatusCode = (int)httpResponse.StatusCode;

            if (httpResponse.Headers != null)
            {
                response.Headers = new Dictionary<string, string>();

                foreach (var header in httpResponse.Headers)
                {
                    response.Headers[header.Key] = String.Join("; ", header.Value);
                }
            }

            response.Text = await httpResponse.Content.ReadAsStringAsync();
            response.IsSuccessful = httpResponse.IsSuccessStatusCode;

            LogResponse(requestInfo, response);

            return response;
        }

        public override void CancelAll()
        {
            _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
        }

        System.Net.Http.HttpMethod ToRequestMethod(HttpMethod method)
        {
            switch (method)
            {
                case HttpMethod.GET:
                    return System.Net.Http.HttpMethod.Get;
                case HttpMethod.DELETE:
                    return System.Net.Http.HttpMethod.Delete;
                case HttpMethod.POST:
                    return System.Net.Http.HttpMethod.Post;
                case HttpMethod.PUT:
                    return System.Net.Http.HttpMethod.Put;
                case HttpMethod.PATCH:
                    return new System.Net.Http.HttpMethod("PATCH");
                default:
                    return System.Net.Http.HttpMethod.Get;
            }
        }

        CancellationTokenSource _tokenSource = new CancellationTokenSource();
    }
}
