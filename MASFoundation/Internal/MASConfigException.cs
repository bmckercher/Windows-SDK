/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System;
using System.Collections.Generic;

namespace MASFoundation.Internal
{
    /// <summary>
    /// MAS configuration exception
    /// </summary>
    internal class MASConfigException : Exception
    {
        /// <summary>
        /// Constructor for MAS configuration exception
        /// </summary>
        public MASConfigException()
        {
            Errors = new List<string>();
        }

        /// <summary>
        /// List of detected errors in configuration
        /// </summary>
        public List<string> Errors { get; private set; }
    }
}
