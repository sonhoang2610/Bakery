using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    static Hashtable mainPool = new Hashtable();
    public static void add(Type pType, object pObject)
    {
        add(pType.ToString(), pObject);
    }
    public static void add<T>(object pObject)
    {
        add(typeof(T).ToString(), pObject);
    }

    public static object add(string tag, object pObject)
    {
        object pPool = getPool(tag);
        if (pPool == null)
        {
            pPool = new ArrayList();
            mainPool.Add(tag, pPool);
        }
        ((ArrayList)pPool).Add(pObject);
        return pObject;
    }

    static IList getPool(string tag)
    {
        return (IList)mainPool[tag];
    }
    public static T get<T>() where T : class
    {
        Type pType = typeof(T);
        IList array = (IList)getPool(pType.ToString());
        return (array != null && array.Count > 0) ? (T)array[0] : null;
    }



    public static T addMonobehavior<T>(T pObject) where T : MonoBehaviour
    {
        return (T)add(typeof(T).ToString(), pObject) ;
    }

    public static T getMonobehavior<T>() where T : MonoBehaviour 
    {
        return getMonobehavior<T>( typeof(T).ToString());
    }

    public static T addMonobehavior<T>(string tag, T pObject) where T : MonoBehaviour
    {
        return (T)add(tag, pObject);
    }

    public static T getMonobehavior<T>(string tag) where T : MonoBehaviour
    {
        IList pool = getPool(tag);
        if (pool == null || pool.Count == 0)
        {
            return null;
        }
        for (int i = 0; i < pool.Count; ++i)
        {
            if (!((T)pool[i]).gameObject.activeSelf)
            {
                return (T)pool[i];
            }
        }
        return null;
    }

    public static GameObject addGameObject(string tag, GameObject pObject)
    {
        return (GameObject)add(tag, pObject);
    }

    public static GameObject getGameObject(string tag) 
    {
        IList pool = getPool(tag);
        if (pool == null || pool.Count == 0)
        {
            return null;
        }
        for (int i = 0; i < pool.Count; ++i)
        {
            if (!((GameObject)pool[i]).activeSelf)
            {
                ((GameObject)pool[i]).SetActive(true);
                return (GameObject)pool[i];
            }
        }
        return null;
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void clearAll()
    {
        mainPool.Clear();
    }
}
