using MicroFeel.CMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;
using System.Security.Authentication.ExtendedProtection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace QCloud.Test
{
    public class TestBase
    {
        protected readonly ServiceProvider serviceProvider;
        public IConfiguration Configuration { get; }

        public TestBase()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<TestBase>();
            Configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddHttpClient("queue", (q) =>
            {
                q.BaseAddress = new Uri("https://cmq-queue-sh.api.qcloud.com");
            });
            services.AddHttpClient("topic", (t) =>
            {
                t.BaseAddress = new Uri("https://cmq-topic-sh.api.qcloud.com");
            });
            serviceProvider = services.BuildServiceProvider();
        }

    }
}
