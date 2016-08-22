/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System.Collections.Generic;
using System.Net;
using System.Text;
using Windows.Security.Cryptography;
using System.Linq;

namespace MASFoundation.Internal.Http
{
    internal class HttpUrlBuilder
    {
        string _baseUrl;

        public HttpUrlBuilder()
        {
        }

        public HttpUrlBuilder(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public void Add(string name, string value)
        {
            _parameters.Add(new KeyValuePair<string, string>(name, WebUtility.UrlEncode(value)));
        }

        public void AddNonce()
        {
            var buffer = CryptographicBuffer.GenerateRandom(10);
            string randomHex = CryptographicBuffer.EncodeToHexString(buffer);

            Add("nonce", randomHex);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (_baseUrl != null)
            {
                sb.Append(_baseUrl);
            }

            if (_parameters.Count > 0)
            {
                if (_baseUrl != null)
                {
                    sb.Append("?");
                }

                var sorted = _parameters.OrderBy(pair => pair.Key);
                foreach (var pair in sorted)
                {
                    sb.AppendFormat("{0}={1}&", pair.Key, pair.Value);
                }

                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        List<KeyValuePair<string, string>> _parameters = new List<KeyValuePair<string, string>>();
    }
}
