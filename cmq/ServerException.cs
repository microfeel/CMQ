using Newtonsoft.Json.Linq;
using System;

namespace MicroFeel.CMQ
{
    public class ServerException : Exception
    {
        private readonly string result;
        public readonly int errorCode = 0;
        public readonly string errorMessage = "";
        public readonly string action = "";

        public ServerException(string serverResult, string action)
        {
            this.result = serverResult;
            var jObj = JObject.Parse(result);
            this.errorCode = (int)jObj["code"];
            this.errorMessage = (string)jObj["message"];
            this.action = action;
        }
        public override string ToString()
        {
            var requestid = "";
            var codeDesc = "";
            var jObj = JObject.Parse(result);
            if (jObj["requestId"] != null)
            {
                requestid = $",requestid:{jObj["requestId"]}";
            }
            if (jObj["codeDesc"] != null)
            {
                requestid = $",codeDesc:{jObj["codeDesc"]}";
            }

            return $"code:{errorCode}, message:{errorMessage}@{action}{requestid}{codeDesc}";
        }
    }
}
