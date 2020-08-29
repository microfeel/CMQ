using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace MicroFeel.CMQ
{
    internal class CmqClient
    {
        private readonly string CURRENT_VERSION = "SDK_C#_1.0";

        private readonly string secretId;
        private readonly string secretKey;
        private readonly string path;
        private readonly string endpoint;
        private string method = "POST";
        /// <summary>
        /// http timeout milseconds
        /// </summary>
        private int timeout;
        private string signMethod;
        //private readonly HttpClient httpClient;

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
                signMethod = value;
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

        public CmqClient(string secretId, string secretKey, string endpoint, string path/*, HttpClient client*/, string httpMethod = "POST")
        {
            this.secretId = secretId;
            this.secretKey = secretKey;
            this.endpoint = endpoint;
            method = httpMethod;
            if (!(endpoint.StartsWith("http://") || endpoint.StartsWith("https://")))
            {
                throw new ClientException("endpoint only support http or https");
            }

            this.path = path;
            //if (client == null)
            //{
            //    throw new ClientException("httpclient不能为空.");
            //}
            //this.httpClient = client;
            signMethod = Sign.HMACSHA256;
            timeout = 10000;       //10s
        }

        public async Task<string> Call(string action, SortedDictionary<string, string> param)
        {
            //添加公共参数
            Random ran = new Random();
            int nonce = ran.Next(int.MaxValue);
            param.Add("Action", action);
            param.Add("Nonce", Convert.ToString(new Random().Next(int.MaxValue)));
            param.Add("SecretId", secretId);
            int timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            param.Add("Timestamp", Convert.ToString(timestamp));
            param.Add("RequestClient", CURRENT_VERSION);
            param.Add("SignatureMethod", signMethod);
            //拼接签名串
            string host = new Uri(endpoint).Host;

            string src = $"{method}{host}{path}?";

            src += string.Join("&", param.OrderBy(p => p.Key, StringComparer.Ordinal).Select(p => $"{p.Key.Replace("_", ".")}={p.Value}"));
            //签名
            param.Add("Signature", Sign.Signature(src, secretKey, signMethod));

            var url = endpoint + path;
            var req = string.Join("&", param.Select(p => $"{p.Key}={HttpUtility.UrlEncode(p.Value)}")); ;
            if (method.ToUpper() == "GET")
            {
                url += $"?{req}";

                if (url.Length > 2048)
                {
                    throw new ClientException("URL length is larger than 2K when use the GET method ");
                }
            }

            using (var httpreq = new HttpRequestMessage(new HttpMethod(method), url))
            using (var httpClient = new HttpClient())
            {
                httpreq.Content = new StringContent(req);
                httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
                var rspMessage = await httpClient.SendAsync(httpreq);
                var result = await rspMessage.Content.ReadAsStringAsync();
                var jObj = JObject.Parse(result);
                int code = (int)jObj["code"];
                if (code != 0 && code != 7000)
                {
                    throw new ServerException(code, jObj["message"].ToString(), action);
                }
                return result;
            }
        }

        /// <summary>
        /// Post提交数据
        /// </summary>
        /// <param name="postUrl">URL</param>
        /// <param name="paramData">参数</param>
        /// <returns></returns>
        private string PostWebRequest(string postUrl, string paramData)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化 /
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ret;
        }


    }
};
