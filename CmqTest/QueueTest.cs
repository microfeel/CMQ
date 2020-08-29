using MicroFeel.CMQ;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QCloud.Test
{
    [TestClass]
    public class QueueTest: TestBase
    {
        private string GetRandomQueueName => $"TestQueue{new Random(DateTime.Now.Millisecond).Next(10000):D5}";

        [TestMethod]
        public async Task CreateQueue()
        {
            var queueName = GetRandomQueueName;
            await queueAccount.CreateQueue(queueName, new QueueMeta());
            var queues = new List<string>();
            var totalCount = await queueAccount.ListQueue(queueName, queues);
            Console.WriteLine($"totalCount:{totalCount}");
            foreach (var q in queues)
            {
                Console.WriteLine(q);
            }
            Assert.IsTrue(queues.Count() >= 1);
        }

        [TestMethod]
        public async Task DeleteQueue()
        {
            try
            {

                Console.WriteLine("删除之前=============================");
                var queues = new List<string>();
                var totalCount = await queueAccount.ListQueue("TestQueue", queues);
                Console.WriteLine($"totalCount:{totalCount}");
                for (int i = 0; i < queues.Count(); i++)
                {
                    var queueName = queues.ElementAt(i);
                    Console.WriteLine(queueName);
                    await queueAccount.DeleteQueue(queueName);
                }
                Console.WriteLine("删除之后=============================");
                queues.Clear();
                totalCount = await queueAccount.ListQueue("TestQueue", queues);
                Console.WriteLine($"totalCount:{totalCount}");
                foreach (var q in queues)
                {
                    Console.WriteLine(q);
                }
                Assert.IsTrue(queues.Count() == 0);
            }
            catch (ServerException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public async Task ListQueuesAsync()
        {
            try
            {

                for (int j = 0; j < 3; j++)
                {
                    var queues = new List<string>();
                    var totalCount = await queueAccount.ListQueue("", queues);
                    Console.WriteLine($"totalCount:{totalCount}");
                    Assert.IsTrue(queues.Count() > 0);
                    for (int i = 0; i < queues.Count(); i++)
                    {
                        var queueName = queues.ElementAt(i);
                        Console.WriteLine(queueName);
                    }
                }
            }
            catch (ServerException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch
            {
                throw;
            }
        }

        [TestMethod]
        public async Task SendMessages()
        {
            try
            {
                var queue = queueAccount.GetQueue("DaShu-K3-subQueue");
                //var message = "{\"FItemId\":0,\"Name\":\"产品部2\",\"Number\":\"0004\",\"ShortNumber\":null,\"PhoneNumber\":null,\"FaxNumber\":null,\"ParentNumber\":null,\"IsWorkShop\":false}";
                var message = File.ReadAllText(".\\msg.json");
                var ss = await queue.SendMessage(message);
                Console.WriteLine(ss);
            }
            catch (ServerException ex)
            {
                Console.WriteLine(ex.errorMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public async Task ReceiveMessages()
        {
            var queue = queueAccount.GetQueue("TestQueue01806");
            var list = new List<string>();
            var ss = await queue.ReceiveMessage(10);
            Console.WriteLine($"id:{ss.msgId},handle:{ss.receiptHandle},content:{ss.msgBody}");
            var h = ss.receiptHandle;
            queue = queueAccount.GetQueue("TestQueue01806");
            await queue.DeleteMessage(ss.receiptHandle);
        }

        [TestMethod]
        public void DeleteMessages()
        {

        }
    }
}
