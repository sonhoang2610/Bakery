using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EazyObject : MonoBehaviour {
    int index;

    public int Index
    {
        get
        {
            return index;
        }

        set
        {
            index = value;
        }
    }

    public virtual void  initIndex(int index)
    {
       
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
