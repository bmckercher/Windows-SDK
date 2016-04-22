using System.Diagnostics;

namespace MASFoundation.Internal
{
    internal class Logger : ILogger
    {
        public void Info(string message)
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

        public void Warn(string message)
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

        public void Error(string message)
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
