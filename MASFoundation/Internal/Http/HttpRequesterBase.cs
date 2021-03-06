/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MASFoundation.Internal.Http
{
    internal abstract class HttpRequesterBase
    {
        public abstract Task<HttpTextResponse> RequestTextAsync(HttpRequestInfo requestInfo);

        public abstract void CancelAll();

        protected void LogResponse(HttpRequestInfo requestInfo, HttpTextResponse response)
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
                Logger.LogError(sb.ToString());
            }
            else
            {
                Logger.LogInfo(sb.ToString());
            }
        }
    }
}
