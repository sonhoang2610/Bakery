
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[InitializeOnLoad]
public class HierarchyHighlighter
{

    static HierarchyHighlighter()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItem_CB;
    }
    
    private static void HierarchyWindowItem_CB(int selectionID, Rect selectionRect)
    {
        if (Application.isPlaying) return;
        Object o = EditorUtility.InstanceIDToObject(selectionID);
        if (o != null && o.GetType() != typeof(MonoBehaviour) && (o as GameObject).GetComponent<IHierarchyHighlighter>() != null)
        {
            IHierarchyHighlighter h = (o as GameObject).GetComponent<IHierarchyHighlighter>();
            if (Event.current.type == EventType.Repaint)
            {
                EditorApplication.RepaintHierarchyWindow();
                GUI.color = h.getColorText();
                selectionRect.width /= 3;
                selectionRect.x = 2*selectionRect.width;
                GUI.Box(selectionRect, "");
                GUI.Box(selectionRect, "");
                GUI.Box(selectionRect, "");
                GUI.color = Color.white;
                //GUI.Label(selectionRect, o.name);
                //doing this three times because once is kind of transparent.
    
           
            }
        }
    }
}
