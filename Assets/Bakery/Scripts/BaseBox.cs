using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Networking;
using UIWidgets;
//public static class NGUISupport
//{
//    public static T addChildNGUI<T>(this GameObject pParent, T prefab) where T : Component
//    {
//        var pItem = pParent.AddChild<T>(prefab);
//        if (pParent.transform.GetComponent<UIPanel>())
//        {
//            return pItem;
//        }
//        int depth = pParent.GetComponentInParent<UIWidget>().depth;
//        int mainDepth = pItem.GetComponentInChildren<UIWidget>().depth;
//        var widgets = pItem.GetComponentsInChildren<UIWidget>();
//        foreach (var pWidget in widgets)
//        {
//            pWidget.depth = depth + (pWidget.depth - mainDepth);
//        }
//        return pItem;
//    }

//    public static GameObject addChildGameObjectNGUI(this GameObject pParent, GameObject prefab) 
//    {
//        var pItem = pParent.AddChild(prefab);
//        if (pParent.transform.GetComponent<UIPanel>())
//        {
//            return pItem;
//        }
//        int depth = pParent.GetComponentInParent<UIWidget>().depth;
//        int mainDepth = pItem.GetComponentInChildren<UIWidget>().depth;
//        var widgets = pItem.GetComponentsInChildren<UIWidget>();
//        foreach (var pWidget in widgets)
//        {
//            pWidget.depth = depth + (pWidget.depth - mainDepth) + 1;
//        }
//        return pItem;
//    }
//}
public class BaseBox< T0, T1> : ListViewCustom<T0,T1> where T0 : BaseItem<T1> where T1 : new() 
{
    //[HideInInspector]
    //public List<T0> items = new List<T0>();
    public EventOnDataItem[] actionWithData;
    [SerializeField]
    protected GameObject loading;

    protected int oldIndex = 0;
    protected int currentPage = 0,limitPage = 1;


    public void setLimit(int pLimit)
    {
        limitPage = pLimit;

    }

    public virtual void nextPage()
    {
        currentPage++;
        if(currentPage>= limitPage)
        {
            currentPage = 0;
        }
        updatePage();
    }

    public virtual void previousPage()
    {
        currentPage--;
        if (currentPage < 0)
        {
            currentPage = limitPage - 1;
        }
        updatePage();
    }

    public virtual void updatePage()
    {

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EzClientSocket.onMessage += onExecuteMsg;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EzClientSocket.onMessage -= onExecuteMsg;
    }
    private void onExecuteMsg(EzClientSocket ws, EzMessage msg)
    {
        Dispatcher.InvokeAsync(delegate ()
        {
            onExecuteMessage(msg);
        });          
    }
    protected virtual void onExecuteMessage(EzMessage msg)
    {

    }

    protected override void SetData(T0 component, T1 item)
    {
        base.SetData(component, item);
        if (actionWithData != null)
        {
            component._onData.Clear();
            component._onData.addFromList(actionWithData);
        }
    }
}
