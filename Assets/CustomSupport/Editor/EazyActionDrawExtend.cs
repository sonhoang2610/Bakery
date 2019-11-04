using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using EazyCustomAction;
using Extends;

public class EazyActionDrawExtend  {
    RootMotionController target;
    int indexGroup;
    int indexInGroup;
    SerializedObject serializedObject;
    SerializedProperty mainProp;
    SerializedProperty[] fieldValues;

    public RootMotionController Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
        }
    }

    public int IndexGroup
    {
        set
        {
            indexGroup = value;
        }
    }

    public int IndexInGroup
    {
        set
        {
            indexInGroup = value;
        }
    }

    public virtual void OnEnable()
    {

        serializedObject = new SerializedObject(target);
        SerializedProperty prop = serializedObject.FindProperty("_arrayAction");
         prop = prop.GetArrayElementAtIndex(indexGroup);
        if(prop != null)
        {
            prop = prop.FindPropertyRelative("_arrayAction");
            prop = prop.GetArrayElementAtIndex(indexInGroup);
            if (prop != null)
            {
                mainProp = prop.FindPropertyRelative("_info");
            }
        }
        EazyActionInfo mainObject = (EazyActionInfo)SerializedPropertyExtensions.GetTargetObjectOfProperty(mainProp);

        Type typeAction = mainObject.SelectedAction.MainType;
        FieldInfo[] fields = typeAction.GetFields();
        List<SerializedProperty> values = new List<SerializedProperty>();
        foreach(FieldInfo field in fields)
        {
            if (field.IsDefined(typeof(CustomExtendFieldAction), false))
            {
                values.Add(mainProp.FindPropertyRelative(field.Name));
            }
        }
        fieldValues = values.ToArray();
    }
    public virtual float getHeight()
    {
        return 0;
    }
    public void onDraw(Rect rect)
    {
        for(int i = 0; i < fieldValues.Length; ++i)
        {
            EditorGUI.PropertyField(rect, fieldValues[i]);
        }
    }
}
