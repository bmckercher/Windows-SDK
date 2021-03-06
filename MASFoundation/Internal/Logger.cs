/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using System.Diagnostics;

namespace MASFoundation.Internal
{
    internal static class Logger
    {
        static internal void LogInfo(string message)
        {
            if (MAS.LogLevel == LogLevel.Full)
            {
                MAS.RaiseLogMessage("Info: " + message);
            }
        }

        static internal void LogWarning(string message)
        {
            if (MAS.LogLevel == LogLevel.Full || MAS.LogLevel == LogLevel.ErrorOnly)
            {
                MAS.RaiseLogMessage("Warn: " + message);
            }
        }

        static internal void LogError(string message)
        {
            if (MAS.LogLevel == LogLevel.Full || MAS.LogLevel == LogLevel.ErrorOnly)
            {
                MAS.RaiseLogMessage("Error: " + message);
            }
        }
    }
}
