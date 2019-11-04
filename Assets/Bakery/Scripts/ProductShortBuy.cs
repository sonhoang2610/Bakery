using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UnityEngine.UI;

public class ProductShortBuy : BaseItem<ProductStatusInfo> {
    [SerializeField]
    Text label;
    public override void SetData(ProductStatusInfo pInfo)
    {
        base.SetData(pInfo);
        ProductStruct pInfoProduct = ProductConfig.nameProduct[pInfo.productID];
        label.text = string.Format("{0}: {1},", pInfoProduct.nameProduct, pInfo.quantitySchedule);
    }

}
