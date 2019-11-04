using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RootMotionCurves : MonoBehaviour {
    [SerializeField]
    Vector3 defaultPos = Vector3.zero;
    [SerializeField]
    bool applyRoot = false;
    public Vector3 DefaultPos
    {
        get
        {
            return defaultPos;
        }

        set
        {
            defaultPos = value;
        }
    }

    public bool ApplyRoot
    {
        get
        {
            return applyRoot;
        }

        set
        {
            applyRoot = value;
        }
    }


    private void Update()
    {
        if (!ApplyRoot)
        {
            DefaultPos = transform.localPosition;
        }
    }

    private void LateUpdate()
    {
        if (ApplyRoot)
        {
            transform.localPosition += DefaultPos;
        }
    }
}
