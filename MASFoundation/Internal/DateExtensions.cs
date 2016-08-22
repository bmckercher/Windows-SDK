/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System;

namespace MASFoundation.Internal
{
    internal static class DateExtensions
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixTime(this long seconds)
        {
            return Epoch.AddSeconds(seconds);
        }

        public static long ToUnixTime(this DateTime date)
        {
            return Convert.ToInt64((date - Epoch).TotalSeconds);
        }
    }
}
