using System;

namespace MASFoundation.Internal
{
    /// <summary>
    /// MAS Exception - this is marked as internal because of a limitation with Windows Runtime component:
    /// no public custom exceptions.  However we want an internal exception so we can set the HResult 
    /// code.  HResult code is exposed as number in javascript exception.
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
            HResult = kind.ToHResult();
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
            HResult = kind.ToHResult();
        }

        /// <summary>
        /// MAS error code
        /// </summary>
        public ErrorCode MASErrorCode
        {
            get;
            private set;
        }
    }
}
