using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UIWidgets;

[System.Serializable]
public class EventExportProducts : UnityEvent<ProductStatusInfo[]>
{

}

public class BoxInsertPRoduct : BaseBox<ItemSchedule, ProductStatusInfo> {
    public EventExportProducts onExport;

    protected override void OnEnable()
    {
        List<ProductStatusInfo> infos = new List<ProductStatusInfo>();
        for (int i = 0; i < ProductConfig.nameProduct.Keys.Count; ++i)
        {
            var pInfo = ProductConfig.nameProduct[ProductConfig.nameProduct.Keys.ElementAt(i)];
            if (pInfo.quantity > 0)
            {
                infos.Add( new ProductStatusInfo(pInfo.idProduct, 0, 0));
            }
        }
        DataSource = infos.ToObservableList();
    }

    protected override void OnDisable()
    {
    }

    public void export()
    {
        ProductStatusInfo[] pInfos = DataSource.ToArray();
        for(int i = 0; i < pInfos.Length; ++i)
        {
            ItemSchedule pItem = null;
            int pIndex = i;
            GetVisibleComponents().ForEach((ItemSchedule item) => {
                if(item._info == pInfos[pIndex])
                {
                    pItem = item;
                    return;
                }
            });
            if(pItem != null)
            {
                pInfos[i].quantitySchedule = int.Parse(pItem.labelQuantity.text);
                pInfos[i].quantityUsed = int.Parse(pItem.labelQuantity.text);
            }
        }
        onExport.Invoke(pInfos);
        gameObject.SetActive(false);
    }

}