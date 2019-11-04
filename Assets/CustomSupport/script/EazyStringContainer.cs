using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode]
public class EazyStringContainer : ScriptableObject
{

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    //static void OnBeforeSceneLoadRuntimeMethod()
    //{
    //    xmlString = EazyStringContainer.Load("eazystring");
    //}
    public static EazyStringContainer xmlString;

    public static EazyStringContainer getInstance()
    {
        if (xmlString == null)
        {
            xmlString = EazyStringContainer.Load("eazystring");
        }

        return xmlString;
    }

    [XmlArray("EazyStrings"), XmlArrayItem("EazyString")]
    public EazyStringXml[] eazyStrings;

    public EazyStringContainer()
    {
    }



    public string getStringByTag(string pTag)
    {
        string pString = "";
        if (eazyStrings != null)
        {
            foreach (var s in eazyStrings)
            {
                if (s._tag == pTag)
                {
                    pString = s._stringXml;
                    break;
                }
            }
        }
        return pString;
        //return I2.Loc.LocalizationManager.GetTranslation(pTag, false);
    }

    public static string getString(string pTag)
    {
        return EazyStringContainer.getInstance().getStringByTag(pTag);
    }

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(EazyStringContainer));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }


#if UNITY_EDITOR
    [MenuItem("Assets/Create/Eazy String")]
    public static void createAssetEazyString()
    {
        ScriptableObjectUtility.CreateAsset<EazyStringContainer>("Assets/Platformer2D/Resources", "eazystring");
    }
#endif
    public static EazyStringContainer Load(string path)
    {
        EazyStringContainer leveltaskContainer = Resources.Load<EazyStringContainer>(path);
        if (leveltaskContainer == null)
        {
#if UNITY_EDITOR
            leveltaskContainer = ScriptableObjectUtility.CreateAsset<EazyStringContainer>("Assets/Platformer2D/Resources", path);
#endif
        }
        return leveltaskContainer;
        // StreamWriter writer;
        //var serializer = new XmlSerializer(typeof(EazyStringContainer));
        //EazyStringContainer container = new EazyStringContainer();
        //FileStream file = null;
        ////}
        //if (File.Exists(path))
        //{
        //    file = new FileStream(path, FileMode.Open);
        //    return serializer.Deserialize(file) as EazyStringContainer;

        //}
        //else
        //{
        //    file = new FileStream(path, FileMode.Create);
        //    serializer.Serialize(file, container);
        //    return container;
        //}   
        //if(file == null)
        //{
        //    file = new FileStream(path, FileMode.CreateNew);
        //}

    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static EazyStringContainer LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(EazyStringContainer));
        return serializer.Deserialize(new StringReader(text)) as EazyStringContainer;
    }
}
