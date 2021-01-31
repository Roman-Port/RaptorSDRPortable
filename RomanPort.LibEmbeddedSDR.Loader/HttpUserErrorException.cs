using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Loader
{
    class HttpUserErrorException : Exception
    {
        public HttpUserErrorException(string message) : base(message)
        {

        }
    }
}
