using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroFeel.CMQ
{
    public class CmqAccount
    {
        private readonly CmqClient client;
        /// <summary>
        /// 默认分页大小
        /// </summary>
        private const int _defaultPageLimit = 20;
        /// <summary>
        /// 默认最大分页容量
        /// </summary>
        private const int _pageLimit = 50;

        public CmqAccount(string endpoint, string secretId, string secretKey)
        {
            client = new CmqClient(secretId, secretKey, endpoint, "/v2/index.php", "POST");
        }

        public void SetSignMethod(string signMethod)
        {
            client.SetSignMethod(signMethod);
        }
        public void SetHttpMethod(string method)
        {
            client.SetHttpMethod(method);
        }
        public void SetTimeout(int timeout)
        {
            //timeout is milseconds for the http request
            client.SetTimeout(timeout);
        }

        public async Task<int> ListQueue(string searchWord, List<string> queueList, int offset = 0, int limit = _defaultPageLimit)
        {
            if (limit > _pageLimit)
            {
                throw new ArgumentOutOfRangeException($"每页最多{_pageLimit}条记录!");
            }
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();
            if (!searchWord.Equals(""))
            {
                param.Add("searchWord", searchWord);
            }

            if (offset >= 0)
            {
                param.Add("offset", Convert.ToString(offset));
            }

            if (limit > 0)
            {
                param.Add("limit", Convert.ToString(limit));
            }

            string result =await client.Call("ListQueue", param);

            JObject jObj = JObject.Parse(result);
            int code = (int)jObj["code"];
            if (code != 0)
            {
                throw new ServerException(code, jObj["message"].ToString(), jObj["requestId"].ToString());
            }

            int totalCount = (int)jObj["totalCount"];
            JArray queueListArray = JArray.Parse(jObj["queueList"].ToString());

            //return queueListArray.Select(v => v["queueName"].ToString());
            foreach (var item in queueListArray)
            {
                queueList.Add(item["queueName"].ToString());
            }
            return totalCount;
        }

        public async Task CreateQueue(string queueName, QueueMeta meta)
        {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();
            if (queueName == "")
            {
                throw new ClientException("Invalid parameter: queueName is empty");
            }
            else
            {
                param.Add("queueName", queueName);
            }

            if (meta.maxMsgHeapNum > 0)
            {
                param.Add("maxMsgHeapNum", Convert.ToString(meta.maxMsgHeapNum));
            }

            if (meta.pollingWaitSeconds > 0)
            {
                param.Add("pollingWaitSeconds", Convert.ToString(meta.pollingWaitSeconds));
            }

            if (meta.visibilityTimeout > 0)
            {
                param.Add("visibilityTimeout", Convert.ToString(meta.visibilityTimeout));
            }

            if (meta.maxMsgSize > 0)
            {
                param.Add("maxMsgSize", Convert.ToString(meta.maxMsgSize));
            }

            if (meta.msgRetentionSeconds > 0)
            {
                param.Add("msgRetentionSeconds", Convert.ToString(meta.msgRetentionSeconds));
            }

            string result =await client.Call("CreateQueue", param);
            JObject jObj = JObject.Parse(result);
            int code = (int)jObj["code"];
            if (code != 0)
            {
                throw new ServerException(code, jObj["message"].ToString(), jObj["requestId"].ToString());
            }
        }

        public async Task DeleteQueue(string queueName)
        {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();
            if (queueName == "")
            {
                throw new ClientException("Invalid parameter: queueName is empyt");
            }
            else
            {
                param.Add("queueName", queueName);
            }

            string result =await client.Call("DeleteQueue", param);
            JObject jObj = JObject.Parse(result);
            int code = (int)jObj["code"];
            if (code != 0)
            {
                throw new ServerException(code, jObj["message"].ToString(), jObj["requestId"].ToString());
            }
        }

        public Queue GetQueue(string queueName)
        {
            return new Queue(queueName, client);
        }

        public async Task<int> ListTopic(string searchWord, List<string> topicList, int offset = 0, int limit = _defaultPageLimit)
        {
            if (limit > _pageLimit)
            {
                throw new ArgumentOutOfRangeException($"每页最多{_pageLimit}条记录!");
            }
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();
            if (!searchWord.Equals(""))
            {
                param.Add("searchWord", searchWord);
            }

            if (offset >= 0)
            {
                param.Add("offset", Convert.ToString(offset));
            }

            if (limit > 0)
            {
                param.Add("limit", Convert.ToString(limit));
            }

            string result =await client.Call("ListTopic", param);

            JObject jObj = JObject.Parse(result);
            int code = (int)jObj["code"];
            if (code != 0)
            {
                throw new ServerException(code, jObj["message"].ToString(), jObj["requestId"].ToString());
            }

            int totalCount = (int)jObj["totalCount"];
            JArray topicListArray = JArray.Parse(jObj["topicList"].ToString());
            foreach (var item in topicListArray)
            {
                topicList.Add(item["topicName"].ToString());
            }
            return totalCount;
        }

        public Topic GetTopic(string topicName)
        {
            return new Topic(topicName, client);
        }

        public async Task CreateTopic(string topicName, int maxMsgSize)
        {
            await CreateTopic(topicName, maxMsgSize, 1);
        }

        public async Task CreateTopic(string topicName, int maxMsgSize, int filterType = 1)
        {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();
            if (topicName == "")
            {
                throw new ClientException("Invalid parameter: topicName is empty");
            }
            else
            {
                param.Add("topicName", topicName);
            }

            param.Add("filterType", Convert.ToString(filterType));
            if (maxMsgSize < 1 || maxMsgSize > 65536)
            {
                throw new ClientException("Invalid paramter: maxMsgSize > 65536 or maxMsgSize < 1 ");
            }

            string result =await client.Call("CreateTopic", param);
            JObject jObj = JObject.Parse(result);
            int code = (int)jObj["code"];
            if (code != 0)
            {
                throw new ServerException(code, jObj["message"].ToString(), jObj["requestId"].ToString());
            }
        }

        public async Task DeleteTopic(string topicName)
        {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();
            if (topicName == "")
            {
                throw new ClientException("Invalid parameter: topicName is empty");
            }
            else
            {
                param.Add("topicName", topicName);
            }

            string result =await client.Call("DeleteTopic", param);
            JObject jObj = JObject.Parse(result);
            int code = (int)jObj["code"];
            if (code != 0)
            {
                throw new ServerException(code, jObj["message"].ToString(), jObj["requestId"].ToString());
            }
        }

        public async Task CreateSubscribe(string topicName, string subscriptionName, string endpoint, string protocol)
        {
            await CreateSubscribe(topicName, subscriptionName, endpoint, protocol, null, null, "BACKOFF_RETRY", "JSON");
        }

        public async Task CreateSubscribe(string topicName, string subscriptionName, string endpoint, string protocol,
            List<string> filterTag, List<string> bindingKey, string notifyStrategy, string notifyContentFormat)
        {
            if (filterTag != null && filterTag.Count > 5)
            {
                throw new ClientException("Invalid parameter:Tag number > 5");
            }

            SortedDictionary<string, string> param = new SortedDictionary<string, string>();
            if (topicName == "")
            {
                throw new ClientException("Invalid parameter: topicName is empty");
            }
            else
            {
                param.Add("topicName", topicName);
            }

            if (subscriptionName == "")
            {
                throw new ClientException("Invalid parameter: subscriptionName is empty");
            }
            else
            {
                param.Add("subscriptionName", subscriptionName);
            }

            if (endpoint == "")
            {
                throw new ClientException("Invalid parameter: endpoint is empty");
            }
            else
            {
                param.Add("endpoint", endpoint);
            }

            if (protocol == "")
            {
                throw new ClientException("Invalid parameter: protocol is empty");
            }
            else
            {
                param.Add("protocol", protocol);
            }

            if (notifyStrategy == "")
            {
                throw new ClientException("Invalid parameter: notifyStrategy is empty");
            }
            else
            {
                param.Add("notifyStrategy", notifyStrategy);
            }

            if (notifyContentFormat == "")
            {
                throw new ClientException("Invalid parameter: notifyContentFormat is empty");
            }
            else
            {
                param.Add("notifyContentFormat", notifyContentFormat);
            }

            if (filterTag != null)
            {
                for (int i = 0; i < filterTag.Count; ++i)
                {
                    param.Add("filterTag." + Convert.ToString(i), filterTag[i]);
                }
            }

            if (bindingKey != null)
            {
                for (int i = 0; i < bindingKey.Count; ++i)
                {
                    param.Add("bindingKey." + Convert.ToString(i), bindingKey[i]);
                }
            }

            string result =await client.Call("Subscribe", param);
            JObject jObj = JObject.Parse(result);
            int code = (int)jObj["code"];
            if (code != 0)
            {
                throw new ServerException(code, jObj["message"].ToString(), jObj["requestId"].ToString());
            }
        }
        public async Task DeleteSubscribe(string topicName, string subscriptionName)
        {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();
            if (topicName == "")
            {
                throw new ClientException("Invalid parameter: topicName is empty");
            }
            else
            {
                param.Add("topicName", topicName);
            }

            if (subscriptionName == "")
            {
                throw new ClientException("Invalid parameter: subscriptionName is empty");
            }
            else
            {
                param.Add("subscriptionName", subscriptionName);
            }

            string result =await client.Call("Unsubscribe", param);
            JObject jObj = JObject.Parse(result);
            int code = (int)jObj["code"];
            if (code != 0)
            {
                throw new ServerException(code, jObj["message"].ToString(), jObj["requestId"].ToString());
            }
        }

        public Subscription GetSubscribe(string topicName, string subscriptionName)
        {
            return new Subscription(topicName, subscriptionName, client);
        }

    }

}