using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UIWidgets;

[System.Serializable]
public class EventOnDataItem : UnityEvent<object>
{

}

public struct StringParamArray
{
    public  string[] _data;
    public StringParamArray(string[] pData)
    {
        _data = pData;
    }
}

public  class BaseItem<T> : ListViewItem, IViewData<T> where T : new() {
   // [HideInInspector]
    public T _info;
    [HideInInspector]
    public List<EventOnDataItem> _onData =new List<EventOnDataItem>();


    public virtual void onExecute(int index)
    {
       if(_onData != null && index < _onData.Count)
        {
            _onData[index].Invoke(_info);
        }
    }

    public virtual void onExecuteFirst()
    {
        onExecute(0);
    }

    public virtual void SetData(T item)
    {
        _info = item;
    }
}
