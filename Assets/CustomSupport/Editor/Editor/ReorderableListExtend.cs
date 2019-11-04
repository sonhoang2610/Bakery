using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Kit.Editor
{
    public class ReorderableListExtend
    {
        #region variable
        SerializedObject serializedObject;
        SerializedProperty property;

        IList mainList;
        Type mainType;

        private string propertyName;

        List<float> elementHeights;
        ReorderableList orderList;

        Texture2D backgroundImage;
        #endregion

        #region System
        public ReorderableListExtend(SerializedObject serializedObject, string propertyName,
            bool dragable = true, bool displayHeader = true, bool displayAddButton = true, bool displayRemoveButton = true)
        {
            this.propertyName = propertyName;
            this.serializedObject = serializedObject;
            this.property = serializedObject.FindProperty(this.propertyName);
            elementHeights = new List<float>(property.arraySize);
            SetHightLightBackgroundImage();

            orderList = new ReorderableList(serializedObject, property, dragable, displayHeader, displayAddButton, displayRemoveButton);
            orderList.onAddCallback += OnAdd;
            orderList.onSelectCallback += OnSelect;
            orderList.onRemoveCallback += OnRemove;
            orderList.drawHeaderCallback += OnDrawHeader;
            orderList.drawElementCallback += OnDrawElement;
            orderList.drawElementBackgroundCallback += OnDrawElementBackground;
            orderList.elementHeightCallback += OnCalculateItemHeight;
        }

        public ReorderableListExtend(IList list, Type pTypeList,
     bool dragable = true, bool displayHeader = true, bool displayAddButton = true, bool displayRemoveButton = true)
        {
            
            SetHightLightBackgroundImage();
            mainList = list;
            mainType = pTypeList;
            elementHeights = new List<float>(list.Count);
            orderList = new ReorderableList(list, pTypeList, dragable, displayHeader, displayAddButton, displayRemoveButton);
            this.propertyName = list.ToString();
            orderList.onAddCallback += OnAdd;
            orderList.onSelectCallback += OnSelect;
            orderList.onRemoveCallback += OnRemove;
            orderList.drawHeaderCallback += OnDrawHeader;
            orderList.drawElementCallback += OnDrawElement;
            orderList.drawElementBackgroundCallback += OnDrawElementBackground;
            orderList.elementHeightCallback += OnCalculateItemHeight;
        }

        ~ReorderableListExtend()
        {
            orderList.onAddCallback -= OnAdd;
            orderList.onSelectCallback -= OnSelect;
            orderList.onRemoveCallback -= OnRemove;
            orderList.drawHeaderCallback -= OnDrawHeader;
            orderList.drawElementCallback -= OnDrawElement;
            orderList.drawElementBackgroundCallback -= OnDrawElementBackground;
            orderList.elementHeightCallback -= OnCalculateItemHeight;
            backgroundImage = null;
        }
        #endregion

        #region API
        public virtual void SetHightLightBackgroundImage()
        {
            backgroundImage = new Texture2D(3, 1);
            backgroundImage.SetPixel(0, 0, new Color(0f, .8f, .7f));
            backgroundImage.hideFlags = HideFlags.DontSave;
            backgroundImage.wrapMode = TextureWrapMode.Clamp;
            backgroundImage.Apply();
        }

        public void DoLayoutList()
        {
            orderList.DoLayoutList();
        }

        public void DoList(Rect rect)
        {
            orderList.DoList(rect);
        }
        #endregion

        #region listener
        protected virtual void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, property.displayName);
        }




        private void OnAdd(ReorderableList list)
        {
            if (mainList == null)
            {
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                serializedObject.ApplyModifiedProperties();
                list.index = index;
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                OnAdd(list, element);
            }
            else
            {
                object pObject = null;
                if (mainType.IsSubclassOf(typeof(ScriptableObject)))
                {

                     pObject = ScriptableObject.CreateInstance(mainType);
                }
                else
                {
                    pObject = Activator.CreateInstance(mainType);
                }
                mainList.Add(pObject);
                OnAdd(list, pObject, mainType);
            }
       
        }

        private void OnRemove(ReorderableList list)
        {
            if (mainList == null)
            {
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(list.index);
                OnRemove(list, element);
            }
            else
            {
                object pObject = mainList[list.index];
                OnRemove(list, pObject,mainType);
            }
        }

        private void OnSelect(ReorderableList list)
        {
            if (mainList == null)
            {
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(list.index);
                OnSelect(list, element);
            }
            else
            {
                object pObject = mainList[list.index];
                OnSelect(list, pObject, mainType);
            }
        }

        private void OnDrawElement(Rect rect, int index, bool active, bool focused)
        {
            if (mainList== null)
            {
                if (property == null || property.arraySize <= index)
                    return;

                SerializedProperty element = property.GetArrayElementAtIndex(index);
                float height = EditorGUI.GetPropertyHeight(element) + EditorGUIUtility.standardVerticalSpacing;
                RenewElementHeight(index, height);
                rect.height = height;
                // rect.width -= 40;
                //rect.x += 20;

                OnDrawElement(rect, index, active, focused, element);
            }
            else
            {
                if (mainList.Count <= index || index < 0)
                    return;
                float height = ElementHeightCallBack(index);
                RenewElementHeight(index, height);
                rect.height = height;
                OnDrawElement(rect, index, active, focused, mainList[index], mainType);
            }
        }
        private void OnDrawElementBackground(Rect rect, int index, bool active, bool focused)
        {
            if (mainList == null)
            {
                if (property == null || property.arraySize <= index || index < 0)
                    return;

                SerializedProperty element = property.GetArrayElementAtIndex(index);
                float height = elementHeights[index];
                rect.height = height;
                rect.width -= 4;
                rect.x += 2;

                OnDrawElementBackground(rect, index, active, focused, element, height);
            }
            else
            {
                if (mainList.Count <= index || index < 0)
                    return;
                float height = elementHeights[index];
                rect.height = height;
                rect.width -= 4;
                rect.x += 2;
                OnDrawElementBackground(rect, index, active, focused,mainList[index],mainType, height);
            }
        }
        #endregion

        #region Template
        protected virtual void OnAdd(ReorderableList list, SerializedProperty newElement) { }
        protected virtual void OnAdd(ReorderableList list, object newElement, Type pType) { }
        protected virtual void OnSelect(ReorderableList list, SerializedProperty selectedElement) { }
        protected virtual void OnSelect(ReorderableList list, object pObjectdelete, Type type) { }
        protected virtual void OnRemove(ReorderableList list, SerializedProperty deleteElement)
        {
            if (EditorUtility.DisplayDialog(
                "Warning !",
                "Are you sure you want to delete:\n\r[ " + deleteElement.displayName + " ] ?",
                "Yes", "No"))
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            }
        }
        protected virtual void OnRemove(ReorderableList list,object pObjectdelete,Type type)
        {
            if (EditorUtility.DisplayDialog(
                "Warning !",
                "Are you sure you want to delete:\n\r[ " + "this object" + " ] ?",
                "Yes", "No"))
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            }
        }
        protected virtual void OnDrawElement(Rect rect, int index, bool active, bool focused, SerializedProperty element)
        {
            EditorGUI.PropertyField(rect, element, true);
        }
        protected virtual void OnDrawElement(Rect rect, int index, bool active, bool focused, object pObject,Type pType)
        {
         
        }
        protected virtual void OnDrawElementBackground(Rect rect, int index, bool active, bool focused, SerializedProperty element, float height)
        {
            if (active)
                EditorGUI.DrawTextureTransparent(rect, backgroundImage, ScaleMode.ScaleAndCrop);
        }

        protected virtual void OnDrawElementBackground(Rect rect, int index, bool active, bool focused, object pObject,Type pType, float height)
        {
            if (active)
                EditorGUI.DrawTextureTransparent(rect, backgroundImage, ScaleMode.ScaleAndCrop);
        }

        protected virtual float ElementHeightCallBack(int index)
        {
            return 20.0f;
        }
        #endregion

        #region height hotfix
        private void RenewElementHeight(int index, float height)
        {
            try
            {
                elementHeights[index] = height;
            }
            catch
            {
            }
            finally
            {
                ElementListOverflowFix();
            }
        }
        private float OnCalculateItemHeight(int index)
        {
            float height = 0.0f;
            try
            {
                if (height != elementHeights[index])
                {
                    height = elementHeights[index];
                    EditorUtility.SetDirty(serializedObject.targetObject);
                }
            }
            catch
            {
            }
            finally
            {
                ElementListOverflowFix();
            }
            return height;
        }
        private void ElementListOverflowFix()
        {
            if (mainList == null)
            {
                if (property.arraySize != elementHeights.Count)
                {
                    float[] floats = elementHeights.ToArray();
                    Array.Resize(ref floats, property.arraySize);
                    elementHeights = floats.ToList();
                    EditorUtility.SetDirty(serializedObject.targetObject);
                }
            }
            else
            {
                float[] floats = elementHeights.ToArray();
                Array.Resize(ref floats, mainList.Count);
                elementHeights = floats.ToList();
            }
        }
        #endregion
    }
}
