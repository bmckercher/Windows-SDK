using System;

namespace MASFoundation
{
    public class MASException : Exception
    {
        public MASException(ErrorCode kind, string message) :
            base(message)
        {
            MASErrorCode = kind;
        }

        public MASException(ErrorCode kind, string message, Exception innerException) :
            base(message, innerException)
        {
            MASErrorCode = kind;
        }

        public ErrorCode MASErrorCode
        {
            get;
            private set;
        }
    
    }
}
