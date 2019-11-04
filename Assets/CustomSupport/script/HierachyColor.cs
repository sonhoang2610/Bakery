using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DrawType { Sphere,Cube}
[ExecuteInEditMode]
public class HierachyColor : MonoBehaviour,IHierarchyHighlighter {
    [SerializeField]
    Color color = new Color(0,0,0,0);
    [SerializeField]
    DrawType typeDraw;
    public Color getColorText()
    {
        return color;
    }

    private void Awake()
    {
        if (!Application.isPlaying)
        {
            if (color == new Color(0, 0, 0, 0))
            {
                color = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), 1);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Color defaultColor = Gizmos.color;  
        Gizmos.color = color;
        if (typeDraw == DrawType.Sphere)
        {
            Gizmos.DrawSphere(transform.position, 0.05f);
        }
        else
        {
            Gizmos.DrawCube(transform.position, new Vector3(0.05f, 0.05f, 0.05f));
        }
        Gizmos.color = defaultColor;
    }
}
