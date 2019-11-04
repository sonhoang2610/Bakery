using System;
using System.Collections;
using System.Collections.Generic;
using EazyEngine.Networking;
using UnityEngine;
using UIWidgets;
using UnityEngine.UI;

public class BoxCalendar : BaseBox<ItemCalendar,CalendarStruct> {
    public int limitDateLoad;
    public DateTime current;
    public Text monthTitle;
    protected override void OnEnable()
    {
        base.OnEnable();
        current = DateTime.Now;
        limitDateLoad =  DateTime.DaysInMonth(current.Year, current.Month);
        current = current.AddDays(-(current.Day - 1));
        CalendarStruct[] infos = new CalendarStruct[limitDateLoad];
        for (int i = 0; i < limitDateLoad; ++i)
        {
            DateTime nextDay  =  current.AddDays(i);
            infos[i] = new CalendarStruct(nextDay,0);
        }
        DataSource = infos.ToObservableList();
        progressloadScheduleFrom(current.ToString("dd/MM/yyyy"));
    }

    public void nextMonth()
    {
        current = current.AddMonths(1);
        monthTitle.text = current.Month.ToString();
        limitDateLoad = DateTime.DaysInMonth(current.Year, current.Month);
        current = current.AddDays(-(current.Day - 1));
        CalendarStruct[] infos = new CalendarStruct[limitDateLoad];
        for (int i = 0; i < limitDateLoad; ++i)
        {
            DateTime nextDay = current.AddDays(i);
            infos[i] = new CalendarStruct(nextDay, 0);
        }
        DataSource = infos.ToObservableList();
        progressloadScheduleFrom(current.ToString("dd/MM/yyyy"));
    }

    public void previousMonth()
    {
        current = current.AddMonths(-1);
        monthTitle.text = current.Month.ToString();
        limitDateLoad = DateTime.DaysInMonth(current.Year, current.Month);
        current = current.AddDays(-(current.Day - 1));
        CalendarStruct[] infos = new CalendarStruct[limitDateLoad];
        for (int i = 0; i < limitDateLoad; ++i)
        {
            DateTime nextDay = current.AddDays(i);
            infos[i] = new CalendarStruct(nextDay, 0);
        }
        DataSource = infos.ToObservableList();
        progressloadScheduleFrom(current.ToString("dd/MM/yyyy"));
    }

    public void reload(string pTime,int pQuanity)
    {
        //for(int i = 0; i < items.Count; ++i)
        //{
        //    if(items[i]._info.time.ToString("dd/MM/yyyy") == pTime)
        //    {
        //        items[i]._info.Quantity = pQuanity;
        //        items[i].setInfo(items[i]._info);
        //    }
        //}
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    public void progressloadScheduleFrom(string pDateTime)
    {
        EzMessage pMSg = new EzMessage((byte)ComonMsg.LoadScheduleFrom);
        pMSg.addValue((byte)LoadScheduleFrom.Time, pDateTime);
        ConnectionManager.Instance.Ws.Send(pMSg);
    }
    protected override void onExecuteMessage(EzMessage msg)
    {
        if(msg.Header == (ushort)ComonMsg.LoadScheduleFromACK)
        {
         var pTable =   msg.getValue((byte)LoadScheduleFromACK.TABLE) as EzTable;
            if(pTable != null)
            {
                DateTime current = DateTime.Now;
                List<CalendarStruct> pInfos = new List<CalendarStruct>();
                for(int i = 0; i < pTable.row; ++i)
                {
                  for(int j  = 0; j < DataSource.Count; ++j)
                    {
                        if(DataSource[j].time.ToString("dd/MM/yyyy") == pTable._datas[1, i].ToString())
                        {
                            DataSource[j] = new CalendarStruct(DataSource[j].time, (int)pTable._datas[0, i]);
                        }
                    }
                   
                }
            }
        }
    }
}
