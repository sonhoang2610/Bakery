using UnityEngine;
using UnityToolbag;
using System.Collections.Generic;
using System;
using System.Linq;
//using System.Reflection;
using EazyCustomAction;
using System.Globalization;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AtlasLoader
{
    public Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();

    //Creates new Instance only, Manually call the loadSprite function later on 
    public AtlasLoader()
    {

    }

    //Creates new Instance and Loads the provided sprites
    public AtlasLoader(string spriteBaseName)
    {
        loadSprite(spriteBaseName);
    }

    //Loads the provided sprites
    public void loadSprite(string spriteBaseName)
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>(spriteBaseName);
        if (allSprites == null || allSprites.Length <= 0)
        {
            Debug.LogError("The Provided Base-Atlas Sprite `" + spriteBaseName + "` does not exist!");
            return;
        }

        for (int i = 0; i < allSprites.Length; i++)
        {
            spriteDic.Add(allSprites[i].name, allSprites[i]);
        }
    }

    //Get the provided atlas from the loaded sprites
    public Sprite getAtlas(string atlasName)
    {
        Sprite tempSprite;

        if (!spriteDic.TryGetValue(atlasName, out tempSprite))
        {
            Debug.LogError("The Provided atlas `" + atlasName + "` does not exist!");
            return null;
        }
        return tempSprite;
    }

    //Returns number of sprites in the Atlas
    public int atlasCount()
    {
        return spriteDic.Count;
    }
}
public static class MonoBehaviorExtension
{
    static List<MonoBehaviour> listMono;
    public static void changeScene()
    {
        listMono = null;
    }
    public static List<T> FindObjectsOfTypeAll<T>()
    {
        List<T> results = new List<T>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (s.isLoaded)
            {
                var allGameObjects = s.GetRootGameObjects();
                for (int j = 0; j < allGameObjects.Length; j++)
                {
                    var go = allGameObjects[j];
                    results.AddRange(go.GetComponentsInChildren<T>(true));
                }
            }
        }
        return results;
    }
    public static List<MonoBehaviour> Monos
    {
        get
        {
            return listMono == null ? listMono = FindObjectsOfTypeAll<MonoBehaviour>() : listMono;
        }
    }
}
namespace EazyReflectionSupport
{

    using System.Reflection;
    public enum TypeReflection { None, Method, Field, Properties }
    [Serializable]
    public struct EazyMethodInfo
    {
        public TypeReflection typeReflection;
        public string methodString;
        public string componentString;
        public GameObject target;
        object targetNormal;
        public EazyMethodInfo(object target, string method)
        {
            typeReflection = TypeReflection.Method;
            this.methodString = method;
            this.componentString = "";
            this.targetNormal = target;
            this.target = null;
        }
        public EazyMethodInfo(GameObject target, string component, string method)
        {
            targetNormal = null;
            typeReflection = TypeReflection.Method;
            this.methodString = method;
            this.componentString = component;
            this.target = target;
        }
        public EazyMethodInfo(GameObject target, TypeReflection typeReflection, string component, string method)
        {
            targetNormal = null;
            this.typeReflection = typeReflection;
            this.methodString = method;
            this.componentString = component;
            this.target = target;
        }

        public bool getValue<T>(ref T pObjectRef)
        {
            object pObjectreturn = null;
            if (target != null)
            {
                object pObject = target.GetComponent(componentString);

                if (typeReflection == TypeReflection.Method)
                {
                    pObjectreturn = pObject.GetType().GetMethod(methodString).Invoke(pObject, null);
                }
                else
                    if (typeReflection == TypeReflection.Properties)
                {
                    pObjectreturn = pObject.GetType().GetProperty(methodString).GetValue(pObject, null);
                }
                else if (typeReflection == TypeReflection.Field)
                {
                    pObjectreturn = pObject.GetType().GetField(methodString).GetValue(pObject);
                }
                if (pObjectreturn != null && typeof(T) == pObjectreturn.GetType())
                {
                    pObjectRef = (T)pObjectreturn;
                    return true;
                }
            }
            else if (targetNormal != null)
            {
                pObjectreturn = targetNormal.GetType().GetMethod(methodString).Invoke(targetNormal, null);
                if (pObjectreturn != null && pObjectreturn.GetType() == typeof(T))
                {
                    pObjectRef = (T)pObjectreturn;
                    return true;
                }
            }
            return false;
        }
    }
    [Serializable]
    public abstract class EazyGetValue<T>
    {
        public GameObject target;
        public int selected;
        public EazyMethodInfo methodInfo;
    }
    [Serializable]
    public class EazyGetBoolValue : EazyGetValue<bool>
    {
    }

    public static class TypeExtension
    {
        public static Type getType(string strType)
        {
            if (strType != "")
            {
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly ass in assemblies)
                {
                    Type pType = ass.GetType(strType);
                    if (pType != null)
                    {
                        return pType;
                    }
                }
            }
            return null;
        }

        public static Type convertToType(this EazyType pType)
        {
            if (pType != null && pType.Assembly != null && pType.Assembly != "" && pType.MainType != null && pType.MainType != "")
            {
                return Assembly.Load(pType.Assembly).GetType(pType.MainType);
            }
            return null;
        }
    }

    public static class MethodExtension
    {
        public static EazyMethodInfo[] getAllEzMethodReturnType(this GameObject pObject, Type type)
        {
            List<EazyMethodInfo> _listMedthoid = new List<EazyMethodInfo>();
            var mObjs = pObject.GetComponents<Component>();
            for (int i = 0; i < mObjs.Length; i++)
            {
                Type pType = mObjs[i].GetType();
                MethodInfo[] pMethods = pType.GetMethods();
                for (int j = 0; j < pMethods.Length; j++)
                {
                    if (pMethods[j].ReturnType == type && !pMethods[j].IsSpecialName)
                    {
                        _listMedthoid.Add(new EazyMethodInfo(pObject, pMethods[j].DeclaringType.Name, pMethods[j].Name));
                    }
                }
            }
            return _listMedthoid.ToArray();
        }

        public static EazyMethodInfo[] getAllEzFieldreturnType(this GameObject pObject, Type type)
        {
            List<EazyMethodInfo> _listMedthoid = new List<EazyMethodInfo>();
            var mObjs = pObject.GetComponents<Component>();
            for (int i = 0; i < mObjs.Length; i++)
            {
                Type pType = mObjs[i].GetType();
                FieldInfo[] pMethods = pType.GetFields();
                for (int j = 0; j < pMethods.Length; j++)
                {
                    if (pMethods[j].FieldType == type)
                    {
                        _listMedthoid.Add(new EazyMethodInfo(pObject, TypeReflection.Field, pMethods[j].DeclaringType.Name, pMethods[j].Name));
                    }
                }
            }
            return _listMedthoid.ToArray();
        }

        public static EazyMethodInfo[] getAllEzPropertyreturnType(this GameObject pObject, Type type)
        {
            List<EazyMethodInfo> _listMedthoid = new List<EazyMethodInfo>();
            var mObjs = pObject.GetComponents<Component>();
            for (int i = 0; i < mObjs.Length; i++)
            {
                Type pType = mObjs[i].GetType();
                PropertyInfo[] pMethods = pType.GetProperties();
                for (int j = 0; j < pMethods.Length; j++)
                {
                    if (pMethods[j].PropertyType == type)
                    {
                        _listMedthoid.Add(new EazyMethodInfo(pObject, TypeReflection.Properties, pMethods[j].DeclaringType.Name, pMethods[j].Name));
                    }
                }
            }
            return _listMedthoid.ToArray();
        }
        public static EazyMethodInfo[] getAllEzMethodReturnType<T>(this GameObject pObject)
        {
            List<EazyMethodInfo> _listMedthoid = new List<EazyMethodInfo>();
            var mObjs = pObject.GetComponents<Component>();
            for (int i = 0; i < mObjs.Length; i++)
            {
                Type pType = mObjs[i].GetType();

                MethodInfo[] pMethods = pType.GetMethods();
                for (int j = 0; j < pMethods.Length; j++)
                {
                    if (pMethods[j].ReturnType == typeof(T) && pMethods[j].IsGenericMethodDefinition)
                    {
                        _listMedthoid.Add(new EazyMethodInfo(pObject, pMethods[j].DeclaringType.Name, pMethods[j].Name));
                    }
                }
            }
            return _listMedthoid.ToArray();
        }

        public static MethodInfo[] getAllMethodReturnType<T>(this GameObject pObject)
        {
            List<MethodInfo> _listMedthoid = new List<MethodInfo>();
            var mObjs = pObject.GetComponents<Component>();
            for (int i = 0; i < mObjs.Length; i++)
            {
                Type pType = mObjs[i].GetType();
                MethodInfo[] pMethods = pType.GetMethods();
                for (int j = 0; j < pMethods.Length; j++)
                {
                    if (pMethods[j].ReturnType == typeof(T))
                    {
                        _listMedthoid.Add(pMethods[j]);
                    }
                }
            }
            return _listMedthoid.ToArray();
        }

        public static string[] convertToStringMethods(this MethodInfo[] vs)
        {
            string[] methodNames = new string[vs.Length];
            for (int i = 0; i < vs.Length; ++i)
            {
                methodNames[i] = vs[i].convertToStringMethod();
            }
            return methodNames;
        }

        public static string convertToStringMethod(this MethodInfo v)
        {
            return v.DeclaringType.Name + "." + v.Name + "()";
        }

        public static string[] convertToStringMethods(this EazyMethodInfo[] vs)
        {
            if (vs != null)
            {
                string[] methodNames = new string[vs.Length];
                for (int i = 0; i < vs.Length; ++i)
                {
                    methodNames[i] = vs[i].convertToStringMethod();
                }
                return methodNames;
            }
            return null;
        }

        public static string convertToStringMethod(this EazyMethodInfo v)
        {
            return v.componentString + "." + v.methodString + ((v.typeReflection == TypeReflection.Method) ? "()" : ":");
        }
    }
}
public enum EzAllTypeShow { EzInt, EzString, EzBool }
[System.Serializable]
public class EzSerializeObject : ISerializationCallbackReceiver
{
    public string jsonObject;
    object mainObject;
    public EazyType type = null;
    public EzAllTypeShow ezType;

    public object Value
    {
        get
        {
            return mainObject;
        }

        set
        {
            mainObject = value;
            if (mainObject != null)
            {
                if (type == null || type.Type != null)
                {
                    type = new EazyType();
                }
                type.Type = mainObject.GetType();
            }
            OnBeforeSerialize();
        }
    }

    public void OnAfterDeserialize()
    {
        if (type != null && type.Type != null)
        {
            if (type.Type.Namespace != "System")
            {
                mainObject = JsonUtility.FromJson(jsonObject, type.Type);
            }
            else
            {
                mainObject = Convert.ChangeType(jsonObject, type.Type);
            }
        }
    }

    public void OnBeforeSerialize()
    {
        if (type != null && type.Type != null)
        {
            if (mainObject != null)
            {
                if (type.Type.Namespace != "System")
                {
                    jsonObject = JsonUtility.ToJson(mainObject);
                }
                else
                {
                    jsonObject = mainObject.ToString();
                }
            }
            else
            {
                jsonObject = "";
            }
        }
    }

    public void setEzType(EzAllTypeShow type)
    {
        switch (type)
        {
            case EzAllTypeShow.EzBool: Value = false; break;
            case EzAllTypeShow.EzString: Value = "A"; break;
            case EzAllTypeShow.EzInt: Value = 0; break;
        }
        ezType = type;
    }
    public EzAllTypeShow getEzType()
    {
        return ezType;
    }
}
//public static class PlayMakerExtends
//{
//    public static PlayMakerFSM FindFsmOnGameObject(GameObject go, string fsmName)
//    {
//        foreach (var fsm in PlayMakerFSM.FsmList)
//        {
//            if (fsm.gameObject == go && fsm.FsmName == fsmName)
//            {
//                return fsm;
//            }
//        }
//        return null;
//    }
//}
#if UNITY_EDITOR
public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static T CreateAsset<T>(string path, string name) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        string assetPathAndName = (path + "/" + name + ".asset");
        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        return asset;
    }
}
#endif

//public static class NGUIAnchorExtends
//{

//    public static UIRect.AnchorPoint GetAnchorPoint(this UIRect rect,int index)
//    {
//        if(index == 0)
//        {
//            return rect.leftAnchor;
//        }else if(index == 1)
//        {
//            return rect.topAnchor;
//        }else if(index == 2)
//        {
//            return rect.rightAnchor;
//        }
//        else
//        {
//            return rect.bottomAnchor;
//        }
//    }
//}
public static class MathExtends
{
    public static Vector3 paraboldDraw(Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        if (Mathf.Abs(start.y - end.y) < 0.1f)
        {
            //start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            return result;
        }
        else
        {
            //start and end are not level, gets more complicated
            Vector3 travelDirection = end - start;
            Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
            Vector3 up = Vector3.Cross(right, levelDirecteion);
            if (end.y > start.y) up = -up;
            Vector3 result = start + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            return result;
        }
    }

    public static float LerpWithOutClamp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    public static float InverseLerpWithOutClamp(float a, float b, float t)
    {
        if ((a - b) == 0) return 1;
        return (t - a) / Mathf.Abs(a - b);
    }

    public static float[] intersectLine(float start1, float start2, float end1, float end2)
    {
        List<float> arrayPointItrSect = new List<float>();
        if ((start1 - end1) * (start2 - end1) <= 0)
        {
            arrayPointItrSect.Add(end1);
        }
        if ((start1 - end2) * (start2 - end2) <= 0)
        {
            arrayPointItrSect.Add(end2);
        }
        return arrayPointItrSect.ToArray();
    }
}
public static class SpriteExtends
{
    public static Sprite loadSprite(string spite)
    {
        return null;
    }
}
public static class StringUtils
{
    public static string convertMoneyAndAddDot(long i)
    {

        string money = i.ToString();
        int pCounterDot = (money.Length - 1) / 3;
        if (i < 0)
        {
            pCounterDot = (money.Length - 2) / 3;
        }

        int pIndex = money.Length;
        string newStrMoney = "";
        while (pCounterDot > 0)
        {
            newStrMoney = "." + money.Substring(pIndex - 3, 3) + newStrMoney;
            pCounterDot--;
            pIndex -= 3;
        }
        newStrMoney = money.Substring(0, pIndex) + newStrMoney;
        return newStrMoney;
    }

    //support for 0 -> 999.999.999.999
    public static string convertMoneyAndAddText(long i)
    {
        string newString = "";
        long intOrigin = i;
        long intExcess = 0;
        int level = 0;
        if (intOrigin < 10000)
        {
            return convertMoneyAndAddDot(intOrigin);
        }
        while (intOrigin >= 1000)
        {
            intExcess = intOrigin % 1000;
            intOrigin /= 1000;
            level++;
        }
        newString += intOrigin.ToString();
        if (intExcess >= 10)
        {
            string stringExecess = ((long)(intExcess / 10)).ToString();
            newString += "." + (stringExecess.Length == 1 ? "0" + stringExecess : stringExecess);
        }
        switch (level)
        {
            case 1:
                newString += "K";
                break;
            case 2:
                newString += "M";
                break;
            case 3:
                newString += "B";
                break;
            default:
                return i.ToString();
        }
        return newString;
    }
    static CultureInfo elGR;
    public static string addDotMoney(long money)
    {
        if (money.ToString().Length < 4)
        {
            return money.ToString();
        }
        string pDefaultString = Convert.ToString(money);
        string result = "";
        int index = 0;
        for (int i = pDefaultString.Length - 1; i >= 0; --i)
        {
            index++;
            result = pDefaultString[i] + result;
            if (index == 3 && i != 0 &&  (i != 1 || pDefaultString[i] != '-' ))
            {
                result = "." + result;
                index = 0;
            }
        }
        return result;
    }
    public static string clearDot(this string v)
    {
        return v.Replace(".", string.Empty);
    }
    public static int convertToInt(this string v)
    {
        return int.Parse(v);
    }

    public static long convertToLong(this string v)
    {
        return long.Parse(v);
    }
    public static int convertStringDotToInt(this string v)
    {
        return v.clearDot().convertToInt();
    }
    public static string clearWhiteSpace(this string s)
    {
        for (int i = s.Length - 1; i >= 0; i--)
        {
            if (s.Substring(i, 1) == " ")
            {
                s = s.Remove(i, 1);
            }
        }
        return s;
    }
    public static string insertWhiteSpaceEvery(this string s, int pCount)
    {
        int index = 0;
        int count = 0;
        while (index < s.Length)
        {
            if (count == pCount)
            {
                s = s.Insert(index, " ");
                //index++;
                count = 0;
            }
            else
            {
                count++;
            }
            index++;
        }
        return s;
    }
    public static bool isHaveWhiteSpace(this string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (s.Substring(i, 1) == " ")
            {
                return true;
            }
        }
        return false;
    }

    public static bool startWithNumberic(this string s)
    {
        int n;
        return int.TryParse(s.Substring(0, 1), out n);
    }

    public static string FormartString(string format, params object[] objects)
    {
        return string.Format(format, objects);
    }
    public static string FormartString1(string format, int index)
    {
        return string.Format(format, index);
    }
}
public static class ActionCustomExtends
{
    public static EazyAction covertAction(this EazyActionInfo pAction)
    {
        EazyAction _mainAction = null;
        if (pAction.SelectedAction.name == EazyActionContructor.Sequences.name)
        {
            _mainAction = EazyCustomAction.Sequences.create();
            List<EazyAction> pListAction = new List<EazyAction>();
            for (int i = 0; i < pAction.ListActionInfo.Count; i++)
            {
                pListAction.Add(pAction.ListActionInfo[i].covertAction());
            }
            ((EazyCustomAction.Sequences)_mainAction)._listAction = pListAction.ToArray();
        }
        else
        {
            _mainAction = (EazyAction)Activator.CreateInstance(pAction.SelectedAction.MainType);
            if (_mainAction != null)
            {
                _mainAction.copyFromInfo(pAction);
            }
        }
        // Activator.CreateInstance
        //else if (pAction.SelectedAction >= ActionOption.MoveTo && pAction.SelectedAction <= ActionOption.ScaleBy)
        //{
        //    if (pAction.SelectedAction == ActionOption.MoveTo)
        //    {
        //        _mainAction = EazyMove.to(pAction.InfoStep.Vector3, pAction.Unit, pAction.UnitType == 0 ? true : false);
        //    }
        //    else if (pAction.SelectedAction == ActionOption.MoveBy)
        //    {
        //        _mainAction = EazyMove.by(pAction.InfoStep.Vector3, pAction.Unit, pAction.UnitType == 0 ? true : false);
        //    }
        //    else if (pAction.SelectedAction == ActionOption.RotateBy)
        //    {
        //        _mainAction = EazyRotate.by(pAction.InfoStep.Vector3, pAction.Unit, pAction.UnitType == 0 ? true : false);
        //    }
        //    else if (pAction.SelectedAction == ActionOption.RotateTo)
        //    {
        //        _mainAction = EazyRotate.to(pAction.InfoStep.Vector3, pAction.Unit, pAction.UnitType == 0 ? true : false);
        //    }
        //    else if (pAction.SelectedAction == ActionOption.ScaleBy)
        //    {
        //        _mainAction = EazyScale.by(pAction.InfoStep.Vector3, pAction.Unit, pAction.UnitType == 0 ? true : false);
        //    }
        //    else if (pAction.SelectedAction == ActionOption.ScaleTo)
        //    {
        //        _mainAction = EazyScale.to(pAction.InfoStep.Vector3, pAction.Unit, pAction.UnitType == 0 ? true : false);
        //    }
        //    if (pAction.TypeInfoIn != 0)
        //    {
        //        ((EazyVector3Action)_mainAction).setFrom(pAction.InfoFrom.Vector3);
        //    }
        //}
        //else if (pAction.SelectedAction == ActionOption.FadeBy)
        //{
        //    _mainAction = EazyFade.by(pAction.InfoStep.Color.a, pAction.Unit, pAction.UnitType == 0 ? true : false);
        //}
        //else if (pAction.SelectedAction == ActionOption.FadeTo)
        //{
        //    _mainAction = EazyFade.to(pAction.InfoStep.Color.a, pAction.Unit, pAction.UnitType == 0 ? true : false);
        //}
        //else if (pAction.SelectedAction == ActionOption.TintTo)
        //{
        //    _mainAction = EazyColor4F.to(pAction.InfoStep.Color, pAction.Unit, pAction.UnitType == 0 ? true : false);
        //}
        //if (pAction.EaseTypePopUp == 0)
        //{
        //    _mainAction.setEase(pAction.EaseType);
        //}
        //else
        //{
        //    _mainAction.Curve = pAction.CurveEase;
        //}
        //if (pAction.LoopType == 0)
        //{
        //    if (pAction.LoopTime > 1)
        //    {
        //        _mainAction.loop(pAction.LoopTime);
        //        //_mainAction = EazyRepeat.create(_mainAction, pAction.LoopTime);
        //    }
        //}
        if (_mainAction != null)
        {
            _mainAction.setName(pAction.Name);
        }
        return _mainAction;
    }


    public static bool checkExist<T>(T[] ts, T t) where T : class
    {
        for (int i = 0; i < ts.Length; ++i)
        {
            if (ts[i] == t)
            {
                return true;
            }
        }
        return false;
    }


}
public static class objExtend
{
    public static void CopyAllTo<T>(this T source, T target)
    {
        var type = typeof(T);
        foreach (var sourceProperty in type.GetProperties())
        {
            var targetProperty = type.GetProperty(sourceProperty.Name);
            targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
        }
        foreach (var sourceField in type.GetFields())
        {
            var targetField = type.GetField(sourceField.Name);
            targetField.SetValue(target, sourceField.GetValue(source));
        }
    }
}
public static class ArrayExtension
{
    public delegate void AddListCallBack<T>(T pObject);
    public static int ClosestTo(this IEnumerable<int> collection, int target)
    {
        // NB Method will return int.MaxValue for a sequence containing no elements.
        // Apply any defensive coding here as necessary.
        var closest = int.MaxValue;
        var minDifference = int.MaxValue;
        foreach (var element in collection)
        {
            var difference = Math.Abs((long)element - target);
            if (minDifference > difference)
            {
                minDifference = (int)difference;
                closest = element;
            }
        }

        return closest;
    }
    public static void addFromList<T>(this IList<T> list, T[] another, AddListCallBack<T> callback = null)
    {
        foreach (T element in another)
        {
            list.Add(element);
            if (callback != null)
            {
                callback(element);
            }
        }
    }

    public static bool checkExist<T>(this T[] vs, T e) where T : class
    {
        for (int i = 0; i < vs.Length; ++i)
        {
            if (vs[i] == e)
            {
                return true;
            }
        }
        return false;
    }

    public static int findIndex<T>(this T[] vs, T e) where T : class
    {
        for (int i = 0; i < vs.Length; ++i)
        {
            if (vs[i] == e)
            {
                return i;
            }
        }
        return -1;
    }

    public static int findIndex(this string[] vs, string v)
    {
        for (int i = 0; i < vs.Length; ++i)
        {
            if (v == vs[i])
            {
                return i;
            }
        }
        return 0;
    }
}
public static class GameObjectExtensions
{
    public static Type[] getAllComponentStringType(this GameObject gObj)
    {
        if (gObj)
        {
            var mObjs = gObj.GetComponents<Component>();
            Type[] mTypes = new Type[mObjs.Length];
            for (int i = 0; i < mObjs.Length; ++i)
            {
                mTypes[i] = mObjs[i].GetType();
            }
            return mTypes;
        }
        return null;
    }
    public static string[] convertStrings(this Type[] v)
    {
        string[] strs = new string[v.Length];
        for (int i = 0; i < v.Length; ++i)
        {
            strs[i] = v[i].ToString();
        }
        return strs;
    }
    /// <summary>
    /// Returns all monobehaviours (casted to T)
    /// </summary>
    /// <typeparam name="T">interface type</typeparam>
    /// <param name="gObj"></param>
    /// <returns></returns>
    public static T[] GetInterfaces<T>(this GameObject gObj)
    {
        if (!typeof(T).IsInterface) throw new System.SystemException("Specified type is not an interface!");
        var mObjs = gObj.GetComponents<MonoBehaviour>();

        return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
    }

    /// <summary>
    /// Returns the first monobehaviour that is of the interface type (casted to T)
    /// </summary>
    /// <typeparam name="T">Interface type</typeparam>
    /// <param name="gObj"></param>
    /// <returns></returns>
    public static T GetInterface<T>(this GameObject gObj)
    {
        if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
        return gObj.GetInterfaces<T>().FirstOrDefault();
    }

    /// <summary>
    /// Returns the first instance of the monobehaviour that is of the interface type T (casted to T)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gObj"></param>
    /// <returns></returns>
    public static T GetInterfaceInChildren<T>(this GameObject gObj)
    {
        if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
        return gObj.GetInterfacesInChildren<T>().FirstOrDefault();
    }

    /// <summary>
    /// Gets all monobehaviours in children that implement the interface of type T (casted to T)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gObj"></param>
    /// <returns></returns>
    public static T[] GetInterfacesInChildren<T>(this GameObject gObj)
    {
        if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");

        var mObjs = gObj.GetComponentsInChildren<MonoBehaviour>();

        return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
    }

    public static GameObject findInActiveObject(this List<GameObject> pList)
    {
        for (int i = 0; i < pList.Count; ++i)
        {
            if (!pList[i].activeSelf)
            {
                return pList[i];
            }
        }
        return null;
    }
}

[Serializable]
public struct Pos
{
    public int x;
    public int y;

    public override bool Equals(object other)
    {
        return this.x == ((Pos)other).x && this.y == ((Pos)other).y;
    }
    public override int GetHashCode()
    {
        return 0;
    }
    public Pos(int pX, int pY)
    {
        x = pX;
        y = pY;
    }
    public Vector2 vec2
    {
        get
        {
            return new Vector2(x, y);
        }
    }
    public static Pos Zero
    {
        get
        {
            Pos pZero;
            pZero.x = 0;
            pZero.y = 0;
            return pZero;
        }
    }


    public static Pos None
    {
        get
        {
            Pos pNone;
            pNone.x = -1;
            pNone.y = -1;
            return pNone;
        }
    }
    public static bool operator ==(Pos lhs, Pos rhs)
    {
        return lhs.Equals(rhs);
    }
    public static bool operator !=(Pos lhs, Pos rhs)
    {
        return !lhs.Equals(rhs);
    }
    public static Pos operator +(Pos lhs, Pos rhs)
    {
        return new Pos(lhs.x + rhs.x, lhs.y + rhs.y);
    }
    public static Pos operator *(Pos lhs, int pIndex)
    {
        return new Pos(lhs.x * pIndex, lhs.y * pIndex);
    }
    public static Pos operator *(int pIndex, Pos lhs)
    {
        return new Pos(lhs.x * pIndex, lhs.y * pIndex);
    }
    public static Pos operator -(Pos lhs, Pos rhs)
    {
        return new Pos(lhs.x - rhs.x, lhs.y - rhs.y);
    }
}
public class CacheTexture2D
{
    private List<Texture2D> _cacheTexture = new List<Texture2D>();
    public void addTexture(Texture2D pTexture)
    {
        _cacheTexture.Add(pTexture);
    }

    public Texture2D getTextureByName(string pName)
    {
        for (int i = 0; i < _cacheTexture.Count; i++)
        {
            if (_cacheTexture[i].name == pName)
            {
                Texture2D pTexture = _cacheTexture[i];
                return pTexture;
            }
        }
        return null;
    }
}
public static class TranformExtension
{
    public static void RotationLocalDirect(this Transform v, Vector2 direction)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        v.localRotation = rotation;
    }
    public static void RotationDirect(this Transform v, Vector2 direction)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        v.rotation = rotation;
    }

    public static void RotationDirect2D(this Transform v, Vector2 direction)
    {
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        v.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    public static void TranposeAroundLocal2D(this Transform v, Vector2 pivot, float angle)
    {
        Vector2 pPos = v.transform.localPosition;
        pPos = pPos.Rotate2DAround(pivot, Vector2.Distance(pivot, pPos), angle);
        v.transform.localPosition = v.transform.localPosition.insert(pPos);
    }
    public static void setLocalPosition2D(this Transform v, Vector2 pos)
    {

        v.transform.localPosition = v.transform.localPosition.insert(pos);
    }
    public static void setLocaPosX(this Transform v, float pX)
    {
        Vector3 pPos = v.transform.localPosition;
        pPos.x = pX;
        v.transform.localPosition = pPos;
    }
    public static void setLocaPosY(this Transform v, float pY)
    {
        Vector3 pPos = v.transform.localPosition;
        pPos.y = pY;
        v.transform.localPosition = pPos;
    }
    public static void setLocaPosZ(this Transform v, float pZ)
    {
        Vector3 pPos = v.transform.localPosition;
        pPos.z = pZ;
        v.transform.localPosition = pPos;
    }
}
public static class VectorExtension
{


    public static Vector3 insert(this Vector3 v, Vector2 v1)
    {
        v.x = v1.x;
        v.y = v1.y;
        return v;
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
    //public static float getRotate(this Vector2 v)
    //{
    //    var angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    //   return Quaternion.AngleAxis(angle, Vector3.forward);
    //}

    public static Pos ConvetPos(this Vector2 v)
    {
        return new Pos((int)v.x, (int)v.y);
    }

    public static Vector2 Rotate2DAround(this Vector2 v, Vector2 pivot, float radius, float angle)
    {
        float constX = (float)Math.Sin(angle * Math.PI / 180);
        float constY = (float)Math.Cos(angle * Math.PI / 180);
        Vector2 newPoint = new Vector2(constX, constY);
        newPoint = pivot + newPoint * radius;
        v.x = newPoint.x;
        v.y = newPoint.y;
        return v;
    }
    public static Vector3 changeDestinationByDirection(this Vector3 v, Vector3 to, float distance)
    {
        float ratio = distance / Vector3.Distance(v, to);
        Vector3 next = (to - v) * ratio;
        return next + v;
    }
}
public static class CacheBehaviorExtension
{


    public static bool pushBack<T>(this T[] v, T pObject) where T : class
    {
        bool pOut = true;
        for (int i = v.Length - 1; i >= 0; i--)
        {
            if (v[i] != null && i < v.Length - 1)
            {
                v[i + 1] = pObject;
                pOut = false;
                break;
            }
            if (i == 0)
            {
                v[0] = pObject;
                pOut = false;
            }
        }
        if (pOut)
        {
            Debug.Log("Out Of Range  pushback");
            return false;
        }
        return true;
    }

    public static bool resort<T>(this T[] v) where T : class
    {
        bool pResort = false;
        for (int i = 0; i < v.Length; i++)
        {
            if (v[i] == null)
            {
                for (int j = i + 1; j < v.Length; j++)
                {
                    if (v[j] != null)
                    {
                        v[i] = v[j];
                        v[j] = null;
                        pResort = true;
                        break;
                    }
                }
            }
        }
        return pResort;
    }


    public static bool removeAtIndex<T>(this T[] v, int pIndex, bool pResort = true) where T : class
    {
        v[pIndex] = null;
        if (pResort)
        {
            for (int i = pIndex; i < v.Length; i++)
            {
                if (i < v.Length - 1)
                {
                    v[i] = v[i + 1];
                }
            }
        }
        if (pIndex >= v.Length)
        {
            Debug.Log("Out Of Range  removeAtIndex");
            return false;
        }
        return true;
    }

    public static void clear<T>(this T[] v) where T : class
    {
        for (int i = 0; i < v.Length; i++)
        {
            v[i] = null;
        }
    }
    public static int LastObjectIndex<T>(this T[] v) where T : class
    {
        for (int i = v.Length - 1; i >= 0; i--)
        {
            if (v[i] != null)
            {
                return i;
            }
        }
        return -1;
    }
    public static int LengthObject<T>(this T[] v) where T : class
    {
        int pCount = 0;
        for (int i = 0; i < v.Length; i++)
        {
            if (v[i] != null)
            {
                pCount++;
            }
        }
        return pCount;
    }
    public static int Length<T>(this T[][] v)
    {
        int pCount = 0;
        for (int i = 0; i < v.Length; i++)
        {
            pCount += v[i].Length;
        }
        return pCount;
    }
    public static T at<T>(this T[][] v, int index)
    {
        if (v.Length > index && index >= 0)
        {
            int pIndex = 0;
            for (int i = 0; i < v.Length; i++)
            {
                for (int j = 0; j < v[i].Length; j++)
                {
                    if (pIndex == index)
                    {
                        return v[i][j];
                    }
                    pIndex++;
                }
            }
        }
        else
        {
            Debug.LogError("out of index array");
        }
        return default(T);
    }

    public static void scheduleUpdate(this CacheBehaviour v, UpdateObject pObjectUpdate)
    {
        ScheduleLayer pSche = v.Schedule;
        if (!pSche)
        {
            pSche = v.gameObject.AddComponent<ScheduleLayer>();
        }
        pSche.scheduleUpdate(pObjectUpdate);
    }


    public static Pos ConvetPos(this Vector2 v)
    {
        return new Pos((int)v.x, (int)v.y);
    }
}
public static class TimeExtension
{
    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }
}
public static class ColorExtension
{
    public static Color setAlpha(this Color color, float a)
    {
        color.a = a;
        return color;
    }
    public static Color converFloatToColorA(this float v)
    {
        return new Color(0, 0, 0, v);
    }

    public static Vector3 convertColortoVector3(this Color v)
    {
        return new Vector3(v.r, v.g, v.b);
    }

    public static Color coppyFromVector3(this Color v, Vector3 vec)
    {
        v = new Color(vec.x, vec.y, vec.z, v.a);
        return v;
    }
}
public static class CameraExtension
{
    public static Camera getCameraByName(string name)
    {
        Camera[] allCamera = Camera.allCameras;
        foreach (var cam in allCamera)
        {
            if (cam.name == name)
            {
                return cam;
            }
        }
        return null;
    }
}

public static class ResourceExtension
{
    public static Sprite LoadAtlas(string atlas, string spritename)
    {
        Sprite[] textures = Resources.LoadAll<Sprite>(atlas);
        return textures.Where(t => t.name == spritename).First<Sprite>();
    }
}

