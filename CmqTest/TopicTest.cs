using MicroFeel.CMQ;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QCloud.Test
{
    [TestClass]
    public class TopicTest : TestBase
    {
        private string GetRandomTopicName => $"TestTopic{new Random(DateTime.Now.Millisecond).Next(10000):D5}";


        [TestMethod]
        public async Task CreateTopic()
        {
            var TopicName = GetRandomTopicName;
            await topicAccount.CreateTopic(TopicName, 1024 * 1024);
            var Topics = new List<string>();
            var totalCount = await topicAccount.ListTopic(TopicName, Topics);
            Console.WriteLine($"totalCount:{totalCount}");
            foreach (var q in Topics)
            {
                Console.WriteLine(q);
            }
            Assert.IsTrue(Topics.Count == 1);
        }

        [TestMethod]
        public async Task DeleteTopic()
        {
            try
            {

                Console.WriteLine("删除之前=============================");
                var Topics = new List<string>();
                var totalCount = await topicAccount.ListTopic("TestTopic", Topics);
                Console.WriteLine($"totalCount:{totalCount}");
                for (int i = 0; i < Topics.Count(); i++)
                {
                    var TopicName = Topics.ElementAt(i);
                    Console.WriteLine(TopicName);
                    await topicAccount.DeleteTopic(TopicName);
                }
                Console.WriteLine("删除之后=============================");
                Topics.Clear();
                totalCount = await topicAccount.ListTopic("TestTopic", Topics);
                Console.WriteLine($"totalCount:{totalCount}");
                foreach (var q in Topics)
                {
                    Console.WriteLine(q);
                }
                Assert.IsTrue(Topics.Count() == 0);
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
        public async Task ListTopicsAsync()
        {
            var Topics = new List<string>();
            var totalCount = await topicAccount.ListTopic("", Topics);
            Console.WriteLine($"totalCount:{totalCount}");
            Assert.IsTrue(Topics.Count() > 0);
            for (int i = 0; i < Topics.Count(); i++)
            {
                var TopicName = Topics.ElementAt(i);
                Console.WriteLine(TopicName);
            }
        }

        [TestMethod]
        public async Task PublishMessages()
        {
            try
            {
                var Topic = topicAccount.GetTopic("DaShu-K3");
                //var message = "{\"FItemId\":0,\"Name\":\"产品部2\",\"Number\":\"0004\",\"ShortNumber\":null,\"PhoneNumber\":null,\"FaxNumber\":null,\"ParentNumber\":null,\"IsWorkShop\":false}";
                var message = File.ReadAllText(".\\msg.json");
                var ss = await Topic.PublishMessage(message, new List<string> { "模式凭证列表" }, "");
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
        public async Task BatchPublishMessages()
        {
            try
            {
                var Topic = topicAccount.GetTopic("DaShu-K3");
                var message = File.ReadAllText(".\\msg.json");
                var ss = await Topic.BatchPublishMessage(new List<string> { message }, new List<string> { "模式凭证列表" }, "");
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
        public void DeleteMessages()
        {

        }
    }
}
