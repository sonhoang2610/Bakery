using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour {
    [SerializeField]
    int ColLimit;
    [SerializeField]
    float distanceCol,distanceRow;
    [SerializeField]
    Vector2 anchorPoint;
    [ContextMenu("Reposition")]
    public void Reposition()
    {
        //MonoBehaviour[] mObjects = transform.
        //int index = 0;
        //for(int i= 0; i < mObjects.Length; ++i)
        //{
        //    if(mObjects[i].gameObject != gameObject)
        //    {
        //        mObjects[i].transform.localPosition = new Vector3((index % ColLimit) * distanceCol, (int)(index / ColLimit) * distanceRow);
        //        index++;
        //    }     
        //}
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
