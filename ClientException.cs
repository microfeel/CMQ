using System;
using System.Collections.Generic;
using System.Text;

namespace MicroFeel.CMQ
{
    public class ClientException : Exception
    {
        public ClientException(string message) : base(message) { }
    }
}
