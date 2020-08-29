# CMQ使用注意事项

## Usage

可以参考测试项目中的用法.

```
  //创建cmq账户
  var topicAccount = new CmqAccount("https://cmq-topic-sh.api.qcloud.com", //endpoint
                                     Configuration["QcloudSecret:SecretId"],//secretid
                                     Configuration["QcloudSecret:SecretKey"]);//secretkey

  //之后可以用些账户进行操作
  var Topics = new List<string>();
  var totalCount = await topicAccount.ListTopic("TestTopic", Topics);
  .....
  var Topic = topicAccount.GetTopic("mytopicName");
  topic.publishMessage("a test message");
  ...
```


## 如果在raspberrypi等Linux系统上使用,并且调用接口为https时,注意腾讯云目前使用的TLS协议只支持1.0,在新型linux系统中默认使用TLS1.2进行通讯.

解决此问题的方法是:

    sudo nano /etc/ssl/openssl.cnf

将里面的system_default_sect节改为

    [system_default_sect]
    MinProtocol = TLSv1.0
    CipherString = DEFAULT@SECLEVEL=2

