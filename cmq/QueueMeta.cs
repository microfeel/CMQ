namespace MicroFeel.CMQ
{
    public class QueueMeta
    {
        /// <summary>
        /// 缺省消息接收长轮询等待时间
        /// </summary>
        public static readonly int DEFAULT_POLLING_WAIT_SECONDS = 0;
        /// <summary>
        /// 缺省消息可见性超时
        /// </summary>
        public static readonly int DEFAULT_VISIBILITY_TIMEOUT = 30;
        /// <summary>
        /// 缺省消息最大长度，单位字节
        /// </summary>
        public static readonly int DEFAULT_MAX_MSG_SIZE = 65536;
        /// <summary>
        /// 缺省消息保留周期，单位秒
        /// </summary>
        public static readonly int DEFAULT_MSG_RETENTION_SECONDS = 345600;

        /// <summary>
        /// 最大堆积消息数 
        /// </summary>
        public int maxMsgHeapNum = -1;
        /// <summary>
        /// 消息接收长轮询等待时间
        /// </summary>
        public int pollingWaitSeconds = DEFAULT_POLLING_WAIT_SECONDS;
        /// <summary>
        /// 消息可见性超时
        /// </summary>
        public int visibilityTimeout = DEFAULT_VISIBILITY_TIMEOUT;
        /// <summary>
        /// 消息最大长度
        /// </summary>
        public int maxMsgSize = DEFAULT_MAX_MSG_SIZE;
        /// <summary>
        /// 消息保留周期
        /// </summary>
        public int msgRetentionSeconds = DEFAULT_MSG_RETENTION_SECONDS;
        /// <summary>
        /// 队列创建时间
        /// </summary>
        public int createTime = -1;
        /// <summary>
        /// 队列属性最后修改时间
        /// </summary>
        public int lastModifyTime = -1;
        /// <summary>
        /// 队列处于Active状态的消息总数
        /// </summary>
        public int activeMsgNum = -1;
        /// <summary>
        /// 队列处于Inactive状态的消息总数
        /// </summary>
        public int inactiveMsgNum = -1;
        /// <summary>
        /// 已删除的消息，但还在回溯保留时间内的消息数量
        /// </summary>
        public int rewindmsgNum;
        /// <summary>
        /// 消息最小未消费时间
        /// </summary>
        public int minMsgTime;
        /// <summary>
        /// 延时消息数量
        /// </summary>
        public int delayMsgNum;
    }
}
