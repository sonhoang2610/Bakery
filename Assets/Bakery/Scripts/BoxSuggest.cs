using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Networking;
using UIWidgets;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class BoxSuggest : BaseBox<ItemSuggest, CustomerInfo>, IDeselectHandler
{

    protected Transform CanvasTransform;

    protected override void Awake()
    {
        base.Awake();
    }
    public virtual void HideOptions()
    {
        if (CanvasTransform != null)
        {
            Utilites.GetOrAddComponent<HierarchyToggle>(this).Restore();
        }

        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the options.
    /// </summary>
    public virtual void ShowOptions()
    {
        DataSource.Clear();
        CanvasTransform = Utilites.FindTopmostCanvas(this.transform);
        if (CanvasTransform != null)
        {
            Utilites.GetOrAddComponent<HierarchyToggle>(this).SetParent(CanvasTransform);
        }

        this.gameObject.SetActive(true);
        gameObject.GetComponent<Selectable>().Select();
    }
    protected override void onExecuteMessage(EzMessage msg)
    {
        Dispatcher.InvokeAsync(delegate
        {
            if (msg.Header == (ushort)ComonMsg.FilterCustomerACK)
            {
                executeFilterCustomer(msg);
            }
        });
    }
    public void findCustomerByPhone(Text pPhone)
    {
        if (!string.IsNullOrEmpty(pPhone.text))
        {
            ShowOptions();
        }
        else
        {
            return;
        }
        EzMessage pMsg = new EzMessage((ushort)ComonMsg.FilterCustomerNextGen);
        pMsg.addValue((byte)FilterCustomerParam.Filter, 0);
        pMsg.addValue((byte)FilterCustomerParam.Value, pPhone.text);
        pMsg.addValue((byte)FilterCustomerParam.Page, 0);
        ConnectionManager.Instance.Ws.Send(pMsg);
    }
    public void findCustomerByName(Text pName)
    {
        if (!string.IsNullOrEmpty(pName.text))
        {
            ShowOptions();
        }
        else
        {
            return;
        }
        EzMessage pMsg = new EzMessage((ushort)ComonMsg.FilterCustomerNextGen);
        pMsg.addValue((byte)FilterCustomerParam.Filter, 4);
        pMsg.addValue((byte)FilterCustomerParam.Value, pName.text);
        ConnectionManager.Instance.Ws.Send(pMsg);
    }
    public int checkExist(List<CustomerInfo> pList, int pID)
    {
        for (int i = 0; i < pList.Count; ++i)
        {
            if (pList[i].id == pID)
            {
                return i;
            }
        }
        return -1;
    }
    public void executeFilterCustomer(EzMessage msg)
    {
        EzTable tableData = (EzTable)msg.getValue((byte)LoadCustomerAcKParam.TABLE);
        List<CustomerInfo> pInfos = new List<CustomerInfo>();
        object[,] data = tableData._datas;
        for (int i = 0; i < tableData.row; ++i)
        {
            CustomerInfo pInfo = new CustomerInfo();
            pInfo.id = (int)data[9, i];
            pInfo.nickname = data[4, i].ToString();
            pInfo.number_bought = (int)data[14, i];
            pInfo.buy = (int)data[15, i];
            pInfo.facebookID = data[5, i].ToString();
            pInfo.avaLink = data[6, i].ToString();
            pInfo.cancel = (int)data[2, i];
            pInfo.score = (int)data[2, i];
            pInfos.Add(pInfo);
            List<CustomerProfile> listprofile = new List<CustomerProfile>();
            listprofile.Add(new CustomerProfile(data[12, i].ToString(), data[10, i].ToString(), data[11, i].ToString(), data[1, i].ToString(), ""));
            pInfo.profiles = listprofile.ToArray();
        }
        DataSource = pInfos.ToObservableList();
    }

    protected override void DeselectHandlerOnDeselect(BaseEventData eventData)
    {
        base.DeselectHandlerOnDeselect(eventData);
        var ev = eventData as PointerEventData;
        if (ev.pointerCurrentRaycast.gameObject == null || !ev.pointerCurrentRaycast.gameObject.transform.IsChildOf(transform))
        {
            HideOptions();
        }
    }
}
