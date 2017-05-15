# SuperSocketClient.U3D
Integration SuperSocket.ClientEngine to Unity3D.   
Make some samples.  
Passed on Unity3D 5.4.2f2  

## Reference (Directory - Library with Address)
* ProtoBase - [SuperSocket.ProtoBase v1.7.0.15](https://github.com/kerryjiang/SuperSocket.ProtoBase)  
`Commit:5fcc925ca72b67a528ad1fd72611f9c37effb214`
* ClientEngine - [SuperSocket.ClientEngine v0.8.10](https://github.com/kerryjiang/SuperSocket.ClientEngine)  
`Commit: d16adc1779c079589346363f9ce596ca973e706c`
* crypto - [The Bouncy Castle C# Cryptographic ](https://github.com/bcgit/bc-csharp)  
`Commit: 44c1a7c05913560362d3d241dbd0bc58a5b6dbac`

## Usage
* Checkout the repository.
* Copy directory "SuperSocket" to Unity3D "Assets" directory.
* Add "NO_SPINWAIT_CLASSS" to scripting defines symbols in PlayerSettings, for Unity3D dose not support C# 6 features.
* In SuperSocketClientTest.cs ， modify the target address to your own server address
```C#
            mClient.BeginConnect(new DnsEndPoint("172.17.99.3", 10009));
```
* Create a new scene, and a GameObject with component "SuperSocketClientTest" attached.
* Run, there will be a *Button* on the top-left of the screen, press to test TCP/IP connection.
* In this case, a FixedHeaderReceiveFilter was defined. The first 4 bytes in binary steam indicate the length of the whole data package.