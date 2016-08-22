/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿namespace MASFoundation
{
    /// <summary>
    /// Logging level
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Do not log any errors
        /// </summary>
        None = 0,

        /// <summary>
        /// Log everything
        /// </summary>
        Full,

        /// <summary>
        /// Log warnings and errors only
        /// </summary>
        ErrorOnly
    }
}
