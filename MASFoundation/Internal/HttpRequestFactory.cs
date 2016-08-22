/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MASFoundation.Internal.Http;

namespace MASFoundation.Internal
{
    internal static class HttpRequestFactory
    {
        static HttpRequestFactory()
        {
            _tokenSource = new CancellationTokenSource();

            _normalRequestImpl = new NormalHttpRequester();
            _mutualRequestImpl = new MutualHttpsRequester();
        }

        public static Task<HttpTextResponse> RequestTextAsync(HttpRequestInfo requestInfo)
        {
            if (requestInfo.Certificate != null)
            {
                return _mutualRequestImpl.RequestTextAsync(requestInfo);
            }
            else
            {
                return _normalRequestImpl.RequestTextAsync(requestInfo);
            }
        }

        public static async Task<T> RequestAsync<T>(HttpRequestInfo requestInfo) where T : HttpResponseBaseData
        {
            var response = await RequestTextAsync(requestInfo);

            return (T)Activator.CreateInstance(typeof(T), response);
        }

        public static void CancelAll()
        {
            _normalRequestImpl.CancelAll();
            _mutualRequestImpl.CancelAll();
        }

        static CancellationTokenSource _tokenSource;

        static HttpRequesterBase _normalRequestImpl;
        static HttpRequesterBase _mutualRequestImpl;
    }    
}
