using System;
using System.Collections.Generic;

namespace MASFoundation
{
    /// <summary>
    /// MAS configuration exception
    /// </summary>
    public class MASConfigException : Exception
    {
        /// <summary>
        /// Constructor for MAS configuration exception
        /// </summary>
        public MASConfigException()
        {
            Errors = new List<string>();
        }

        /// <summary>
        /// List of detected errors in configuration
        /// </summary>
        public List<string> Errors { get; private set; }
    }
}
