/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System;
using System.Diagnostics;

namespace MASFoundation.Internal
{
    internal static class ErrorFactory
    {
        [DebuggerStepThrough] // We don't want Visual Studio to break debugger here just because we threw an exception
        public static void ThrowError(ErrorCode error, Exception innerException = null)
        {
            throw new MASException(error, error.ToErrorString(innerException), innerException);
        }
    }
}
