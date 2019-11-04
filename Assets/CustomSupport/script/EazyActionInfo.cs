using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace EazyCustomAction
{
    [System.Serializable]
    public class EazyActionWithTarget
    {
        [SerializeField]
        GameObject _target;
        [SerializeField]
        EazyActionInfo _info;
        public EazyActionWithTarget(EazyActionInfo info, GameObject target = null)
        {
            Target = target;
            Info = info;
        }

        public GameObject Target
        {
            get
            {
                return _target;
            }

            set
            {
                _target = value;
            }
        }

        public EazyActionInfo Info
        {
            get
            {
                return _info;
            }

            set
            {
                _info = value;
            }
        }

    }
    [System.Serializable]
    public class EazyActionInfoGroup 
    {
        [SerializeField]
        bool _isDefault;
        [SerializeField]
        string _name;
        [SerializeField]
        List<EazyActionWithTarget> _arrayAction;
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (value != _name)
                {
                    for (int i = 0; i < _arrayAction.Count; ++i)
                    {
                        _arrayAction[i].Info.Name = value;
                    }
                }
                _name = value;

            }
        }

        public List<EazyActionWithTarget> ArrayAction
        {
            get
            {
                return _arrayAction;
            }

            set
            {
                _arrayAction = value;
            }
        }

        public bool IsDefault
        {
            get
            {
                return _isDefault;
            }

            set
            {
                _isDefault = value;
            }
        }

        public void init()
        {
            ArrayAction = new List<EazyActionWithTarget>();
        }

        //private void OnEnable()
        //{

        //    hideFlags = HideFlags.HideAndDontSave;
        //}
    }

    [Serializable]
    public class EazyActionInfo 
    {

        //private void OnEnable()
        //{
        //    hideFlags = HideFlags.HideAndDontSave;
        //}
        public void init(EazyActionContructor select)
        {
            init();
            SelectedAction = select;
        }

        public void init()
        {
            //loopTime = 1;
            //isLocal = true;
            //listActionInfo = new List<EazyActionInfo>();
            infoFrom = new TypeInfoAction();
            InfoStep = new TypeInfoAction();
           SelectedAction = EazyActionContructor.Sequences;
            Name = "None";
            CurveEase = new AnimationCurve();
            CurveEase.AddKey(0, 0);
            CurveEase.AddKey(1, 1);
        }

        [SerializeField]
        private int id = 0, timeRepeat, easeTypePopUp = 0, loopType = 0, loopTime = 1, unitType = 0, typeInfoIn = 0, typeInfoOut = 0, layer = 0;
        [SerializeField]
        private float unit = 0;
        [SerializeField]
        private bool realTime = false,isEnableBy = false;
        [SerializeField]
        private bool isLocal = true;
        [SerializeField]
        private string name;
        [SerializeField]
        private EazyActionContructor selectedAction;
        [SerializeField]
        GameObject targetFrom;
        [SerializeField]
        public TypeInfoAction infoFrom;
        [SerializeField]
        GameObject targetStep;
        [SerializeField]
        public TypeInfoAction infoStep;
        [SerializeField]
        private EazyActionInfo actionParrent;
        [SerializeField]
        private AnimationCurve curveEase = null;
        [SerializeField]
        private EaseCustomType easeType = EaseCustomType.linear;
        [SerializeField]
        private List<EazyActionInfo> listActionInfo = new List<EazyActionInfo>();
        [SerializeField]
        UnityEvent onComplete;

        #region properties
        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public int TimeRepeat
        {
            get
            {
                return timeRepeat;
            }

            set
            {
                timeRepeat = value;
            }
        }

        public int EaseTypePopUp
        {
            get
            {
                return easeTypePopUp;
            }

            set
            {
                easeTypePopUp = value;
            }
        }

        public int LoopType
        {
            get
            {
                return loopType;
            }

            set
            {
                loopType = value;
            }
        }

        public int LoopTime
        {
            get
            {
                return loopTime;
            }

            set
            {
                loopTime = value;
            }
        }

        public int UnitType
        {
            get
            {
                return unitType;
            }

            set
            {
                unitType = value;
            }
        }

        public int TypeInfoIn
        {
            get
            {
                return typeInfoIn;
            }

            set
            {
                typeInfoIn = value;
            }
        }

        public int TypeInfoOut
        {
            get
            {
                return typeInfoOut;
            }

            set
            {
                typeInfoOut = value;
            }
        }

        public int Layer
        {
            get
            {
                return layer;
            }

            set
            {
                layer = value;
            }
        }

        public float Unit
        {
            get
            {
                return unit;
            }

            set
            {
                unit = value;
            }
        }

        public bool RealTime
        {
            get
            {
                return realTime;
            }

            set
            {
                realTime = value;
            }
        }

        public bool IsLocal
        {
            get
            {
                return isLocal;
            }

            set
            {
                isLocal = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public EazyActionContructor SelectedAction
        {
            get
            {
                return selectedAction;
            }

            set
            {
                selectedAction = value;
            }
        }

        public TypeInfoAction InfoFrom
        {
            get
            {
                return infoFrom;
            }

            set
            {
                infoFrom = value;
            }
        }

        public TypeInfoAction InfoStep
        {
            get
            {
                return infoStep;
            }

            set
            {
                infoStep = value;
            }
        }

        public EazyActionInfo ActionParrent
        {
            get
            {
                return actionParrent;
            }

            set
            {
                actionParrent = value;
            }
        }

        public AnimationCurve CurveEase
        {
            get
            {
                return curveEase;
            }

            set
            {
                curveEase = value;
            }
        }

        public EaseCustomType EaseType
        {
            get
            {
                return easeType;
            }

            set
            {
                easeType = value;
            }
        }

        public List<EazyActionInfo> ListActionInfo
        {
            get
            {
                return listActionInfo;
            }

            set
            {
                listActionInfo = value;
            }
        }

        public bool IsEnableBy
        {
            get
            {
                return isEnableBy;
            }

            set
            {
                isEnableBy = value;
            }
        }



        public GameObject TargetStep
        {
            get
            {
                return targetStep;
            }

            set
            {
                targetStep = value;
            }
        }

        public GameObject TargetFrom
        {
            get
            {
                return targetFrom;
            }

            set
            {
                targetFrom = value;
            }
        }
        #endregion

        //public void OnEnable()
        //{     
        //    if (listActionInfo == null)
        //    {
        //        loopTime = 1;
        //        isLocal = true;
        //        listActionInfo = new List<EazyActionInfo>();
        //    }

        //    hideFlags = HideFlags.HideAndDontSave;
        //}

        public EazyActionInfo[] getAllChilds()
        {
            List<EazyActionInfo> array = new List<EazyActionInfo>();
            array.addFromList(ListActionInfo.ToArray(), onaddListChild);
            for (int i = 0; i < ListActionInfo.Count; ++i)
            {
                array.addFromList(ListActionInfo[i].getAllChilds());
            }
            return array.ToArray();
        }

        public void  onaddListChild(EazyActionInfo action)
        {
            action.actionParrent = this;
        }

        public bool isChild(EazyActionInfo pParrent)
        {
            if (ActionParrent == pParrent)
            {
                return true;
            }
            if (ActionParrent != null && ActionParrent.isChild(pParrent))
            {
                return true;
            }
            return false;
        }

        public bool removeActionChild(EazyActionInfo pAction)
        {
            bool pRemove = ListActionInfo.Remove(pAction);
            if (!pRemove)
            {
                for (int i = ListActionInfo.Count - 1; i >= 0; i--)
                {
                    pRemove = ListActionInfo[i].removeActionChild(pAction);
                }
            }
            return pRemove;
        }


        public void editActionChild(int pID, EazyActionInfo pAction)
        {
            for (int i = ListActionInfo.Count - 1; i >= 0; i--)
            {
                if (ListActionInfo[i].Id == pID)
                {
                    ListActionInfo[i] = pAction;
                    return;
                }
            }
        }

        public void setID(int pID)
        {
            Id = pID;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

}
