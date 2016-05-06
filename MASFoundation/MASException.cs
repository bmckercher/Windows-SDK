using System;

namespace MASFoundation
{
    /// <summary>
    /// MAS Exception
    /// </summary>
    internal class MASException : Exception
    {
        /// <summary>
        /// Constructor for MASException
        /// </summary>
        /// <param name="kind">Defined error code</param>
        /// <param name="message">Additional message</param>
        public MASException(ErrorCode kind, string message) :
            base(message)
        {
            MASErrorCode = kind;
            HResult = ToHResult(kind);
        }

        /// <summary>
        /// Constructor for MASException with inner or nested exception
        /// </summary>
        /// <param name="kind">Defined error code</param>
        /// <param name="message">Additional message</param>
        /// <param name="innerException">Inner or nested exceptio</param>
        public MASException(ErrorCode kind, string message, Exception innerException) :
            base(message, innerException)
        {
            MASErrorCode = kind;
            HResult = ToHResult(kind);
        }

        /// <summary>
        /// MAS error code
        /// </summary>
        public ErrorCode MASErrorCode
        {
            get;
            private set;
        }

        private int ToHResult(ErrorCode code)
        {
            return unchecked((int)0xA0000000 | (int)code);
        }
    
    }
}
