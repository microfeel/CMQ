using MicroFeel.CMQ;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace QCloud.Test
{
    public class TestBase
    {
        protected CmqAccount queueAccount;
        protected CmqAccount topicAccount;

        public TestBase()
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<TestBase>();

            var Configuration = builder.Build();
            topicAccount = new CmqAccount("https://cmq-topic-sh.api.qcloud.com",
                                     Configuration["QcloudSecret:SecretId"],
                                     Configuration["QcloudSecret:SecretKey"]);
            queueAccount = new CmqAccount("https://cmq-queue-sh.api.qcloud.com",
                          Configuration["QcloudSecret:SecretId"],
                          Configuration["QcloudSecret:SecretKey"]);
        }
    }
}
