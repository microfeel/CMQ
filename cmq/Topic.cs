using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroFeel.CMQ
{
    public class Topic
    {
        private readonly string topicName;
        private readonly CmqClient client;
        internal Topic(string topicName, CmqClient client)
        {
            this.topicName = topicName;
            this.client = client;
        }

        public async Task SetTopicAttributes(int maxMsgSize)
        {
            var param = new SortedDictionary<string, string>
            {
                { "topicName", topicName }
            };
            if (maxMsgSize < 0 || maxMsgSize > 65536)
            {
                throw new ClientException("Invalid parameter maxMsgSize < 0 or maxMsgSize > 65536");
            }

            param.Add("maxMsgSize", Convert.ToString(maxMsgSize));

            await client.Call("SetTopicAttributes", param);
        }


        public async Task<TopicMeta> GetTopicAttributes()
        {
            var param = new SortedDictionary<string, string>
            {
                { "topicName", topicName }
            };

            string result = await client.Call("GetTopicAttributes", param);
            JObject jObj = JObject.Parse(result);

            var meta = new TopicMeta
            {
                msgCount = (int)jObj["msgCount"],
                maxMsgSize = (int)jObj["maxMsgSize"],
                msgRetentionSeconds = (int)jObj["msgRetentionSeconds"],
                createTime = (int)jObj["createTime"],
                lastModifyTime = (int)jObj["lastModifyTime"],
                filterType = (int)jObj["filterType"]
            };
            return meta;
        }

        public async Task<string> PublishMessage(string msgBody)
        {
            return await PublishMessage(msgBody, new List<string>(), "");
        }

        public async Task<string> PublishMessage(string msgBody, string routingKey)
        {
            return await PublishMessage(msgBody, new List<string>(), routingKey);
        }

        public async Task<string> PublishMessage(string msgBody, IList<string> tagList, string routingKey)
        {
            var param = new SortedDictionary<string, string>
            {
                { "topicName", topicName },
                { "msgBody", msgBody }
            };
            if (!string.IsNullOrEmpty(routingKey))
            {
                param.Add("routingKey", routingKey);
            }

            if (tagList != null)
            {
                for (int i = 0; i < tagList.Count; i++)
                {
                    string k = $"msgTag.{i + 1}";
                    param.Add(k, tagList[i]);
                }
            }
            string result = await client.Call("PublishMessage", param);
            JObject jObj = JObject.Parse(result);

            return jObj["msgId"].ToString();

        }

        public async Task<IList<string>> BatchPublishMessage(IList<string> vtMsgBody)
        {
            return await BatchPublishMessage(vtMsgBody, new List<string>(), "");
        }

        public async Task<IList<string>> BatchPublishMessage(IList<string> vtMsgBody, string routingKey)
        {
            return await BatchPublishMessage(vtMsgBody, new List<string>(), routingKey);
        }

        public async Task<IList<string>> BatchPublishMessage(IList<string> vMsgBody, IList<string> vTagList, string routingKey)
        {
            var param = new SortedDictionary<string, string>
            {
                { "topicName", topicName }
            };

            if (!string.IsNullOrEmpty(routingKey))
            {
                param.Add("routingKey", routingKey);
            }

            if (vMsgBody != null)
            {
                for (int i = 0; i < vMsgBody.Count; i++)
                {
                    string k = $"msgBody.{i + 1}";
                    param.Add(k, vMsgBody[i]);
                }
            }
            if (vTagList != null)
            {
                for (int i = 0; i < vTagList.Count; i++)
                {
                    string k = $"msgTag{i + 1}";
                    param.Add(k, vTagList[i]);
                }
            }

            string result = await client.Call("BatchPublishMessage", param);

            var jObj = JObject.Parse(result);

            var vMsgId = new List<string>();
            var idsArray = JArray.Parse(jObj["msgList"].ToString());
            foreach (var item in idsArray)
            {
                vMsgId.Add(item["msgId"].ToString());
            }
            return vMsgId;
        }

        public async Task<int> ListSubscription(int offset, int limit, string searchWord, IList<string> subscriptionList)
        {
            var param = new SortedDictionary<string, string>
            {
                { "topicName", topicName }
            };
            if (searchWord != "")
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

            string result = await client.Call("ListSubscriptionByTopic", param);

            JObject jObj = JObject.Parse(result);

            int totalCount = (int)jObj["totalCount"];
            if (jObj["subscriptionList"] != null)
            {
                //List<string> vMsgId = new List<string>();
                JArray idsArray = JArray.Parse(jObj["subscriptionList"].ToString());
                foreach (var item in idsArray)
                {
                    subscriptionList.Add(item["subscriptionName"].ToString());
                }
            }

            return totalCount;
        }


    }
}
