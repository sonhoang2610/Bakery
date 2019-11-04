using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyCustomAction {
    public class EazyAllType
    {

    }

    [Serializable]
    public struct EazyActionContructor
    {
        [SerializeField]
        EazyType mainType;
        [SerializeField]
        string mainComponent;
        public Type[] types;
        public string name;
        public bool enableTwoType;
        public bool enableLocal;

        public EazyActionContructor(string name, bool enableTwoType, bool enableLocal, params Type[] pType)
        {
            this.enableLocal = enableLocal;
            this.types = pType;
            this.name = name;
            this.enableTwoType = enableTwoType;
            mainComponent = "";
            mainType = null;
            if (name == "Sequences")
            {
                MainType = typeof(Sequences);
            }
        
        }

        public static EazyActionContructor Sequences
        {
            get
            {
                return new EazyActionContructor("Sequences", false,false, typeof(EazyAllType));
            }
        }

        public Type MainType
        {
            get
            {

                return mainType != null ? mainType.Type : (mainType = new EazyType()).Type;
            }

            set
            {
                if(mainType == null)
                {
                    mainType = new EazyType();
                }
                mainType.MainType = value.ToString();
                mainType.Assembly = value.Assembly.ToString();
            }
        }

        public string MainComponent
        {
            get
            {
                return mainComponent;
            }

            set
            {
                mainComponent = value;
            }
        }

    }

    public sealed class DrawEzActionBaseOn : Attribute
    {
        public Type _type;

        public DrawEzActionBaseOn(Type pType)
        {
            _type = pType;
        }

    }

    public sealed class CustomExtendDrawAction : Attribute
    {
        public Type _type;

        public CustomExtendDrawAction(Type pType)
        {
            _type = pType;
        }

    }
    public sealed class CustomExtendFieldAction : Attribute
    {

    }

    public sealed class EazyActionNew : Attribute
    {
        public EazyActionContructor contructorAction;
        public string name;

        public EazyActionNew(string name,bool enableTwoType,bool enableLocal ,params Type[] pType)
        {
            contructorAction = new EazyActionContructor(name, enableTwoType, enableLocal, pType);
        }
    }
}
