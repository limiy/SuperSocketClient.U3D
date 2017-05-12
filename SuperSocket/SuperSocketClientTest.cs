using System;
using UnityEngine;
using SuperSocket.ProtoBase;
using SuperSocket.ClientEngine;
using System.Text;
using System.Collections;
using System.Net;
using System.Linq;

public class SuperSocketClientTest : MonoBehaviour {

    class MyBinaryPackageInfo : IPackageInfo
    {
        public MyBinaryPackageInfo()
        {

        }
        public MyBinaryPackageInfo(IBufferStream buffStream)
        {
            var buffers = buffStream.Buffers;
            if (buffers.Count < 1)
            {
                Debug.Log("Data delivery error! count < 2");
            }
            var header = buffers[0];
            var body = buffers[1];
            int offset = 0;
            this.Type = BitConverter.ToUInt16(body.Array, body.Offset + offset + 0);
            this.Id = BitConverter.ToUInt32(body.Array, body.Offset + offset + 2);
            this.Body = body.Array.CloneRange(body.Offset + offset + 6, body.Count - 6 - offset);
        }
        public UInt16 Type { get; set; }
        public UInt32 Id { get; set; }
        public byte[] Body { get; set; }

        public byte[] ToBinary()
        {
            int headerSize = 4;
            int typeSize = 2;
            int idSize = 4;
            int bodySize = this.Body == null ? 0 : Body.Length;
            UInt32 totalLength = (UInt32)(headerSize + typeSize + idSize + bodySize);

            var mm = new System.IO.MemoryStream();
            var bytes = BitConverter.GetBytes(totalLength);
            mm.Write(bytes, 0 , bytes.Length);
            bytes = BitConverter.GetBytes(Type);
            mm.Write(bytes, 0, bytes.Length);
            bytes = BitConverter.GetBytes(Id);
            mm.Write(bytes, 0, bytes.Length);
            bytes = Body;
            if(bytes != null)
                mm.Write(bytes, 0, bytes.Length);

            bytes = mm.ToArray();
            return bytes;
        }
    }

    class MyReceiveFilter : FixedHeaderReceiveFilter<MyBinaryPackageInfo>
    {
        public MyReceiveFilter()
            : base(4)
        {

        }

        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            return bufferStream.ReadInt32(true) - length;
        }

        public override MyBinaryPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            return new MyBinaryPackageInfo(bufferStream);
        }
    }

    EasyClient mClient = null;
    EasyClient mHttpClient = null;

    // Use this for initialization
    void Start () {
    }

    private void OnGUI()
    {
        if (GUILayout.Button("TCP Connect test"))
        {
            Debug.Log("Before network init at " + Time.realtimeSinceStartup);

            if (mClient == null)
            {
                mClient = new EasyClient();
                //mClient.Security.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Default;
                mClient.Closed += MClient_Closed;
                mClient.Connected += MClient_Connected;
                mClient.Error += MClient_Error;
            }
            // Initialize the client with the receive filter and request handler
            mClient.Initialize(new MyReceiveFilter(), MClient_Received);
            mClient.BeginConnect(new DnsEndPoint("172.17.99.3", 10009));

            Debug.Log("After network init at " + Time.realtimeSinceStartup);
        }
    }
    private void MClient_Error(object sender, ErrorEventArgs e)
    {
        Debug.Log("Connect error." + e.Exception.Message);
    }

    private void MClient_Connected(object sender, EventArgs e)
    {
        Debug.Log("Connected.");
    }

    private void MClient_Closed(object sender, EventArgs e)
    {
        Debug.Log("Connection closed.");
    }

    private void MClient_Received(MyBinaryPackageInfo respone)
    {
        // handle the received request
        Debug.Log(string.Format("Type:{0}, Id:{1:X4}, BodyLength:{2}", respone.Type, respone.Id, respone.Body.Length));
        if (respone.Type == 1001)
        {
            Debug.Log("服务器应答。准备发送测试消息");
            MyBinaryPackageInfo request = new MyBinaryPackageInfo();
            request.Type = 1006;
            request.Id = 0;
            request.Body = null;
            mClient.Send(request.ToBinary());
            Debug.Log("测试消息 - 发送请求");
        }
        if (respone.Type == 1007)
        {
            Debug.Log("测试消息 - 收到响应");
        }
    }
    // Update is called once per frame
    void Update () {
	
	}

    private void OnDestroy()
    {
        if( mClient != null )
        {
            mClient.Close();
            mClient = null;
        }
    }
}
