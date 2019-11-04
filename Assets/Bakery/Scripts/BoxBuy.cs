using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Networking;
using System;
using EazyEngine.Tools;
using UnityEngine.UI;
using UIWidgets;
using System.Linq;

public class BoxBuy : Singleton<BoxBuy> {
    [SerializeField]
    InputField sdt, username, adress, timeDay,timeMonth,timeYear, note;
    [SerializeField]
    BoxBuyProduct boxBuy;
    [SerializeField]
    GameObject loading;
    [SerializeField]
    GameObject successText;

    public static BoxBuy instance;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }


    System.Action deleteTicket;
    bool fixItem = false;
    public void setInfoFix(object pObject,DateTime pTime,Pos[] pTickets, System.Action pActionDelete)
    {
        fixItem = true;
        deleteTicket = pActionDelete;
        CustomerInfo pInfo = (CustomerInfo)pObject;
        username.text = pInfo.name;
        adress.text = pInfo.adress;
        timeDay.text = pTime.ToString("dd");
        timeMonth.text = pTime.ToString("MM");
        timeYear.text = pTime.ToString("yyyy");
        sdt.text = pInfo.phone;
        note.text = pInfo.note;
        List<ProductStatusInfo> list = new List<ProductStatusInfo>();
        for (int i = 0; i < pTickets.Length; ++i)
        {
            ProductStatusInfo pInfoo = new ProductStatusInfo();
            pInfoo.productID = pTickets[i].x.ToString();
            pInfoo.quantitySchedule = pTickets[i].y;
            pInfoo.quantityUsed = pTickets[i].y;
            list.Add(pInfoo);
        }
        boxBuy.DataSource = list.ToObservableList();
        //email.text = pInfo.email;
    }

    public void setInfo(object pObject)
    {
        fixItem = false;
        CustomerInfo pInfo = (CustomerInfo)pObject;
        username.text = pInfo.name;
        adress.text = pInfo.adress;
        timeDay.text = "";
        timeMonth.text = DateTime.Now.ToString("MM");
        timeYear.text = DateTime.Now.ToString("yyyy");
        sdt.text = pInfo.phone;
        note.text = pInfo.note;
       // TicketStruct pInfo =  pObject;
        //email.text = pInfo.email;
    }

    public void accept()
    {
        if(string.IsNullOrEmpty(sdt.text) || string.IsNullOrEmpty(username.text) || string.IsNullOrEmpty(adress.text)
            || string.IsNullOrEmpty(timeDay.text) || string.IsNullOrEmpty(timeMonth.text) || string.IsNullOrEmpty(timeYear.text) || string.IsNullOrEmpty(note.text) ||( boxBuy._infoss == null || boxBuy._infoss.Length == 0))
        {
            return;
        }
        loading.gameObject.SetActive(true);
        if (fixItem)
        {

            deleteTicket();
            StartCoroutine(delayTicket(delegate
            {
                EzMessage pMsg = new EzMessage((ushort)ComonMsg.InsertTicket);
                pMsg.addValue((byte)InsertTicketMsg.ADRESS, adress.text);
                pMsg.addValue((byte)InsertTicketMsg.DATETIME, timeDay.text + "/" + timeMonth.text + "/" + timeYear.text);
                pMsg.addValue((byte)InsertTicketMsg.EMAIL, "");
                pMsg.addValue((byte)InsertTicketMsg.NAME, username.text);
                pMsg.addValue((byte)InsertTicketMsg.NOTE, note.text);
                pMsg.addValue((byte)InsertTicketMsg.PHONE, sdt.text);
                string[] productIDs = new string[boxBuy._infoss.Length];
                int[] products = new int[boxBuy._infoss.Length];
                for (int i = 0; i < boxBuy._infoss.Length; ++i)
                {
                    productIDs[i] = boxBuy._infoss[i].productID;
                    products[i] = boxBuy._infoss[i].quantitySchedule;
                }
                pMsg.addValue((byte)InsertTicketMsg.PRODUCTS, products);
                pMsg.addValue((byte)InsertTicketMsg.PRODUCT_IDS, productIDs);

                ConnectionManager.Instance.Ws.Send(pMsg);
            }));
        }
        else
        {
            EzMessage pMsg = new EzMessage((ushort)ComonMsg.InsertTicket);
            pMsg.addValue((byte)InsertTicketMsg.ADRESS, adress.text);
            pMsg.addValue((byte)InsertTicketMsg.DATETIME, timeDay.text + "/" + timeMonth.text + "/" + timeYear.text);
            pMsg.addValue((byte)InsertTicketMsg.EMAIL, "");
            pMsg.addValue((byte)InsertTicketMsg.NAME, username.text);
            pMsg.addValue((byte)InsertTicketMsg.NOTE, note.text);
            pMsg.addValue((byte)InsertTicketMsg.PHONE, sdt.text);
            string[] productIDs = new string[boxBuy._infoss.Length];
            int[] products = new int[boxBuy._infoss.Length];
            for (int i = 0; i < boxBuy._infoss.Length; ++i)
            {
                productIDs[i] = boxBuy._infoss[i].productID;
                products[i] = boxBuy._infoss[i].quantitySchedule;
            }
            pMsg.addValue((byte)InsertTicketMsg.PRODUCTS, products);
            pMsg.addValue((byte)InsertTicketMsg.PRODUCT_IDS, productIDs);

            ConnectionManager.Instance.Ws.Send(pMsg);
        }


    }

    public IEnumerator delayTicket(System.Action pAction)
    {
        yield return new WaitForSeconds(0.2f);
        pAction.Invoke();
    }

    private void OnEnable()
    {
        EzClientSocket.onMessage += onExecuteMsg;
        ResetBox();
        fixItem = false;
    }

    private void OnDisable()
    {
        EzClientSocket.onMessage -= onExecuteMsg;
    }

    public void ResetBox()
    {
        sdt.text = "";
        username.text = "";
        //email.text = "";
        adress.text = "";
        timeDay.text = "";
        timeMonth.text = DateTime.Now.ToString("MM");
        timeYear.text = DateTime.Now.ToString("yyyy");
        note.text = "";
        List<ProductStatusInfo> list = new List<ProductStatusInfo>();
        for(int i = 0; i < ProductConfig.nameProduct.Count; ++i)
        {
            ProductStatusInfo pInfo = new ProductStatusInfo();
            pInfo.productID = ProductConfig.nameProduct.ElementAt(i).Key;
            pInfo.quantitySchedule = 0;
            pInfo.quantityUsed = 0;
        }
        boxBuy.DataSource = list.ToObservableList() ;
    }

    private void onExecuteMsg(EzClientSocket ws, EzMessage msg)
    {
        Dispatcher.InvokeAsync(delegate {
            if(msg.Header == (ushort)ComonMsg.InsertTicketACK)
            {
                //if(BoxTicket.InstanceRaw && BoxTicket.InstanceRaw.gameObject.activeSelf)
                //{
                //    BoxTicket.InstanceRaw.updateTime();
                //}
                loading.gameObject.SetActive(false);
                successText.gameObject.SetActive(true);
                ResetBox();
                if(BoxTicket.instance != null && BoxTicket.instance.gameObject.activeInHierarchy && BoxTicket.instance.gameObject.activeSelf)
                {
                    BoxTicket.instance.reload();
                }
            }
        });
    }

    public void onloadProductTicket(ProductStatusInfo[] pInfo)
    {
        boxBuy.DataSource = (pInfo).ToObservableList();
    }
    // Use this for initialization
    void Start () {
        loading.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
