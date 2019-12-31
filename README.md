# CMQ使用注意事项

## 如果在raspberrypi等Linux系统上使用,并且调用接口为https时,注意腾讯云目前使用的TLS协议只支持1.0,在新型linux系统中默认使用TLS1.2进行通讯.

解决此问题的方法是:

    sudo nano /etc/ssl/openssl.cnf

将里面的system_default_sect节改为

    [system_default_sect]
    MinProtocol = TLSv1.0
    CipherString = DEFAULT@SECLEVEL=2
