using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RepositionByScreen : MonoBehaviour {
    public Vector2 defaultResolution;
    public Transform transformPivot;
    public bool updatePos = true;
    [ReadOnly]
    public Vector2 currentPercentPos;
    public Vector2 currentResolution;

    public void executePos()
    {
        if (!updatePos)
        {
            transform.position = new Vector3(currentPercentPos.x * Screen.width + transformPivot.position.x, currentPercentPos.y * Screen.height + transformPivot.position.y);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!Application.isPlaying && updatePos)
        {
            if (transformPivot != null && defaultResolution.x != 0 && defaultResolution.y != 0)
            {
                currentPercentPos = new Vector2((transform.position.x - transformPivot.position.x) / defaultResolution.x, (transform.position.y - transformPivot.position.y) / defaultResolution.y);
            }
        }
        if(currentResolution != new Vector2(Screen.width,Screen.height)  && !updatePos)
        {
            currentResolution = new Vector2(Screen.width, Screen.height);
            executePos();
        }
	}
}
