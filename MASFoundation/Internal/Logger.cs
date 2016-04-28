﻿using System.Diagnostics;

namespace MASFoundation.Internal
{
    internal static class Logger
    {
        static internal void LogInfo(string message)
        {
            if (MAS.LogLevel == LogLevel.Full)
            {
                if (MAS.Logger != null)
                {
                    MAS.Logger.Info(message);
                }
                else
                {
                    Debug.WriteLine(message, "Info");
                }
            }
        }

        static internal void LogWarning(string message)
        {
            if (MAS.LogLevel == LogLevel.Full || MAS.LogLevel == LogLevel.ErrorOnly)
            {
                if (MAS.Logger != null)
                {
                    MAS.Logger.Warn(message);
                }
                else
                {
                    Debug.WriteLine(message, "Warning");
                }
            }
        }

        static internal void LogError(string message)
        {
            if (MAS.LogLevel == LogLevel.Full || MAS.LogLevel == LogLevel.ErrorOnly)
            {
                if (MAS.Logger != null)
                {
                    MAS.Logger.Error(message);
                }
                else
                {
                    Debug.WriteLine(message, "Error");
                }
            }
        }
    }
}
