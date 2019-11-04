using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EazyReflectionSupport;

namespace EazyCustomAction
{
    [Serializable]
    public class EazyType : ISerializationCallbackReceiver
    {
        public string mainType;
        public string assembly;
        private Type cache = null;

        public Type Type
        {
            get{ return cache != null ? cache : cache = this.convertToType(); }
            set {
                cache = value;
                if (cache != null)
                {
                    assembly = cache.Assembly.FullName;
                    mainType = cache.FullName;
                }
            }
        }

        public string MainType
        {
            get
            {
                return mainType;
            }

            set
            {
                if(mainType != value)
                {
                    Type newType = this.convertToType();
                    if(newType != null)
                    {
                        cache = newType;
                    }
                }
                mainType = value;
            }
        }

        public string Assembly
        {
            get
            {
                return assembly;
            }

            set
            {
                if (assembly != value)
                {
                    Type newType = this.convertToType();
                    if (newType != null)
                    {
                        cache = newType;
                    }
                }
                assembly = value;
            }
        }

        public void OnAfterDeserialize()
        {
            cache = this.convertToType();
        }

        public void OnBeforeSerialize()
        {

            if (cache != null)
            {
                assembly = cache.Assembly.FullName;
                mainType = cache.FullName;
            }
        }
    }
}
