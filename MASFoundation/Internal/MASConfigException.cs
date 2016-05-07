using System;
using System.Collections.Generic;

namespace MASFoundation.Internal
{
    /// <summary>
    /// MAS configuration exception
    /// </summary>
    internal class MASConfigException : Exception
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
