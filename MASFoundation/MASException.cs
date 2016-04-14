using System;

namespace MASFoundation
{
    public class MASException : Exception
    {
        public MASException(ErrorKind kind, string message) :
            base(message)
        {
            MASErrorCode = (int)kind;
        }

        public MASException(ErrorKind kind, string message, Exception innerException) :
            base(message, innerException)
        {
            MASErrorCode = (int)kind;
        }

        public int MASErrorCode
        {
            get;
            private set;
        }
    
    }
}
