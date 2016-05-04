namespace MASFoundation
{
    /// <summary>
    /// Interface for logging
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log informational message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Info(string message);

        /// <summary>
        /// Log warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Warn(string message);

        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Error(string message);
    }
}
