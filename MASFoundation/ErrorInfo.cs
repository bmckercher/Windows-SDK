namespace MASFoundation
{
    /// <summary>
    /// Contains error information
    /// </summary>
    public class ErrorInfo
    {
        internal ErrorInfo(ErrorCode code, string text)
        {
            Code = code;
            Text = text;
        }

        /// <summary>
        /// MAS error code
        /// </summary>
        public ErrorCode Code
        {
            get;
            private set;
        }

        /// <summary>
        /// Text of error
        /// </summary>
        public string Text
        {
            get;
            private set;
        }
    }
}
