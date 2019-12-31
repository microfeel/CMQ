using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;


namespace MicroFeel.CMQ
{
    internal class CmqClient
    {
        private readonly string CURRENT_VERSION = "SDK_C#_1.0";

        private readonly string secretId;
        private readonly string secretKey;
        private readonly string endpoint;
        private readonly string path;
        private string method;
        /// <summary>
        /// http timeout milseconds
        /// </summary>
        private int timeout;
        private string signMethod;

        public void SetHttpMethod(string value)
        {
            if (value.ToUpper() == "POST" || value.ToUpper() == "GET")
            {
                method = value.ToUpper();
            }
            else
            {
                throw new ClientException("http method only support POST and GET");
            }
        }

        public void SetSignMethod(string value)
        {
            if (value.ToUpper() == "HMACSHA1" || value.ToUpper() == "HMACSHA256")
            {
                signMethod = value.ToUpper();
            }
            else
            {
                throw new ClientException("signMethod only can be HmacSHA1 or HmacSHA256");
            }
        }

        public void SetTimeout(int value)
        {
            timeout = value;
        }

        public CmqClient(string secretId, string secretKey, string endpoint, string path, string method)
        {
            this.secretId = secretId;
            this.secretKey = secretKey;
            this.endpoint = endpoint;
            if (!(endpoint.StartsWith("http://") || endpoint.StartsWith("https://")))
            {
                throw new ClientException("endpoint only support http or https");
            }

            this.path = path;
            this.method = method;
            signMethod = "HMACSHA1";
            timeout = 10000;       //10s
        }

        public async Task<string> Call(string action, SortedDictionary<string, string> param)
        {
            Random ran = new Random();
            int nonce = ran.Next(int.MaxValue);
            param.Add("Action", action);
            param.Add("Nonce", Convert.ToString(new Random().Next(int.MaxValue)));
            param.Add("SecretId", secretId);
            int timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            param.Add("Timestamp", Convert.ToString(timestamp));
            param.Add("RequestClient", CURRENT_VERSION);
            if (signMethod.ToUpper() == "HMACSHA1")
            {
                param.Add("SignatureMethod", "HmacSHA1");
            }
            else
            {
                param.Add("SignatureMethod", "HmacSHA256");
            }

            string host = "";
            if (endpoint.StartsWith("https"))
            {
                host = endpoint.Substring(8);
            }
            else
            {
                host = endpoint.Substring(7);
            }

            string src = method + host + path + "?";

            src += string.Join("&", param.OrderBy(p => p.Key, StringComparer.Ordinal).Select(p => $"{p.Key.Replace("_", ".")}={p.Value}"));

            param.Add("Signature", Sign.Signature(src, secretKey, signMethod));

            string url = endpoint + path;
            string req = "";
            if (method.ToUpper() == "GET")
            {
                url += "?" + string.Join("&", param.Select(p => $"{p.Key}={HttpUtility.UrlEncode(p.Value)}"));

                if (url.Length > 2048)
                {
                    throw new ClientException("URL length is larger than 2K when use the GET method ");
                }
            }
            else
            {
                req += string.Join("&", param.Select(p => $"{p.Key}={HttpUtility.UrlEncode(p.Value)}"));
            }

            using (var httpreq = new HttpRequestMessage(new HttpMethod(method), url))
            using (var httpClient = new HttpClient())
            {
                httpreq.Content = new StringContent(req);
                httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
                var rspMessage = await httpClient.SendAsync(httpreq);
                return await rspMessage.Content.ReadAsStringAsync();
            }
        }
    }
};
