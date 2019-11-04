using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;




[System.Serializable]
public class EazyStringXml
{
    [XmlAttribute("tag")]
    public string _tag;
    public string _stringDefault;
    [XmlAttribute("string")]
    public string _stringXml;
    [SerializeField]
    private bool _xml;

    public string ez_str
    {
        get
        {
            if (Xml)
            {
                return _stringXml = EazyStringContainer.getString(_tag);
            }
            else
            {
                return _stringDefault;
            }
        }
        set
        {
            if (!Xml)
            {
                _stringDefault = value;
            }
            else
            {
                _tag = value;
            }
        }
    }

    public bool Xml
    {
        get
        {
            return _xml;
        }

        set
        {
            _xml = value;
        }
    }
}

