using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroFeel.CMQ
{
    public class Queue
    {
        private string queueName;
        private CmqClient client;
        internal Queue(string queueName, CmqClient client)
        {
            this.queueName = queueName;
            this.client = client;
        }

        public async Task SetQueueAttributes(QueueMeta meta)
        {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>
            {
                { "queueName", queueName }
            };
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

            await client.Call("SetQueueAttributes", param);
        }

        public async Task<QueueMeta> GetQueueAttributes()
        {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>
            {
                { "queueName", queueName }
            };

            string result = await client.Call("GetQueueAttributes", param);
            JObject jObj = JObject.Parse(result);

            return new QueueMeta
            {
                maxMsgHeapNum = (int)jObj["maxMsgHeapNum"],
                pollingWaitSeconds = (int)jObj["pollingWaitSeconds"],
                visibilityTimeout = (int)jObj["visibilityTimeout"],
                maxMsgSize = (int)jObj["maxMsgSize"],
                msgRetentionSeconds = (int)jObj["msgRetentionSeconds"],
                createTime = (int)jObj["createTime"],
                lastModifyTime = (int)jObj["lastModifyTime"],
                activeMsgNum = (int)jObj["activeMsgNum"],
                inactiveMsgNum = (int)jObj["inactiveMsgNum"],
                rewindmsgNum = (int)jObj["rewindMsgNum"],
                minMsgTime = (int)jObj["minMsgTime"],
                delayMsgNum = (int)jObj["delayMsgNum"]
            };
        }

        public async Task<string> SendMessage(string msgBody)
        {
            return await SendMessage(msgBody, 0);
        }

        public async Task<string> SendMessage(string msgBody, int delayTime)
        {
            var param = new SortedDictionary<string, string>
            {
                { "queueName", queueName },
                { "msgBody", msgBody },
                { "delaySeconds", Convert.ToString(delayTime) }
            };

            string result = await client.Call("SendMessage", param);
            JObject jObj = JObject.Parse(result);
            return jObj["msgId"].ToString();
        }

        public async Task<List<string>> BatchSendMessage(List<string> vtMsgBody, int delayTime)
        {
            if (vtMsgBody.Count == 0 || vtMsgBody.Count > 16)
            {
                throw new ClientException("Error: message size is empty or more than 16");
            }

            SortedDictionary<string, string> param = new SortedDictionary<string, string>
            {
                { "queueName", queueName }
            };
            for (int i = 0; i < vtMsgBody.Count; i++)
            {
                string k = "msgBody." + Convert.ToString(i + 1);
                param.Add(k, vtMsgBody[i]);
            }
            param.Add("delaySeconds", Convert.ToString(delayTime));

            string result = await client.Call("BatchSendMessage", param);

            JObject jObj = JObject.Parse(result);

            List<string> vtMsgId = new List<string>();
            JArray idsArray = JArray.Parse(jObj["msgList"].ToString());
            foreach (var item in idsArray)
            {
                vtMsgId.Add(item["msgId"].ToString());
            }
            return vtMsgId;

        }

        public async Task<Message> ReceiveMessage(int pollingWaitSeconds)
        {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>
            {
                { "queueName", queueName }
            };
            if (pollingWaitSeconds > 0)
            {
                //param.Add("UserpollingWaitSeconds", Convert.ToString(pollingWaitSeconds));
                param.Add("pollingWaitSeconds", Convert.ToString(pollingWaitSeconds));
            }
            //else {
            //    param.Add("UserpollingWaitSeconds", Convert.ToString(30000));
            //}
            client.SetTimeout(Convert.ToInt32(TimeSpan.FromMinutes(pollingWaitSeconds + 5).TotalMilliseconds));
            string result = await client.Call("ReceiveMessage", param);
            JObject jObj = JObject.Parse(result);
            int code = (int)jObj["code"];
            if (code == 7000)
            {
                return null;
            }

            return new Message
            {
                msgId = jObj["msgId"].ToString(),
                receiptHandle = jObj["receiptHandle"].ToString(),
                msgBody = jObj["msgBody"].ToString(),
                enqueueTime = (long)jObj["enqueueTime"],
                nextVisibleTime = (long)jObj["nextVisibleTime"],
                firstDequeueTime = (long)jObj["firstDequeueTime"],
                dequeueCount = (int)jObj["dequeueCount"]
            };
        }

        public async Task<List<Message>> BatchReceiveMessage(int numOfMsg, int pollingWaitSeconds)
        {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>
            {
                { "queueName", queueName },
                { "numOfMsg", Convert.ToString(numOfMsg) }
            };
            if (pollingWaitSeconds > 0)
            {
                param.Add("UserpollingWaitSeconds", Convert.ToString(pollingWaitSeconds));
                param.Add("pollingWaitSeconds", Convert.ToString(pollingWaitSeconds));
            }
            else
            {
                param.Add("UserpollingWaitSeconds", Convert.ToString(30000));
            }
            string result = await client.Call("BatchReceiveMessage", param);
            JObject jObj = JObject.Parse(result);

            List<Message> vtMsg = new List<Message>();
            JArray idsArray = JArray.Parse(jObj["msgInfoList"].ToString());
            foreach (var item in idsArray)
            {
                Message msg = new Message
                {
                    msgId = item["msgId"].ToString(),
                    receiptHandle = item["receiptHandle"].ToString(),
                    msgBody = item["msgBody"].ToString(),
                    enqueueTime = (long)item["enqueueTime"],
                    nextVisibleTime = (long)item["nextVisibleTime"],
                    firstDequeueTime = (long)item["firstDequeueTime"],
                    dequeueCount = (int)item["dequeueCount"]
                };
                vtMsg.Add(msg);
            }
            return vtMsg;
        }

        public async Task DeleteMessage(string receiptHandle)
        {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>
            {
                { "queueName", queueName },
                { "receiptHandle", receiptHandle }
            };

            await client.Call("DeleteMessage", param);
        }

        public async Task BatchDeleteMessage(List<string> vtReceiptHandle)
        {
            if (vtReceiptHandle.Count == 0)
            {
                return;
            }

            SortedDictionary<string, string> param = new SortedDictionary<string, string>
            {
                { "queueName", queueName }
            };
            for (int i = 0; i < vtReceiptHandle.Count; i++)
            {
                string k = "receiptHandle." + Convert.ToString(i + 1);
                param.Add(k, vtReceiptHandle[i]);
            }

            await client.Call("BatchDeleteMessage", param);
        }
    }
}
