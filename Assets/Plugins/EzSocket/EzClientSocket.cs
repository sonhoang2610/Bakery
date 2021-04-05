using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EazyEngine.Networking
{

 
        public delegate void OnEventOpen(IClient client);
        public delegate void OnEventClose(IClient client, ushort code, string reason);
        public delegate void OnEventMessage(IClient client, EzMessage msg);
        public delegate void OnEventError(IClient client, string error);
        public delegate void OnEventReceiveBinaryData(IClient client, byte[] datas);
        public static class EzClient
        {
            public static IClient instance;
            public static OnEventOpen OnOpen { set; get; }
            public static OnEventClose OnClose { set; get; }
            public static OnEventMessage OnMessage { set; get; }
            public static OnEventError OnError { set; get; }
            public static OnEventReceiveBinaryData OnEventReceiveBinaryData { set; get; }
        }

        public interface IClient
        {

            void connect();
            void Send(EzMessage msg);
            void Close();
            OnEventOpen onOpen { set; get; }
            OnEventClose onClose { set; get; }
            OnEventMessage onMessage { set; get; }
            OnEventError onError { set; get; }
        }
    
    static class SocketExtensions
    {
        public static bool IsConnected(this Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }
    }


    public class EzClientSocket : IDisposable ,IClient
    {
        public bool isHttp = false;
        private  Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private HttpListener _clientHttp;
        private static byte[] buffer = new byte[4096];
        string _uri;
        int _port;
        public EzClientSocket(string uri, int port,bool http)
        {
            isHttp = http;
            _uri = uri;
            _port = port;
            if(!http)
            {
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                var url = string.Format("ws://{0}:{1}", _uri,_port);
                 websock = new QHWebSocket(url);
                // _clientHttp = new HttpListener();
                // var portURI = string.Format("http://{0}:{1}/", uri, port);
                // _clientHttp.Prefixes.Add(portURI);

            }
         
            EzClient.OnEventReceiveBinaryData = (client, datas) =>
            {
                onReceiveByteData(datas,datas.Length);
            };
        }
        //public void onCloseCallBack(IAsyncResult rs)
        //{
     
        //    Socket client = (Socket)rs.AsyncState;
        //    onError.Invoke(this,"disconect");
        //    onClose.Invoke(this, 0, "");
        //}
            public void connect()
        {
            //Thread pThread = new Thread(createcConnect);
            //pThread.Start();
            createcConnect();
        }
        public static System.Action<byte[]> customSendHTTP;
        public void Send(EzMessage pMsg)
        {
            pMsg.endCode();
            if (!isHttp)
            {
              
                _clientSocket.Send(pMsg.ToArray());
            }
            else
            {
                websock.Send(pMsg.ToArray());
            }
        
        }
     
        public void onCustonReceiveCallBack(byte[] bf)
        {
            EzMessage pMsg = new EzMessage(currentBuffer.ToArray());
            pMsg.deCode();
            onMessage(this, pMsg);
        }

        public void onReceiveByteData(byte[] bufforigin, int received)
        {
            while (received > 0)
            {
                if (onError != null)
                {
                    onError.Invoke(this, "received begin:" + received );
                }
                byte[] dataBuff = new byte[received];
                Array.Copy(bufforigin, dataBuff, received);
                int header = 0;
                do
                {
                    if (!isFilling)
                    {
                        isFilling = true;
                        currentBuffer = new List<byte>();
                        totalSizeMess = BitConverter.ToInt32(dataBuff, header);
                        received -= 4;
                        header += 4;
                    }

                    if (onError != null)
                    {
                        onError.Invoke(this, "received:" + received + "  totalSizeMess:" + totalSizeMess);
                    }

                    int ableReceive = Math.Min(received, totalSizeMess);
                    totalSizeMess -= ableReceive;
                    received -= ableReceive;
                    byte[] messFill = new byte[ableReceive];
                    Array.Copy(dataBuff, header, messFill, 0, ableReceive);
                    currentBuffer.AddRange(messFill);
                    header += ableReceive;
                    if (onError != null)
                    {
                        onError.Invoke(this, "currentBuffer:" + currentBuffer.Count);
                    }

                    if (totalSizeMess <= 0)
                    {
                        EzMessage pMsg = new EzMessage(currentBuffer.ToArray());
                        pMsg.deCode();
                        onMessage(this, pMsg);
                        isFilling = false;
                    }
                    else
                    {
                        onError.Invoke(this, "not enought " + totalSizeMess);
                    }
                } while (received > 0);
                     
            }
      
        }

        public void onReceiveCallBack(IAsyncResult rs)
        {
            Socket client = (Socket)rs.AsyncState;
            if (client != null)
            {
                if (onMessage != null)
                {
                    SocketError error;
                    int received = client.EndReceive(rs, out error);
                    if (error != SocketError.Success)
                    {
                        if (onError != null)
                        {
                            onError.Invoke(this, error.ToString());
                            return;
                        }
                    }
             
                    onReceiveByteData(buffer, received);

                }
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(onReceiveCallBack), client);
            }
        }
       static int totalSizeMess = 0;
        static List<byte> currentBuffer;
        static bool isFilling = false;

        public OnEventOpen onOpen { get => EzClient.OnOpen; set => EzClient.OnOpen = value; }
        public OnEventClose onClose { get => EzClient.OnClose; set => EzClient.OnClose = value; }
        public OnEventMessage onMessage { get => EzClient.OnMessage; set => EzClient.OnMessage = value; }
        public OnEventError onError { get => EzClient.OnError; set => EzClient.OnError = value; }
        public async Task StartAsync()
        {
            // Bắt đầu lắng nghe kết nối HTTP
            _clientHttp.Start();
            do
            {

                try
                {
                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : waiting a client connect");

                    // Một client kết nối đến
                    HttpListenerContext context = await _clientHttp.GetContextAsync();
                    List<byte> bb = new List<byte>();
                    var ss = context.Request.InputStream;
                    int next = ss.ReadByte();
                    while (next != -1)
                    {
                        bb.Add((byte)next);
                        next = ss.ReadByte();
                    }
                    EzMessage msg = new EzMessage(bb.ToArray());
                    msg.deCode();
                    onMessage.Invoke(this, msg);
                    // await ProcessRequest(context);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("...");

            }
            while (_clientHttp.IsListening);
        }

        private QHWebSocket websock;
        void createcConnect()
        {
            if (isHttp)
            {
               // var t = Task.Run(StartAsync);
              //  t.Wait();
              websock.Connect();
                //  var url = string.Format("ws://{0}:{1}", _uri,_port);
                //   websock = new QHWebSocket(url);
                // onOpen?.Invoke(this);
                return;
            }
            try
            {
                if (_clientSocket.Connected) return;
                var url = string.Format("http://{0}", _uri);
                _clientSocket.Connect(_uri, _port);
                if (onOpen != null)
                {
                    onOpen(this);
                }
                 _clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(onReceiveCallBack), _clientSocket);
              
                //do
                //{
                //    int received = _clientSocket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                //    if (onError != null)
                //    {
                //        onError.Invoke(this, received.ToString());
                //    }
                //    byte[] dataBuff = new byte[received];
                //    Array.Copy(buffer, dataBuff, received);
                //    EzMessage pMsg = new EzMessage(dataBuff);
                //    pMsg.deCode();
                //    onMessage(this, pMsg);
                //}
                //while (isOpen);
            }
            catch (Exception e)
            {
                if (onError != null)
                {
                    onError.Invoke(this, e.Message);
                }
            }
            if (onError != null)
            {
                onError.Invoke(this, "out thread");
            }
        }

        public void Dispose()
        {
            //isOpen = false;
            if (!isHttp)
            {
                _clientSocket.Close();
            }
            else
            {
                websock.Close();
            }
        }

        public void Close()
        {
            if (!isHttp)
            {
                _clientSocket.Close();
            }
            else
            {
                websock.Close();
            }
            
        }


    }
}
