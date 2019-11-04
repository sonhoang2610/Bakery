using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EazyEngine.Tools;
using UnityEngine.UI;

[System.Serializable]
public struct CalendarStruct
{
    public DateTime time;
    public int Quantity;
    public CalendarStruct(DateTime pTime,int pQuantity)
    {
        time = pTime;
        Quantity = pQuantity;
    }
}

public class ItemCalendar : BaseItem<CalendarStruct> {
    [SerializeField]
    Text label1, label2,labelQuantity;
    [SerializeField]
    GameObject highLight;
    public override void SetData(CalendarStruct pInfo)
    {
        base.SetData(pInfo);
        label1.text = string.Format("Thứ {0}", (int)pInfo.time.DayOfWeek + 1);
        if((int)pInfo.time.DayOfWeek == 0)
        {
            label1.text = string.Format("Chủ nhật");
        }
        label2.text = pInfo.time.ToString("dd/MM/yyyy");
        labelQuantity.text = pInfo.Quantity.ToString();
        if(DateTime.Now.ToString("dd/MM/yyyy") == pInfo.time.ToString("dd/MM/yyyy"))
        {
            highLight.gameObject.SetActive(true);
        }
        else
        {
            highLight.gameObject.SetActive(false);
        }
        //label1
    }
}
