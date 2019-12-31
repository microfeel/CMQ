using System;

namespace MicroFeel.CMQ
{
    public class ServerException : Exception
    {
        private readonly int httpStatus = 200;
        private readonly int errorCode = 0;
        private readonly string errorMessage = "";
        private readonly string requestId = "";

        public ServerException(int httpStatus) { this.httpStatus = httpStatus; }
        public ServerException(int errorCode, string errorMessage, string requestId)
        {
            this.errorCode = errorCode;
            this.errorMessage = errorMessage;
            this.requestId = requestId;
        }
        public override string ToString()
        {
            if (httpStatus != 200)
            {
                return "http status: " + httpStatus;
            }
            else
            {
                return "code:" + errorCode
                    + ", message:" + errorMessage
                    + ", requestId" + requestId;
            }
        }
    }
}
