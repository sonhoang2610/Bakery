using EazyEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class ListViewCustomer : BaseBox<ItemCustomer, CustomerInfo>
{
    bool isEnable = false, isFisrt = true;
    public ComboboxIcons filter;
    public InputField input;
    public Text page;
    int filterIndex;
    string valueFind;

    public static ListViewCustomer instance;
    protected override void OnEnable()
    {
        base.OnEnable();
        isEnable = true;
        isFisrt = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        isEnable = false;
    }

    private void LateUpdate()
    {
        if (isEnable)
        {
            isEnable = false;
            updatePage();
        }
    }

    public override void Start()
    {
        base.Start();
        instance = this;
    }

    protected override void onExecuteMessage(EzMessage msg)
    {
        if (msg.Header == (ushort)ComonMsg.LoadCustomerTableACK)
        {
           // executeLoadCustomer(msg);
        }
        else if (msg.Header == (ushort)ComonMsg.FilterCustomerACK)
        {
            executeFilterCustomer(msg);
            loading.gameObject.SetActive(false);
        }
    }

    public int checkExist(List<CustomerInfo> pList ,int pID)
    {
        for(int i = 0; i < pList.Count; ++i)
        {
            if(pList[i].id == pID)
            {
                return i;
            }
        }
        return -1;
    }
    public void executeFilterCustomer(EzMessage msg)
    {
        int pPage = msg.getInt((byte)LoadCustomerAcKParam.PAGE);
        setLimit(pPage);
        EzTable tableData = (EzTable)msg.getValue((byte)LoadCustomerAcKParam.TABLE);
        List<CustomerInfo> pInfos = new List<CustomerInfo>();
        object[,] data = tableData._datas;
        for (int i = 0; i < tableData.row; ++i)
        {
            CustomerInfo pInfo = new CustomerInfo();
            int pIndex = checkExist(pInfos, (int)data[9, i]);
            if (pIndex < 0)
            {
                pInfo.id = (int)data[9, i];
                pInfo.nickname = data[4, i].ToString();
                pInfo.number_bought = (int)data[14, i];
                pInfo.buy = (int)data[15, i];
                pInfo.facebookID = data[5, i].ToString();
                pInfo.avaLink = data[6, i].ToString();
                pInfo.cancel = (int)data[2, i];
                pInfo.score = (int)data[2, i];
                pInfo.description = data[17, i].ToString();
                pInfos.Add(pInfo);
            }
            else
            {
                pInfo = pInfos[pIndex];
            }
            List<CustomerProfile> listprofile = new List<CustomerProfile>();
            if (pInfo.profiles != null)
            {
                listprofile.AddRange(pInfo.profiles);
            }
            listprofile.Add(new CustomerProfile(data[12, i].ToString(), data[10, i].ToString(), data[11, i].ToString(), data[1, i].ToString(), "", (int)data[16, i]));
            pInfo.profiles = listprofile.ToArray();
        }
        DataSource = pInfos.ToObservableList();
        input.text = "";
    }
    public override void updatePage()
    {
        page.text = (currentPage + 1).ToString();
        EzMessage pMsg = new EzMessage((ushort)ComonMsg.FilterCustomerNextGen);
        pMsg.addValue((byte)FilterCustomerParam.Filter, filterIndex);
        pMsg.addValue((byte)FilterCustomerParam.Value, (isFisrt || string.IsNullOrEmpty(valueFind)) ? "ALL" : valueFind);
        pMsg.addValue((byte)FilterCustomerParam.Page, currentPage);
        ConnectionManager.Instance.Ws.Send(pMsg);
        isFisrt = false;
        loading.gameObject.SetActive(true);
    }
    public void progressfindCustomer(int indexFilter, string pValue)
    {
        filterIndex = indexFilter;
        valueFind = pValue;
        currentPage = 0;
        updatePage();
    }

    public void search()
    {
        int pIndex = filter.ListView.SelectedIndex;
        if (pIndex > 0)
        {
            pIndex += 2;
        }
        progressfindCustomer(pIndex, input.text);
    }
}
