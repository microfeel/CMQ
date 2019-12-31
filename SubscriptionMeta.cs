using System.Collections.Generic;
namespace MicroFeel.CMQ
{
    public class SubscriptionMeta
    {
        public static readonly string notifyStrategyDefault = "BACKOFF_RETRY";
        public static readonly string notifyContentFormatDefault = "JSON";

        //订阅的终端地址
        public string endpoint;
        //订阅的协议
        public string protocal;
        //推送消息出现错误时的重试策略
        public string notifyStrategy;
        //向 Endpoint 推送的消息内容格式
        public string notifyContentFormat;
        //描述了该订阅中消息过滤的标签列表（仅标签一致的消息才会被推送）
        public List<string> filterTag;
        //Subscription 的创建时间，从 1970-1-1 00:00:00 到现在的秒值
        public int createTime;
        //修改 Subscription 属性信息最近时间，从 1970-1-1 00:00:00 到现在的秒值
        public int lastModifyTime;
        //该订阅待投递的消息数
        public int msgCount;
        public List<string> bindingKey;


        /**
         * subscription meta class .
         *
         */
        public SubscriptionMeta()
        {
            endpoint = "";
            protocal = "";
            notifyStrategy = notifyStrategyDefault;
            notifyContentFormat = notifyContentFormatDefault;
            filterTag = null;
            createTime = 0;
            lastModifyTime = 0;
            msgCount = 0;
            bindingKey = null;
        }
    }
}
