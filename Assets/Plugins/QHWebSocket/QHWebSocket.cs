/* 
 * QHWebsocket for Mobile/Desktop Platform on Unity(+Editor) or Xamarin
 */

using System;
using System.Text;
using qhmono;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using EazyEngine.Networking;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using System.Diagnostics;
#endif


#if UNITY_WEBGL && !UNITY_EDITOR
using AOT;
using System.Collections.Generic;
public enum WebSocketState : ushort{	
	Connecting, Open,
	Closing,
	Closed
}

/* Websocket utilies using QHMessage for transfering data */
public class QHWebSocket :  IClient{
	
	public System.Uri uri {
		get;
		protected set;
	}

	public QHWebSocket (Uri uri){
		this.uri = uri;
		string protocol = uri.Scheme;
		if (!protocol.Equals ("ws") && !protocol.Equals ("wss")) {
		throw new System.ArgumentException ("Unsupported protocol: " + protocol);
		}
		m_hash = GetHashCode ();
		qhws [m_hash] = this;
	}

	private List<byte> msg_buffer = new List<byte>();
	
	public QHWebSocket (string uri){
		this.uri = new Uri (uri);
		string protocol = this.uri.Scheme;
		if (!protocol.Equals ("ws") && !protocol.Equals ("wss")) {
		throw new System.ArgumentException ("Unsupported protocol: " + protocol);
		}
		m_hash = GetHashCode ();
		qhws [m_hash] = this;
	}

	// Storing instance of this of this object
	static Dictionary<int,QHWebSocket> qhws = new Dictionary<int,QHWebSocket> ();
	int m_hash;

	// Now create WebSocket
	delegate void OnWebSocketOpen (int hashcode);
	delegate void OnWebSocketClose (int hashcode, ushort code, string reason);
	delegate void OnWebSocketMessage (int hashcode, IntPtr ptr, int length);
	delegate void OnWebSocketError (int hashcode, string reason);

	// Callback in Unity will pass to JavaScript web-browser
	[MonoPInvokeCallback (typeof(OnWebSocketOpen))]
	public static void onWebSocketOpen (int hashcode){
		QHWebSocket ws = null;
		if (qhws.TryGetValue (hashcode, out ws) && ws != null && EzClient.OnOpen != null) {
		EzClient.OnOpen (ws);
		}
	}

	[MonoPInvokeCallback (typeof(OnWebSocketMessage))]
	public static void onWebSocketMessage (int hashcode, IntPtr ptr, int length){
		QHWebSocket ws = null;
		Debug.LogFormat("OnWebSocketMessage : {0} {1} ", length , ptr);
		if (qhws.TryGetValue (hashcode, out ws) && ws != null ) {

			// Trying to decode
			var data = new byte[length];
			Marshal.Copy (ptr, data, 0, length);
			EzClient.OnEventReceiveBinaryData?.Invoke(ws,data);
		}
	}

	[MonoPInvokeCallback (typeof(OnWebSocketClose))]
	public static void onWebSocketClose (int hashcode, ushort code, string reason){
		QHWebSocket ws = null;
		if (qhws.TryGetValue (hashcode, out ws) && ws != null && EzClient.OnClose != null) {
            ws.msg_buffer.Clear();
            EzClient.OnClose (ws, code, reason);
		}
	}

	[MonoPInvokeCallback (typeof(OnWebSocketError))]
	public static void onWebSocketError (int hashcode, string reason){
		QHWebSocket ws = null;
		if (qhws.TryGetValue (hashcode, out ws) && ws != null && EzClient.OnError  != null) {
            ws.msg_buffer.Clear();
			EzClient.OnError (ws,reason);
		}
	}


	// Implementation in QHWebSocket.jslib
	[DllImport ("__Internal")]
	static extern void SocketCreate (string uri, int hashcode, OnWebSocketOpen onOpen, OnWebSocketMessage onMessage, OnWebSocketClose onClose, OnWebSocketError onError);

	[DllImport ("__Internal")]
	static extern bool SocketSend (int hash, byte[] data, int length);

	[DllImport ("__Internal")]
	static extern int SocketState (int hash);

	[DllImport ("__Internal")]
	static extern void SocketClose (int hash);

	// Must return a Web Socket
	public void Connect (){		
		SocketCreate (uri.ToString (), m_hash, onWebSocketOpen, onWebSocketMessage, onWebSocketClose, onWebSocketError);
	}

	public bool Send (QHMessage msg){
		if (readyState == WebSocketState.Open){
			byte[] data = msg.encode ();
			return Send (data);
		}
		return false;
	}

	public bool Send (byte[] data){
		if (readyState == WebSocketState.Open){
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
				sb.AppendFormat("{0:X2} ",data[i]);
			UnityEngine.Debug.LogFormat("Send {0} bytes : {1}", data.Length, sb.ToString());

			/*
			// Copy from managed to native
			IntPtr ptr = Marshal.AllocHGlobal (data.Length);
			Marshal.Copy (data, 0, ptr, data.Length);
			SocketSend (m_hash, ptr, data.Length);
			Marshal.FreeHGlobal (ptr);
			SocketSend (m_hash, ptr, data.Length);


			GCHandle handle = GCHandle.Alloc (data, GCHandleType.Pinned);
			SocketSend(m_hash,handle.AddrOfPinnedObject(),data.Length);
			handle.Free();
			*/

			// Call native js
			return SocketSend(m_hash,data,data.Length);
		}
		return false;
	}

	public bool Send (string str){
		if (readyState == WebSocketState.Open){
			byte[] data = System.Text.Encoding.UTF8.GetBytes (str);
			Send(data);
			return true;
		}
		return false;
	}

	public WebSocketState readyState {
		get {
			return (WebSocketState)SocketState(m_hash);
		}
	}

	public void connect()
	{
		
	}

	public void Send(EzMessage msg)
	{
		msg.endCode();
		Send(msg
			.ToArray());
	}

	public void Close (){
		SocketClose (m_hash);
	}

	public OnEventOpen onOpen { get => EzClient.OnOpen; set => EzClient.OnOpen = value; }
	public OnEventClose onClose { get => EzClient.OnClose; set => EzClient.OnClose = value; }
	public OnEventMessage onMessage { get => EzClient.OnMessage; set => EzClient.OnMessage = value; }
	public OnEventError onError { get => EzClient.OnError; set => EzClient.OnError = value; }
}
#else
/* Desktop or Mobile platform */
public enum WebSocketState : ushort
{
    Connecting = WebSocketSharp.WebSocketState.Connecting,
    Open = WebSocketSharp.WebSocketState.Open,
    Closing = WebSocketSharp.WebSocketState.Closing,
    Closed = WebSocketSharp.WebSocketState.Closed
}

public class QHWebSocket : IClient
{

    // // Delegate
    // public delegate void OnEventOpen(IClient ws);
    // public delegate void OnEventClose(IClient ws, ushort code, string reason);
    // public delegate void OnEventMessage(IClient ws, QHMessage msg);
    // public delegate void OnEventError(IClient ws, string error);
    //
    // // Delegate point
    // public static OnEventOpen onOpen;
    // public static OnEventClose onClose;
    // public static OnEventMessage onMessage;
    // public static OnEventError onError;
    private List<byte> msg_buffer = new List<byte>();
    public System.Uri uri
    {
        get;
        protected set;
    }

    public QHWebSocket(Uri uri)
    {
        this.uri = uri;
        string protocol = uri.Scheme;
        if (!protocol.Equals("ws") && !protocol.Equals("wss"))
        {
            throw new System.ArgumentException("Unsupported protocol: " + protocol);
        }
    }

    public QHWebSocket(string uri)
    {
        this.uri = new Uri(uri);
        string protocol = this.uri.Scheme;
        if (!protocol.Equals("ws") && !protocol.Equals("wss"))
        {
            throw new System.ArgumentException("Unsupported protocol: " + protocol);
        }
    }

    WebSocketSharp.WebSocket m_ws;

    // Public SSL Configuration
    public string SSLCertificate
    {
        get;
        set;
    }

    // Certificate password
    public string SSLPassword
    {
        get;
        set;
    }

    public void Connect()
    {
        m_ws = new WebSocketSharp.WebSocket(uri.ToString());

#if !UNITY_WEBGL || UNITY_EDITOR
        if (!string.IsNullOrEmpty(SSLCertificate))
        {
            if (!string.IsNullOrEmpty(SSLPassword))
                m_ws.SslConfiguration.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate(SSLCertificate, SSLPassword));
            else
                m_ws.SslConfiguration.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate(SSLCertificate));
        }
#endif
        // Handling Connected case
        m_ws.OnOpen += delegate (object sender, EventArgs e) {
            if (onOpen != null)
                onOpen(this);
        };

        // Handling Error case
        m_ws.OnError += delegate (object sender, WebSocketSharp.ErrorEventArgs e) {
            if (onError != null)
            {
                onError(this, e.Message);
            }
        };

        // Handling Disconnect case
        m_ws.OnClose += delegate (object sender, WebSocketSharp.CloseEventArgs e) {
            if (onClose != null)
                onClose(this, e.Code, e.Reason);
        };

        // Handling Message data
        m_ws.OnMessage += delegate (object sender, WebSocketSharp.MessageEventArgs e) {
            if (e.IsBinary)
            {
	            var databuff = e.RawData;
	            EzClient.OnEventReceiveBinaryData?.Invoke(this,databuff);
            }
            else if (e.IsText)
            {
#if UNITY_5_3_OR_NEWER
                Debug.LogFormat("onMessage(Text): {0} bytes, {1}", e.Data.Length, e.Data);
#else
                    // Debug.WriteLine ("onMessage(Text): {0} bytes, {1}", e.Data.Length, e.Data);
#endif
            }
            else if (e.IsPing)
            {
#if UNITY_5_3_OR_NEWER
                Debug.LogFormat("onMessage(Ping)");
#else
                    // Debug.WriteLine ("onMessage(Ping)");
#endif
            }
        };

        // Connect to server
        m_ws.ConnectAsync();
    }

    public WebSocketState readyState
    {
        get
        {
            return (m_ws == null) ? WebSocketState.Closed : (WebSocketState)m_ws.ReadyState;
        }
    }

    public bool Send(QHMessage msg)
    {
        return Send(msg.encode());
    }


    public bool Send(byte[] data)
    {
        if (m_ws.ReadyState == WebSocketSharp.WebSocketState.Open)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                sb.AppendFormat("{0:X2} ", data[i]);
            m_ws.Send(data);
            return true;
        }
        return false;
    }

    public bool Send(string str)
    {
        if (m_ws.ReadyState == WebSocketSharp.WebSocketState.Open)
        {
            m_ws.Send(str);
            return true;
        }
        return false;
    }

    public void connect()
    {
	    throw new NotImplementedException();
    }

    public void Send(EzMessage msg)
    {
	    msg.endCode();
	    m_ws.Send(msg.ToArray());
    }

    public void Close()
    {
        m_ws.Close();
    }

    public OnEventOpen onOpen { get => EzClient.OnOpen; set => EzClient.OnOpen = value; }
    public OnEventClose onClose { get => EzClient.OnClose; set => EzClient.OnClose = value; }
    public OnEventMessage onMessage { get => EzClient.OnMessage; set => EzClient.OnMessage = value; }
    public OnEventError onError { get => EzClient.OnError; set => EzClient.OnError = value; }
}
#endif