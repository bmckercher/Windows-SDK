/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System;
using System.Text;

namespace MASFoundation.Internal
{
    internal static class StringExtensions
    {
        public static string ToBase64(this string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }

        public static string FromBase64(this string text)
        {
            var converted = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(converted);
        }

        public static byte[] ToBytes(this string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }
    }
}
