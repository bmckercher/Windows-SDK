using System;
using System.Collections.Generic;
using System.Text;

namespace MASFoundation
{
    public class MAGConfigException : Exception
    {
        public MAGConfigException()
        {
            Errors = new List<string>();
        }

        public List<string> Errors { get; private set; }
    }
}
