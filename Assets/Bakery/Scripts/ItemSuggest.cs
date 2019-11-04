using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UnityEngine.UI;

public class ItemSuggest : BaseItem<CustomerInfo> {
    [SerializeField]
    Text name, phone, adress;

    public override void SetData(CustomerInfo pInfo)
    {
        base.SetData(pInfo);
        name.text = pInfo.name + "\n" + pInfo.phone + "\n" + pInfo.adress;
     //   phone.text = pInfo.phone;
      //  adress.text = pInfo.adress;
    }

    public override Graphic[] GraphicsForeground
    {
        get
        {
            return new Graphic[] { name };
        }
    }
}
