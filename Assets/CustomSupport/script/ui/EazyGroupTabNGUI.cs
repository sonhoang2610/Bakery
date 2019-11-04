using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EazyGroupTabNGUI : MonoBehaviour {
    [SerializeField]
    private List<EazyTabNGUI> groupTab;
    [SerializeField]
    private List<Transform> groupLayer;
    [SerializeField]
    UnityEventInt onChooseIndex;
    int currentTab;

    public int CurrentTab
    {
        get
        {
            return currentTab;
        }

        set
        {
            currentTab = value;
        }
    }

    public List<Transform> GroupLayer
    {
        get
        {
            return groupLayer;
        }
    }

    public List<EazyTabNGUI> GroupTab
    {
        get
        {
            if(groupTab == null)
            {
                groupTab = new List<EazyTabNGUI>();
            }
            return groupTab;
        }
    }
    
    public bool isLockOnEnable = false;
    bool isFirst = true;
    private void OnEnable()
    {
        reloadTabs();
        if (!isFirst && !isLockOnEnable)
        {
            if (currentTab != 0) return;
            changeTab(0);
        }   
    }
    // Use this for initialization
     public virtual void  Start () {
        isFirst = false;
        reloadTabs();
        if (currentTab != 0) return;
        changeTab(0);
    }


    public void reloadTabs()
    {
        for (int i = 0; i < GroupTab.Count; i++)
        {
            GroupTab[i].Index = i;
            GroupTab[i].ParentTab = this;
        }
    }

    public void changeTab(int index)
    {
        if (onChooseIndex != null)
        {
            onChooseIndex.Invoke(index);
        }
        CurrentTab = index;
       for (int i = 0; i < GroupLayer.Count; i++)
        {
            if (GroupLayer[i] != null)
            {
                GroupLayer[i].gameObject.SetActive(false);
            }
        }
        if (GroupLayer.Count > 0 && index < GroupLayer.Count)
        {
            if (GroupLayer[index] != null)
            {
                GroupLayer[index].gameObject.SetActive(true);
                if (GroupLayer[index].GetComponent<EazyObject>())
                {
                    GroupLayer[index].GetComponent<EazyObject>().initIndex(index);
                }
            }
        }
        for (int i = 0; i < GroupTab.Count; i++)
        {
            if (i == index)
            {
                GroupTab[i].Pressed = (true);
            }
            else
            {
                GroupTab[i].Pressed = (false);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
     
	}
}
