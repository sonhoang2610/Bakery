using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EazyReflectionSupport;


public abstract class EazyGetValueEditor<T> : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect current = position;
        current.width /= 2;
        SerializedProperty prop = property.FindPropertyRelative("target");

        EditorGUI.ObjectField(current, prop,new GUIContent());
        string[] methoStrings = new string[0]; 
        if (prop != null && prop.objectReferenceValue != null)
        {
          GameObject gameObject =  (GameObject)prop.objectReferenceValue;
          EazyMethodInfo[] methos =  gameObject.getAllEzMethodReturnType(typeof(T));
           methoStrings = methos.convertToStringMethods();
        }
        current.x = current.width ;
        prop = property.FindPropertyRelative("selected");
        prop.intValue = EditorGUI.Popup(current, prop.intValue, methoStrings);
    }
}


