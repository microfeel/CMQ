using System;

namespace MicroFeel.CMQ
{
    public class ServerException : Exception
    {
        public readonly int errorCode = 0;
        public readonly string errorMessage = "";
        public readonly string action = "";

        public ServerException(int errorCode, string errorMessage, string action)
        {
            this.errorCode = errorCode;
            this.errorMessage = errorMessage;
            this.action = action;
        }
        public override string ToString()
        {
            return $"code:{errorCode}, message:{errorMessage}@{action}";
        }
    }
}
