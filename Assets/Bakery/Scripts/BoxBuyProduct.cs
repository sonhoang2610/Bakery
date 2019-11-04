using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UIWidgets;

public class BoxBuyProduct : BaseBox<ProductShortBuy, ProductStatusInfo> {
    public ProductStatusInfo[] _infoss;
    //protected override void OnEnable()
    //{
    //}

    //protected override void OnDisable()
    //{
    //}

    //public override void executeInfos(ProductStatusInfo[] pInfos)
    //{
    //    base.executeInfos(pInfos);
    //    _infoss = pInfos;
    //}
    protected override bool CanOptimize()
    {
        return true;
    }
    public override ObservableList<ProductStatusInfo> DataSource
    {
        get
        {
            return base.DataSource;
        }

        set
        {
            base.DataSource = value;
            _infoss = value.ToArray();
        }
    }
}
