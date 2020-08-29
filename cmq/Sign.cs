using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace MicroFeel.CMQ
{
    /// <summary>
    /// 签名类
    /// </summary>
    public class Sign
    {
        public static string HMACSHA256= "HmacSHA256";
        public static string HMACSHA1= "HmacSHA1";

        ///<summary>生成签名</summary>
        ///<param name="signStr">被加密串</param>
        ///<param name="secret">加密密钥</param>
        ///<returns>签名</returns>
        public static string Signature(string signStr, string secret, string SignatureMethod)
        {
            if (SignatureMethod.ToUpper() == "HMACSHA256")
                using (var mac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
                {
                    var hash = mac.ComputeHash(Encoding.UTF8.GetBytes(signStr));
                    return Convert.ToBase64String(hash);
                }
            else
                using (var mac = new HMACSHA1(Encoding.UTF8.GetBytes(secret)))
                {
                    var hash = mac.ComputeHash(Encoding.UTF8.GetBytes(signStr));
                    return Convert.ToBase64String(hash);
                }
        }

        protected static string BuildParamStr(SortedDictionary<string, object> requestParams, string requestMethod = "GET")
        {
            string retStr = "";
            foreach (string key in requestParams.Keys)
            {
                if (key == "Signature")
                {
                    continue;
                }
                if (requestMethod == "POST" && requestParams[key].ToString().Substring(0, 1).Equals("@"))
                {
                    continue;
                }
                retStr += string.Format("{0}={1}&", key.Replace("_", "."), requestParams[key]);
            }
            return "?" + retStr.TrimEnd('&');
        }

        public static string MakeSignPlainText(SortedDictionary<string, object> requestParams, string requestMethod = "GET", 
            string requestHost = "cvm.api.qcloud.com", string requestPath = "/v2/index.php")
        {
            string retStr = "";
            retStr += requestMethod;
            retStr += requestHost;
            retStr += requestPath;
            retStr += BuildParamStr(requestParams);
            return retStr;
        }
    }
}
