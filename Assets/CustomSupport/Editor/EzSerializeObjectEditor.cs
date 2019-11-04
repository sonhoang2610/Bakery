using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Extends;


[CustomPropertyDrawer(typeof(EzSerializeObject))]
public class EzSerializeObjectEditor : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
        property.serializedObject.Update();
        SerializedProperty prop = property.FindPropertyRelative("ezType");
        Rect rectDraw = position;
        rectDraw.width /= 2;
        EditorGUI.PropertyField(rectDraw, prop,new GUIContent(""));
        rectDraw.x += rectDraw.width;
        EzAllTypeShow type = (EzAllTypeShow)prop.enumValueIndex;
        object value = SerializedPropertyExtensions.GetTargetObjectOfProperty(property);
        if (type == EzAllTypeShow.EzBool)
        {
            if(((EzSerializeObject)value).Value == null || ((EzSerializeObject)value).getEzType() != EzAllTypeShow.EzBool)
            {
                ((EzSerializeObject)value).setEzType(EzAllTypeShow.EzBool);
            }
            bool pBool = EditorGUI.Toggle(rectDraw, (bool)((EzSerializeObject)value).Value);
            ((EzSerializeObject)value).Value = pBool;
        }
        else if (type == EzAllTypeShow.EzString)
        {      
            if (((EzSerializeObject)value).Value == null || ((EzSerializeObject)value).getEzType() != EzAllTypeShow.EzString)
            {
                ((EzSerializeObject)value).setEzType(EzAllTypeShow.EzString);
            }
            string pString = EditorGUI.TextField(rectDraw, (string)((EzSerializeObject)value).Value);
            ((EzSerializeObject)value).Value = pString;
        }
        else if (type == EzAllTypeShow.EzInt)
        {
            if (((EzSerializeObject)value).Value == null || ((EzSerializeObject)value).getEzType() != EzAllTypeShow.EzInt)
            {
                ((EzSerializeObject)value).setEzType(EzAllTypeShow.EzInt);
            }
            int pString = EditorGUI.IntField(rectDraw, (int)((EzSerializeObject)value).Value);
            ((EzSerializeObject)value).Value = pString;
        }
        property.serializedObject.ApplyModifiedProperties();
    }
}
