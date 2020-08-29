using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroFeel.CMQ
{
    public class Subscription
    {
        private readonly string topicName;
        private readonly string subscriptionName;
        private readonly CmqClient client;
        internal Subscription(string topicName, string subscriptionName, CmqClient client)
        {
            this.topicName = topicName;
            this.subscriptionName = subscriptionName;
            this.client = client;
        }
        public async Task ClearFilterTags()
        {
            var param = new SortedDictionary<string, string>
            {
                { "topicName", topicName },
                { "subscriptionName", subscriptionName }
            };

            await client.Call("ClearSUbscriptionFIlterTags", param);
        }

        public async Task SetSubscriptionAttributes(SubscriptionMeta meta)
        {
            var param = new SortedDictionary<string, string>
            {
                { "topicName", topicName },
                { "subscriptionName", subscriptionName }
            };
            if (meta.notifyStrategy != "")
            {
                param.Add("notifyStrategy", meta.notifyStrategy);
            }

            if (meta.notifyContentFormat != "")
            {
                param.Add("notifyContentFormat", meta.notifyContentFormat);
            }

            if (meta.filterTag != null)
            {
                for (int i = 0; i < meta.filterTag.Count; ++i)
                {
                    param.Add("filterTag." + Convert.ToString(i), meta.filterTag[i]);
                }
            }
            if (meta.bindingKey != null)
            {
                for (int i = 0; i < meta.bindingKey.Count; ++i)
                {
                    param.Add("bindingKey." + Convert.ToString(i), meta.bindingKey[i]);
                }
            }

            await client.Call("SetSubscriptionAttributes", param);
        }

        public async Task<SubscriptionMeta> GetSubscriptionAttributes()
        {
            var param = new SortedDictionary<string, string>
            {
                { "topicName", topicName },
                { "subscriptionName", subscriptionName }
            };

            string result = await client.Call("getSubscriptionAttributes", param);
            JObject jObj = JObject.Parse(result);

            SubscriptionMeta meta = new SubscriptionMeta
            {
                filterTag = new List<string>(),
                bindingKey = new List<string>(),

                endpoint = (string)jObj["endpoint"],
                notifyStrategy = (string)jObj["notifyStrategy"],
                notifyContentFormat = (string)jObj["notifyContentFormat"],
                protocal = (string)jObj["protocol"],
                createTime = (int)jObj["createTime"],
                lastModifyTime = (int)jObj["lastModifyTime"],
                msgCount = (int)jObj["msgCount"]
            };

            JArray filterTagArray = JArray.Parse(jObj["filterTag"].ToString());
            foreach (var item in filterTagArray)
            {
                meta.filterTag.Add(item.ToString());
            }

            JArray bindingKeyArray = JArray.Parse(jObj["bindingKey"].ToString());
            foreach (var item in bindingKeyArray)
            {
                meta.bindingKey.Add(item.ToString());
            }

            return meta;

        }
    }
}
