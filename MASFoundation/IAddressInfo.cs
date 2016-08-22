/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿namespace MASFoundation
{
    /// <summary>
    /// Basic address information of the user
    /// </summary>
    public interface IAddressInfo
    {
        /// <summary>
        /// Their region
        /// </summary>
        string Region { get; }

        /// <summary>
        /// Their country
        /// </summary>
        string Country { get; }
    }
}
