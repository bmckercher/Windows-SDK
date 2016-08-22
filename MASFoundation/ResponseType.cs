/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿namespace MASFoundation
{
    /// <summary>
    /// Expected HTTP response type
    /// </summary>
    public enum ResponseType
    {
        /// <summary>
        /// Unknown or don't care
        /// </summary>
        Unknown,

        /// <summary>
        /// JSON format
        /// </summary>
        Json,

        /// <summary>
        /// SCIM JSON format
        /// </summary>
        ScimJson,

        /// <summary>
        /// Plain text format
        /// </summary>
        PlainText,

        /// <summary>
        /// XML format
        /// </summary>
        Xml
    }
}
