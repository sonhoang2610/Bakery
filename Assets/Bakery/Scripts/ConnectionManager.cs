using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Networking;
using EazyEngine.Tools;
using UnityEngine.Events;


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
        Application.targetFrameRate = 300;
        Application.runInBackground = false;
       /// NativeGallery.RequestPermission();
        EzClientSocket.onOpen = onOpenConnect;
        EzClientSocket.onMessage += onMessage;
        EzClientSocket.onError = onError;
        EzClientSocket.onClose = delegate (EzClientSocket ws,ushort a, string error)
        {
            if (Ws != null)
            {
                Ws.Dispose();
            }
            Ws = new EzClientSocket(uri, port);
            Ws.connect();
        };
        Ws = new EzClientSocket(uri,port);
        Ws.connect();
    }
    IEnumerator currentPing;
    bool pingSuccess = true;
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
            Ws = new EzClientSocket(uri, port);
            Ws.connect();
        }
  
    }

    public void onError(EzClientSocket ws, string error)
    {
        Debug.Log(error);
    }
    private void OnDisable()
    {
       
    }

    private void OnDestroy()
    {
        EzClientSocket.onMessage -= onMessage;
        EzClientSocket.onOpen -= onOpenConnect;
        EzClientSocket.onError -= onError;
        ws.Dispose();
    }
    public void onMessage(EzClientSocket ws,EzMessage pMsg)
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

    public void onOpenConnect(EzClientSocket ws)
    {
        Dispatcher.InvokeAsync(delegate {
            Debug.Log("open");
            onSuccessEvent.Invoke();
            currentPing = delayPing(0.1f);
            pingSuccess = true;
            StartCoroutine(currentPing);
        });
     
    }
    int indexpause = 0;
    public void OnApplicationPause(bool pause)
    {
        if (!pause && indexpause == 0)
        {
     
            indexpause = 1;
            if (currentPing != null)
            {
                StopCoroutine(currentPing);
            }
            pingSuccess = false;
            currentPing = delayPing(0);
            StartCoroutine(currentPing);

        }
        if (pause)
        {
            onPause.Invoke();
            indexpause = 0;
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
