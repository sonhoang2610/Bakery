using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSetBool : MonoBehaviour {
    Animator _animator;
    string _param;
    public void setParameter(string pParam)
    {
        _param = pParam;
    }

    public void setBool(bool pBool)
    {
        _animator.SetBool(_param, pBool);
    }
	// Use this for initialization
	void Start () {
        _animator = GetComponent<Animator>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
