using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using EazyCustomAction;

namespace EazyEditor
{
    public static class EazyEditorTools 
    {
        public static T getCustomAttribute<T>(this Type v) where T : class
        {
            foreach (object obs in v.GetCustomAttributes(typeof(T), true))
            {
                T att = (T)obs;
                return att;
            }
            return null;
        }

        public static EazyActionContructor[] getAllEzAction(Type component) 
        {
            var assems = AppDomain.CurrentDomain.GetAssemblies();
            List<EazyActionContructor> actions = new List<EazyActionContructor>();
            foreach (Assembly a in assems)
            {
                foreach (Type t in a.GetTypes())
                {
                    object[] obs = t.GetCustomAttributes(typeof(EazyActionNew), true);
                    foreach (object ob in obs)
                    {
                        EazyActionNew att = (EazyActionNew)ob;
                        if (att.contructorAction.types.isExist(component) || att.contructorAction.types[0].ToString() == typeof(EazyAllType).ToString()) 
                        {
                            EazyActionContructor action = att.contructorAction;
                            action.MainType = t;
                            action.MainComponent = component.ToString();
                            actions.Add(action);
                        }
                    }
                }
            }
            return actions.ToArray();
        }

        public static Type getTypeActionBaseOn(Type action)
        {
            if (action != null)
            {
                object[] obs = action.GetCustomAttributes(typeof(DrawEzActionBaseOn), true);
                foreach (object ob in obs)
                {
                    DrawEzActionBaseOn att = (DrawEzActionBaseOn)ob;
                    return att._type;
                }
            }
            return null;
        }

        public static bool isExist(this Type[] types, Type type)
        {
            for(int i = 0; i < types.Length; ++i)
            {
                if(types[i] == type || type.IsSubclassOf(types[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static string[] convertStrings(this EazyActionContructor[] v)
        {
            if (v != null)
            {
                string[] strs = new string[v.Length];
                for (int i = 0; i < v.Length; ++i)
                {
                    strs[i] = v[i].name;
                }
                return strs;
            }
            return null; 
        }
        public static int findActionByName(this EazyActionContructor[] v, string name)
        {
            if (v != null)
            {
                for (int i = 0; i < v.Length; ++i)
                {
                    if (v[i].name == name)
                    {
                        return i;
                    }
                }
            }
            return 0;
        }


        //public static Type getAllTypeWithAttribute<T>()
        //{
        //    var assems = AppDomain.CurrentDomain.GetAssemblies();
        //    foreach (Assembly a in assems)
        //    {
        //        foreach (Type t in a.GetTypes())
        //        {
        //            foreach (object obs in t.GetCustomAttributes(typeof(T), true))
        //            {
        //                T att = (T)obs;
        //                if (att._type == pType)
        //                {
        //                    return t;
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}
    }
}
