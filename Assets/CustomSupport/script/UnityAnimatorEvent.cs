using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityAnimatorEvent : MonoBehaviour {

    Animator _animator;
    private string nameParameter;

    public string NameParameter
    {
        get
        {
            return nameParameter;
        }

        set
        {
            nameParameter = value;
        }
    }

    public void setBool(bool pBool)
    {
        if(_animator != null)
        {
            _animator.SetBool(NameParameter, pBool);
        }
    }
	// Use this for initialization
	void Start () {
        _animator = GetComponent<Animator>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
