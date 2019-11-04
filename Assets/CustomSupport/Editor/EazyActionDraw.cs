using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EazyCustomAction;
#if UNITY_EDITOR
using UnityEditor;
#endif
using EazyEditor;
using EazyReflectionSupport;

[Serializable]
public class EazyActionDraw
{
    [SerializeField]
    EazyActionInfo _info;
    
    public EazyActionInfo Info
    {
        get
        {
            return _info;
        }

        set
        {
            _info = value;
            if (_info != null)
            {
                // selectAction(_info.SelectedAction);
            }
        }
    }

    public delegate void GUIDrawer(Rect pos);
    public GUIDrawer _guiDrawer;

    public string[] TypeInfoIn = { "From Runtime Object", "From", "Funcion From", "Property From", "Field From" };
    public string[] TypeInfoOut = { "To", "Funcion", "Property", "Field" };
    string[] _popUpUnitAction = { "Duration", "Speed" };
    string[] _popupLoop = { "Loop Time", "Loop Forever" };
    string[] _popupEase = { "Ease", "EaseManual" };

    public bool _pressedAdd = false;
    public GameObject _objectTest = null;

    public bool _disable = true;
    public bool _playing = false;




    public void setPlaying(bool pBool)
    {
        _playing = pBool;
    }

    public void selectActionBaseOn(Type pOption)
    {
        if (pOption == typeof(Vector3))
        {
            _guiDrawer = new GUIDrawer(onDrawVector3Base);
        }
        else if (pOption == typeof(Color))
        {
            _guiDrawer = new GUIDrawer(onDrawColorBase);
        }
        else if (pOption == typeof(float))
        {
            _guiDrawer = new GUIDrawer(onDrawFloatBase);
        }
        else
        {
            _guiDrawer = null;
        }
    }

    public void OnGui(Rect position)
    {
        onDrawBaseAction(position);

    }
    int chooseComponent = 0;
    Type[] cahceComponents;
    Type[] CahceComponents
    {
        get
        {
            return cahceComponents == null ? cahceComponents = _objectTest.getAllComponentStringType() : cahceComponents;
        }
    }



    EazyActionContructor[] cacheActions;
    EazyActionContructor[] CacheActions(Type component)
    {
        return cacheActions == null ? cacheActions = EazyEditorTools.getAllEzAction(component) : cacheActions;
    }
    void onDrawBaseAction(Rect position)
    {

        EditorGUI.BeginDisabledGroup(_disable);
        if (Info.IsEnableBy)
        {
            TypeInfoOut[0] = "By";
        }
        else
        {
            TypeInfoOut[0] = "To";
        }
        int newChoose;
        EditorGUI.BeginDisabledGroup(Info.Layer == 0);
        GUILayout.BeginHorizontal(GUILayout.Width(position.width));
        if (_objectTest)
        {
            chooseComponent = CahceComponents.convertStrings().findIndex(Info.SelectedAction.MainComponent);
            newChoose = EditorGUILayout.Popup(chooseComponent, CahceComponents.convertStrings(), GUILayout.Width(100));
            if (chooseComponent != newChoose)
            {
                chooseComponent = newChoose;
                cacheActions = null;
                Info.SelectedAction = EazyActionContructor.Sequences;
                EazyActionContructor action = Info.SelectedAction;
                action.MainComponent = cahceComponents[chooseComponent].ToString();
                Info.SelectedAction = action;
            }
        }
        if (CahceComponents != null)
        {
            CacheActions(CahceComponents[chooseComponent]);
        }
        int choose = cacheActions.findActionByName(Info.SelectedAction.name);
        if (cacheActions != null)
        {
            newChoose = EditorGUILayout.Popup(choose, cacheActions.convertStrings());
            if (newChoose != choose)
            {
                Info.SelectedAction = cacheActions[newChoose];
            }
        }
        selectActionBaseOn(EazyEditorTools.getTypeActionBaseOn(Info.SelectedAction.MainType));
        //Info.SelectedAction = (ActionOption)EditorGUILayout.EnumPopup(Info.SelectedAction);
        //selectActionBaseOn(Info.SelectedAction);
        EditorGUILayout.LabelField("Parent:", GUILayout.Width(50));
        EditorGUI.BeginDisabledGroup(true);
        if (Info.ActionParrent != null)
        {
            EditorGUILayout.TextField(Info.ActionParrent.Name);
        }
        else
        {
            EditorGUILayout.TextField("None");
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();
        if (Info.SelectedAction.name != EazyActionContructor.Sequences.name)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(position.width));
            EditorGUILayout.LabelField("Object Test:", GUILayout.MinWidth(10), GUILayout.MaxWidth(80));
            EditorGUI.BeginDisabledGroup(true);
            _objectTest = EditorGUILayout.ObjectField(_objectTest, typeof(GameObject), true, GUILayout.MaxWidth(position.width - 280)) as GameObject;
            EditorGUI.EndDisabledGroup();
            //if (Info.SelectedAction == ActionOption.MoveTo || Info.SelectedAction == ActionOption.MoveBy)
            //{
            Info.IsLocal = EditorGUILayout.Toggle(Info.IsLocal, GUILayout.Width(20));
            EditorGUILayout.LabelField("On local", GUILayout.MinWidth(10), GUILayout.MaxWidth(80));
            Info.IsEnableBy = EditorGUILayout.Toggle(Info.IsEnableBy, GUILayout.Width(10));
            EditorGUILayout.LabelField("enable By", GUILayout.MinWidth(10), GUILayout.MaxWidth(80));
            //}
            EditorGUILayout.EndHorizontal();

        }
        if (Info.Layer == 0)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(position.width));
            EditorGUILayout.LabelField("Name Action:", GUILayout.MinWidth(0), GUILayout.MaxWidth(100));
            Info.Name = EditorGUILayout.TextField(Info.Name, GUILayout.MaxWidth(position.width - 100));

            GUILayout.EndHorizontal();
        }
        if (_guiDrawer != null)
        {

            _guiDrawer(position);
        }
        onDrawBotBaseAction(position);
        EditorGUI.EndDisabledGroup();
    }
    void onDrawBotBaseAction(Rect position)
    {
        GUILayout.BeginHorizontal(GUILayout.Width(position.width));
        Info.LoopType = EditorGUILayout.Popup(Info.LoopType, _popupLoop, GUILayout.MaxWidth(100));
        EditorGUI.BeginDisabledGroup(Info.LoopType != 0);
        Info.LoopTime = EditorGUILayout.IntField("", Info.LoopTime, GUILayout.MinWidth(0), GUILayout.MaxWidth(120));
        if (Info.LoopTime < 1)
        {
            Info.LoopTime = 1;
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();
    }
    void onDrawStartBase(Rect position)
    {
        GUILayout.BeginHorizontal(GUILayout.Width(position.width));
        Info.TypeInfoIn = EditorGUILayout.Popup(Info.TypeInfoIn, TypeInfoIn, GUILayout.MinWidth(0), GUILayout.MaxWidth(100));
    }
    void onDrawDestinyBase(Rect position)
    {
        GUILayout.BeginHorizontal(GUILayout.Width(position.width));
        Info.TypeInfoOut = EditorGUILayout.Popup(Info.TypeInfoOut, TypeInfoOut, GUILayout.MinWidth(0), GUILayout.MaxWidth(100));
    }
    void onDrawFunctionType(int pIndex)
    {
        if ((pIndex == 0 && Info.TypeInfoIn >= 2) || (pIndex == 1 && Info.TypeInfoOut >= 1))
        {

            string[] pMethodNames = { };
            EazyMethodInfo[] methoidInfos = null;
            GameObject pObjectGetMethod = null;
            if (pIndex == 0)
            {
                if (Info.TargetFrom == null)
                {
                    Info.TargetFrom = _objectTest;
                }
                pObjectGetMethod = Info.TargetFrom;
                Info.TargetFrom = (GameObject)EditorGUILayout.ObjectField(pObjectGetMethod, typeof(GameObject), true);
            }
            if (pIndex == 1)
            {
                if (Info.TargetStep == null)
                {
                    Info.TargetStep = _objectTest;
                }
                pObjectGetMethod = Info.TargetStep;
                Info.TargetStep = (GameObject)EditorGUILayout.ObjectField(pObjectGetMethod, typeof(GameObject), true);
            }

            if (pObjectGetMethod)
            {
                Type type = EazyEditorTools.getTypeActionBaseOn(Info.SelectedAction.MainType);
                int typeGet = pIndex == 0 ? Info.TypeInfoIn - 2 : Info.TypeInfoOut - 1;
                if (typeGet == 0)
                {
                    methoidInfos = pObjectGetMethod.getAllEzMethodReturnType(type);
                }
                else if (typeGet == 1)
                {
                    methoidInfos = pObjectGetMethod.getAllEzPropertyreturnType(type);
                }
                else if (typeGet == 2)
                {
                    methoidInfos = pObjectGetMethod.getAllEzFieldreturnType(type);
                }
            }
            if (methoidInfos != null)
            {
                pMethodNames = methoidInfos.convertToStringMethods();
                int indexChoose = 0;
                for (int i = 0; i < methoidInfos.Length; ++i)
                {
                    if (pIndex == 0 && methoidInfos[i].Equals(Info.infoFrom.MethodInfo))
                    {
                        indexChoose = i;
                        break;
                    }
                    else if (pIndex == 1 && methoidInfos[i].Equals(Info.infoStep.MethodInfo))
                    {
                        indexChoose = i;
                        break;
                    }
                }
                int newChoose = EditorGUILayout.Popup(indexChoose, pMethodNames);
                if (newChoose != indexChoose)
                {
                    if (pIndex == 0)
                    {
                        Info.infoFrom.MethodInfo = methoidInfos[newChoose];
                        Info.infoFrom.IsMethod = true;
                    }
                    else
                    {
                        Info.infoStep.MethodInfo = methoidInfos[newChoose];
                        Info.infoStep.IsMethod = true;
                    }
                }
            }
        }
        else
        {
            if (pIndex == 0 && Info.TypeInfoIn < 2)
            {
                Info.infoFrom.IsMethod = false;
            }
            if (pIndex == 1 && Info.TypeInfoOut < 1)
            {
                Info.infoStep.IsMethod = false;
            }
        }
        GUIStyle style = new GUIStyle();
        style.normal.background = Resources.Load("asset/help-icon-3", typeof(Texture2D)) as Texture2D;
        EditorGUILayout.LabelField(new GUIContent("", "here is tooltip"), style, GUILayout.Width(18), GUILayout.Height(18));
    }
    void onDrawBotTranformAction(Rect position)
    {
        GUILayout.BeginHorizontal(GUILayout.Width(position.width));
        Info.EaseTypePopUp = EditorGUILayout.Popup(Info.EaseTypePopUp, _popupEase, GUILayout.MaxWidth(100));
        if (Info.EaseTypePopUp == 1)
        {
            Info.CurveEase = EditorGUILayout.CurveField(Info.CurveEase, Color.green, new Rect(0, 0, 1, 1), GUILayout.MinWidth(0), GUILayout.MaxWidth(120));
        }
        else
        {
            Info.EaseType = (EaseCustomType)EditorGUILayout.EnumPopup(Info.EaseType, GUILayout.MaxWidth(120));
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(GUILayout.Width(position.width));
        Info.UnitType = EditorGUILayout.Popup(Info.UnitType, _popUpUnitAction, GUILayout.MaxWidth(100));
        Info.Unit = EditorGUILayout.FloatField("", Info.Unit, GUILayout.MinWidth(0), GUILayout.MaxWidth(120));
        Info.RealTime = EditorGUILayout.Toggle(Info.RealTime, GUILayout.Width(20));
        EditorGUILayout.LabelField("Run on realTime:", GUILayout.MinWidth(0), GUILayout.MaxWidth(100));
        GUILayout.EndHorizontal();
    }
    void onDrawVector3Base(Rect position)
    {
        onDrawStartBase(position);
        if (Info.TypeInfoIn <= 1)
        {
            EditorGUI.BeginDisabledGroup(Info.TypeInfoIn == 0);
            Info.infoFrom.Vector3 = EditorGUILayout.Vector3Field("", Info.InfoFrom.Vector3, GUILayout.MinWidth(0));
            EditorGUI.EndDisabledGroup();
        }
        onDrawFunctionType(0);
        GUILayout.EndHorizontal();
        onDrawDestinyBase(position);
        if (Info.TypeInfoOut == 0)
            Info.infoStep.Vector3 = EditorGUILayout.Vector3Field("", Info.InfoStep.Vector3, GUILayout.MinWidth(0));
        onDrawFunctionType(1);
        GUILayout.EndHorizontal();
        onDrawBotTranformAction(position);
    }
    void onDrawFloatBase(Rect position)
    {
        onDrawStartBase(position);
        if (Info.TypeInfoIn <= 1)
        {
            EditorGUI.BeginDisabledGroup(Info.TypeInfoIn == 0);
            Info.infoFrom.Float = EditorGUILayout.FloatField("", Info.InfoFrom.Float, GUILayout.MaxWidth(120));
            EditorGUI.EndDisabledGroup();
        }
        onDrawFunctionType(0);
        GUILayout.EndHorizontal();
        onDrawDestinyBase(position);
        if (Info.TypeInfoOut == 0)
            Info.infoStep.Float = EditorGUILayout.FloatField("", Info.InfoStep.Float, GUILayout.MaxWidth(120));
        onDrawFunctionType(1);
        GUILayout.EndHorizontal();
        onDrawBotTranformAction(position);
    }
    void onDrawColorBase(Rect position)
    {
        onDrawStartBase(position);
        if (Info.TypeInfoIn <= 1)
        {
            EditorGUI.BeginDisabledGroup(Info.TypeInfoIn == 0);
            Info.infoFrom.Color = EditorGUILayout.ColorField("", Info.InfoFrom.Color, GUILayout.MinWidth(0));
            EditorGUI.EndDisabledGroup();
        }
        onDrawFunctionType(0);
        GUILayout.EndHorizontal();
        onDrawDestinyBase(position);
        if (Info.TypeInfoOut == 0)
            Info.infoStep.Color = EditorGUILayout.ColorField("", Info.InfoStep.Color, GUILayout.MinWidth(0));
        onDrawFunctionType(1);
        GUILayout.EndHorizontal();
        onDrawBotTranformAction(position);
    }
    void onDrawColorAlphaBase(Rect position)
    {
        onDrawStartBase(position);
        if (Info.TypeInfoIn <= 1)
        {
            EditorGUI.BeginDisabledGroup(Info.TypeInfoIn == 0);
            Info.infoFrom.Color = Info.infoFrom.Color.setAlpha(EditorGUILayout.FloatField("", Info.InfoFrom.Color.a, GUILayout.MinWidth(0), GUILayout.MaxWidth(120)));
            EditorGUI.EndDisabledGroup();
        }
        onDrawFunctionType(0);
        GUILayout.EndHorizontal();
        onDrawDestinyBase(position);
        if (Info.TypeInfoOut == 0)
            Info.infoStep.Color = Info.infoFrom.Color.setAlpha(EditorGUILayout.FloatField("", Info.InfoStep.Color.a, GUILayout.MinWidth(0), GUILayout.MaxWidth(120)));
        onDrawFunctionType(1);
        GUILayout.EndHorizontal();
        onDrawBotTranformAction(position);
    }


}
