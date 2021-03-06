﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


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
        private string signMethod;
        private static HttpClient httpClient;

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

        public CmqClient(string secretId, string secretKey, string endpoint, string path, HttpClient client, string httpMethod = "POST")
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
            httpClient = client;
            signMethod = Sign.HMACSHA256;
        }
        //转义字符编码
        private string EncodeString(string str)
        {
            //maxLengthAllowed .NET < 4.5 = 32765;
            //maxLengthAllowed .NET >= 4.5 = 65519;
            int maxLengthAllowed = 65519;
            StringBuilder sb = new StringBuilder();
            int loops = str.Length / maxLengthAllowed;

            for (int i = 0; i <= loops; i++)
            {
                sb.Append(Uri.EscapeDataString(i < loops
                    ? str.Substring(maxLengthAllowed * i, maxLengthAllowed)
                    : str.Substring(maxLengthAllowed * i)));
            }

            return sb.ToString();
        }
        /// <summary>
        /// 调用腾讯云
        /// </summary>
        /// <param name="action"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<string> Call(string action, SortedDictionary<string, string> param)
        {
            //添加公共参数
            Random ran = new Random();
            int nonce = ran.Next(int.MaxValue);
            param.Add("Action", action);
            param.Add("Nonce", new Random().Next(int.MaxValue).ToString());
            param.Add("SecretId", secretId);
            int timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            param.Add("Timestamp", timestamp.ToString());
            param.Add("RequestClient", CURRENT_VERSION);
            param.Add("SignatureMethod", signMethod);
            //拼接签名串
            string host = new Uri(endpoint).Host;

            string src = $"{method}{host}{path}?";

            src += string.Join("&", param.OrderBy(p => p.Key, StringComparer.Ordinal).Select(p => $"{p.Key.Replace("_", ".")}={p.Value}"));
            //签名
            param.Add("Signature", Sign.Signature(src, secretKey, signMethod));

            var url = endpoint + path;
            var req = string.Join("&", param.Select(p => $"{p.Key}={EncodeString(p.Value)}")); ;
            if (method.ToUpper() == "GET")
            {
                url += $"?{req}";

                if (url.Length > 2048)
                {
                    throw new ClientException("URL length is larger than 2K when use the GET method ");
                }
            }

            //为兼容老程序，未注入的httpclient将被手工构造
            if (httpClient == null)
            {
                httpClient = new HttpClient();
            }

            using (var httpreq = new HttpRequestMessage(new HttpMethod(method), url))
            {
                httpreq.Content = new StringContent(req, Encoding.UTF8, "text/json");
                var rspMessage = await httpClient.SendAsync(httpreq);
                var result = await rspMessage.Content.ReadAsStringAsync();
                var jObj = JObject.Parse(result);
                int code = (int)jObj["code"];
                if (code != 0 && code != 7000)
                {
                    throw new ServerException(result, action);
                }
                return result;
            }
        }
        ~CmqClient()
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
                httpClient = null;
            }
        }
    }
};
