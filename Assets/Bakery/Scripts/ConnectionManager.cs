using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Networking;
using EazyEngine.Tools;
using UnityEngine.Events;
using UnityEngine.Networking;


public struct ProductStruct
{
    public string idProduct, nameProduct,desc,pic;
    public int price;
    public int quantity;

    public ProductStruct(string pID,string pName,string pDes,int pPrice,string pic,int pQuantity = 0)
    {
        idProduct = pID;
        nameProduct = pName;
        desc = pDes;
        price = pPrice;
        this.pic = pic;
        quantity = pQuantity;
    }
}
public class ProductConfig
{

    public static Dictionary<string, ProductStruct> nameProduct = new Dictionary<string, ProductStruct>();
}

public class AdressConfig
{

    public static Dictionary<string, int> price = new Dictionary<string, int>();
}

public class ConnectionManager : Singleton<ConnectionManager> {

    public string uri = "127.0.0.1";
    public int port = 9999;
    public UnityEvent onSuccessEvent;
    public UnityEvent onPause;
    EzClientSocket ws;

    public EzClientSocket Ws
    {
        get
        {
            return ws;
        }

        set
        {
            ws = value;
        }
    }

    // Use this for initialization
    void Start () {
      //  Application.targetFrameRate = 300;
        Application.runInBackground = false;
       /// NativeGallery.RequestPermission();
        EzClient.OnOpen = onOpenConnect;
        EzClient.OnMessage+= onMessage;
        EzClient.OnError = onError;
        EzClientSocket.customSendHTTP = customSend;
    
        EzClient.OnClose = delegate (IClient ws,ushort a, string error)
        {
            if (Ws != null)
            {
                Ws.Dispose();
            }
            Ws = new EzClientSocket(uri, port,useSocket);
            Ws.connect();
        };
        Ws = new EzClientSocket(uri,port,useSocket);
        Ws.connect();
    }
    

    IEnumerator startSend(byte[] datas)
    {
        UnityWebRequest www = new UnityWebRequest();
        www.url = string.Format("http://{0}:{1}/", (object) uri, (object) port);
        www.method = UnityWebRequest.kHttpVerbPOST;
        www.downloadHandler = new DownloadHandlerBuffer();
        www.uploadHandler = new UploadHandlerRaw(datas);
        www.timeout = 5;
        www.SetRequestHeader("Content-Type", "applicatopn/json");
        //   var pTime = ((Int32)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
        //   www.SetRequestHeader("time", pTime);
        //  www.SetRequestHeader("token_id", MD5.ComputeHash(pTime + "b96256a5d8bb2cc7d81b5db5c80328fb"));
        yield return www.SendWebRequest();

        if (www.error == null)
        {

            var myContent = www.downloadHandler.data;
            EzMessage msg = new EzMessage(myContent);
            msg.deCode();
            EzClient.OnMessage.Invoke(null,msg); 
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    void customSend(byte[] datas)
    {
        StartCoroutine(startSend(datas));
    }
    IEnumerator currentPing;
    bool pingSuccess = true;
    public bool useSocket;
    public IEnumerator delayPing(float pDelay)
    {
        yield return new WaitForSeconds(pDelay);
        if (ws != null && pingSuccess)
        {
            currentPing = delayPing(10);
            StartCoroutine(currentPing);
            EzMessage pms = new EzMessage((ushort)ComonMsg.LoadProduct);
            pingSuccess = false;
            ws.Send(pms);
        }
        else
        {
           Ws = new EzClientSocket(uri, port,useSocket);
           Ws.connect();
        }
  
    }

    public void onError(IClient ws, string error)
    {
        Debug.Log(error);
    }
    private void OnDisable()
    {
       
    }

    private void OnDestroy()
    {
        EzClient.OnMessage-= onMessage;
        EzClient.OnOpen -= onOpenConnect;
        EzClient.OnError -= onError;
        EzClient.OnClose = null;
        ws.Dispose();
    }
    public void onMessage(IClient ws,EzMessage pMsg)
    {
        Dispatcher.InvokeAsync(delegate {
            if (pMsg.Header == (ushort)ComonMsg.LoadProductAck)
            {
                pingSuccess = true;
                if (ProductConfig.nameProduct.Keys.Count == 0)
                {
                    EzTable table = (EzTable)pMsg.getValue((byte)LoadCustomerAcKParam.TABLE);
                    for (int i = 0; i < table.row; ++i)
                    {
                        ProductConfig.nameProduct.Add(table._datas[0, i].ToString(), new ProductStruct(table._datas[0, i].ToString(), table._datas[1, i].ToString(), table._datas[2, i].ToString(), (int)table._datas[3, i], table._datas[5, i].ToString(), (int)table._datas[4, i]));
                    }

                    EzTable table1 = (EzTable)pMsg.getValue((byte)LoadCustomerAcKParam.TABLE_ADRESS);
                    for (int i = 0; i < table1.row; ++i)
                    {
                        AdressConfig.price.Add(table1._datas[0, i].ToString(), (int)table1._datas[1, i]);
                    }
                }
            }
            
        });
    }

    private bool isOpen = false;
    public void onOpenConnect(IClient ws)
    {
        if (!isOpen)
        {
            isOpen = true;
            Dispatcher.InvokeAsync(delegate
            {
                Debug.Log("open");
                onSuccessEvent.Invoke();
                currentPing = delayPing(0.1f);
                pingSuccess = true;
                StartCoroutine(currentPing);
            });
        }

    }
    int indexpause = 0;
    public void OnApplicationPause(bool pause)
    {
        // if (!pause && indexpause == 0)
        // {
        //
        //     indexpause = 1;
        //     if (currentPing != null)
        //     {
        //         StopCoroutine(currentPing);
        //     }
        //     pingSuccess = false;
        //     currentPing = delayPing(0);
        //     StartCoroutine(currentPing);
        //
        // }
        // if (pause)
        // {
        //     onPause.Invoke();
        //     indexpause = 0;
        // }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
