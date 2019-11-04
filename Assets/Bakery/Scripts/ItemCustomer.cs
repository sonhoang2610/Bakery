using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class ExpandCustomItem
{
    [SerializeField]
    Text mainName, nickname, phone, adress, buyNumber, buyProduct,note;
    public void setData(CustomerInfo item)
    {
        mainName.text = item.name;
        nickname.text = item.nickname;
        phone.text = item.phone;
        adress.text = item.adress;
        buyNumber.text = item.buy.ToString();
        buyProduct.text = item.number_bought.ToString();
        note.text = item.note;
    }
}

public class ItemCustomer : BaseItem<CustomerInfo>, IResizableItem
{
    [SerializeField]
    Text mainName, nickname, phone, adress,buyNumber,buyProduct;
    [SerializeField]
    ExpandCustomItem expand;
    [SerializeField]
    Graphic[] foreground; 
    [SerializeField]
    UnityEvent onSelectCustom, onDeSelectCustom;

 



    bool isSelected = false;
    public GameObject[] ObjectsToResize
    {
        get
        {
            return new GameObject[] { mainName.gameObject, nickname.gameObject, phone.gameObject, adress.gameObject, buyNumber.gameObject, buyProduct.gameObject };
        }
    }
    public override Graphic[] GraphicsForeground
    {
        get
        {
            return foreground;
        }
    }

    public void hideShortInfo()
    {
        for(int i = 0; i < ObjectsToResize.Length; ++i)
        {
            ObjectsToResize[i].SetActive(false);
        }
    }
    public void showShortInfo()
    {
        for (int i = 0; i < ObjectsToResize.Length; ++i)
        {
            ObjectsToResize[i].SetActive(true);
        }
    }
    public override void SetData(CustomerInfo item)
    {
        base.SetData(item);
        mainName.text = item.name;
        nickname.text = item.nickname;
        phone.text = item.phone;
        adress.text = item.adress;
        buyNumber.text = item.buy.ToString();
        buyProduct.text = item.number_bought.ToString();
        if(expand != null)
        {
            expand.setData(item);
        }
    }

    public void SetData(TreeViewItem item)
    {
        throw new System.NotImplementedException();
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        onTurnSelect( eventData);
    }

    public void onTurnSelect(PointerEventData eventData)
    {
        isSelected = !isSelected;
        if (isSelected)
        {
            OnSelect(eventData);
            onSelectCustom.Invoke();
        }
        else
        {
            OnDeselect(eventData);
            onDeSelectCustom.Invoke();
        }
    }

    public void ticket()
    {
        BoxBuy.instance.gameObject.SetActive(true);
        BoxBuy.instance.setInfo(_info);
    }

    public void showProfile()
    {
        BoxProfile.instance.gameObject.SetActive(true);
        BoxProfile.instance.SetData(_info);
    }
}
