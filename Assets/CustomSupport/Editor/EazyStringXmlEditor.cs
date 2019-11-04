using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Kit.Editor;

[CustomPropertyDrawer(typeof(EazyStringXml))]
public class EazyStringXmlEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.targetObject && property.serializedObject.targetObject.GetType().IsSubclassOf(typeof(MonoBehaviour)))
        {
            SerializedProperty isXml = property.FindPropertyRelative("_xml");
            Rect rectToogle = position;
            rectToogle.x = rectToogle.width - 20;
            rectToogle.width = 20;
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUI.PropertyField(rectToogle, isXml, new GUIContent(""));
            EditorGUI.EndDisabledGroup();
            Rect rectStr = position;
            rectStr.width -= 40;
            if (isXml.boolValue)
            {
                EditorGUI.PropertyField(rectStr, property.FindPropertyRelative("_tag"), new GUIContent(property.name + "(Xml tag)"));
            }
            else
            {
                EditorGUI.PropertyField(rectStr, property.FindPropertyRelative("_stringDefault"), new GUIContent(property.name));
            }
        }
        else
        {
            Rect currentRect = position;
            currentRect.width *= 0.3f;
            EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("_tag"),new GUIContent(""));
            currentRect.x += position.width * 0.33f;
            currentRect.width = position.width * 0.6f;
            EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("_stringXml"), new GUIContent(""));
        }
    }
}

[CustomEditor(typeof(EazyStringXml))]
public class EazyStringXmlInspector : Editor
{
    public override void OnInspectorGUI()
    {
       
    }
}

