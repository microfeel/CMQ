using System;
using System.Collections.Generic;
using System.Text;

namespace MicroFeel.CMQ
{
    /// <summary>
    /// 消息队列设置
    /// </summary>
    public class CMQSettings
    {
        /// <summary>
        /// Appid
        /// </summary>
        public string SecretId { get; set; }
        /// <summary>
        /// AppKey
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// 队列地址
        /// </summary>
        public string QueueEndpoint { get; set; }
        /// <summary>
        /// 主题订阅地址
        /// </summary>
        public string TopicEndpoint { get; set; }
    }
}
