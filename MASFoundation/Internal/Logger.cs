using System.Diagnostics;

namespace MASFoundation.Internal
{
    internal class Logger
    {
        public void Info(string message)
        {
            Debug.WriteLine(message, "Info");
        }

        public void Warn(string message)
        {
            Debug.WriteLine(message, "Warning");
        }

        public void Error(string message)
        {
            Debug.WriteLine(message, "Error");
        }
    }
}
