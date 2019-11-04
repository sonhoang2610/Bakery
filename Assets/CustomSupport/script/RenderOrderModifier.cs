using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderOrderModifier : MonoBehaviour {
	public int orderLayer;

    Renderer[] renderParticle;

    private void OnEnable()
    {

    }

    private void Awake()
    {
        renderParticle = GetComponentsInChildren<Renderer>();
        if (renderParticle != null)
        {
            for (int i = 0; i < renderParticle.Length; ++i)
            {
                renderParticle[i].sortingOrder = orderLayer;
            }
        }

    }
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
