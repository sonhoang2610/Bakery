using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ScheduleUpdate(float pSec);
public class UpdateObject
{
    private bool _runOnRealTime = false;
    public bool RunOnRealTime { get { return _runOnRealTime; } set { _runOnRealTime = value; } }
    public string _name="";
    public float _limitUpdate = 0;
    public float _currentUpdate = 0;
    public ScheduleUpdate _sche;
    public UpdateObject(ScheduleUpdate pSche, float pSec)
    {
        _sche = pSche;
        _limitUpdate = pSec;
    }
}
public class ScheduleLayer : MonoBehaviour
{
  
    private List<UpdateObject> _listUpdate;
    public void scheduleUpdate(ScheduleUpdate pUpdate, float pSec)
    {
        scheduleUpdate(new UpdateObject(pUpdate,pSec));
    }

    public void scheduleUpdate(UpdateObject pUpdate)
    {
        if (_listUpdate == null)
        {
            _listUpdate = new List<UpdateObject>();
        }
        _listUpdate.Add(pUpdate);
    }

    public UpdateObject getObjectUpdateName(string pName)
    {
        for (int i = 0; i < _listUpdate.Count; i++)
        {
            UpdateObject pUpdate = _listUpdate[i];
           if(pUpdate._name == pName)
            {
                return pUpdate;
            }
        }
        return null;
    }

    private void Start()
    {
        int abc = 0;
        abc++;
    }
    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < _listUpdate.Count; i++)
        {
            UpdateObject pUpdate = _listUpdate[i];
           float delta = !pUpdate.RunOnRealTime ? Time.deltaTime : Time.fixedUnscaledDeltaTime;
            pUpdate._currentUpdate += delta;
            if (pUpdate._currentUpdate >= pUpdate._limitUpdate)
            {
                pUpdate._sche(pUpdate._currentUpdate);
                pUpdate._currentUpdate = 0;
            }
        }
    }
    
 
}
