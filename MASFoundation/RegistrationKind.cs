/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿namespace MASFoundation
{
    /// <summary>
    /// Device registration flow
    /// </summary>
    public enum RegistrationKind
    {
        /// <summary>
        /// Device registration without User Authentication
        /// </summary>
        Client,

        /// <summary>
        /// Device Registration with User Authentication
        /// </summary>
        User
    }
}
