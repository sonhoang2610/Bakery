using EazyCustomAction;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityToolbag;
using System;
using EazyReflectionSupport;




//[RequireComponent(typeof(Animator))]
public class CustomMoveTarget
{
    public GameObject _targetObject;
    public EazyAction _action;
    public CustomMoveTarget(EazyAction pAction, GameObject pObject)
    {
        _targetObject = pObject;
        _action = pAction;
    }
}

public enum TypeRoot { NONE = 0, ANIMATOR, ACTION }

//public static class RootMotionExtends
//{
//    public static void setCustomActive(this GameObject v, bool pBool)
//    {    
//        if (!pBool)
//        {
//            RootMotionController _root = v.GetComponent<RootMotionController>();
//            _root.stopAllAction();
//        }
//        v.SetActive(pBool);
//    }
//}


[ExecuteInEditMode]
[System.Serializable]
public class RootMotionController : CacheBehaviour
{
    //[ContextMenu("test1")]
    //public void test1()
    //{
    //    Debug.Log(infoTest.EaseType);
    //}
    //// public ActionIn 
    [SerializeField]
    bool _applyDefaultPos;
    [SerializeField]
    Vector3 defaultPos;
    [SerializeField]
    Vector3[] cachePos;
    [SerializeField]
    [HideInInspector]
    public List<EazyActionInfoGroup> _arrayAction;

    [HideInInspector]
    public Vector3 _offsetToAnimation;
    [HideInInspector]
    public TypeRoot enableRootMotion = TypeRoot.NONE;
    [HideInInspector]
    public TypeRoot _beforeRootType = TypeRoot.NONE;
    [HideInInspector]
    public GameObject _parrentMoveBy = null;
    [HideInInspector]
    public Vector3 _offsetParrent = Vector3.zero;
    private List<CustomMoveTarget> _listCustom;
    private Vector3 _lockPosition;
    private bool _isLock = false;
    public bool _isRunOnRealTime = false;
    public bool ignoreActive = false;

    public static RootMotionController _mainRoot;
    [HideInInspector]
    public float _rotateRoot = 0;
    [HideInInspector]
    public Vector3 _pointPivotPos = Vector3.zero;
    [HideInInspector]
    public Vector3 _pointPivotRotate = Vector3.zero;
    private UnityEvent _onLaterUpdate;
    public delegate void EventAnimator(AnimationEvent pParam);
    private EventAnimator _eventAnimator;


    public void setEventAnimator(EventAnimator pEvent)
    {
        _eventAnimator = pEvent;
    }
    public void addEventAnimator(EventAnimator pEvent)
    {
        _eventAnimator += pEvent;
    }
    public void onEventAnimator(AnimationEvent pParam)
    {
        if(_eventAnimator != null){
            _eventAnimator(pParam);
        }
    }
    [SerializeField]
    [HideInInspector]
    RootMotionController objectPreview;
    public void saveData()
    {
        ObjectPreview = Instantiate(gameObject, transform.parent).GetComponent<RootMotionController>();
    }

    public void resetData()
    {
        if (ObjectPreview)
        {
            DestroyImmediate(ObjectPreview.gameObject);
        }
    }

    public void setOnLateUpdate(UnityAction pOnlateUpdate)
    {
        _onLaterUpdate = null;
        if (_onLaterUpdate == null)
        {
            _onLaterUpdate = new UnityEvent();
        }      
        _onLaterUpdate.AddListener(pOnlateUpdate);
    }


    public bool IsLock
    {
        get
        {
            return _isLock;
        }

        set
        {
            _isLock = value;
            if (_isLock)
            {
                _lockPosition = transform.position;
            }
        }
    }

    public Vector3 DefaultPos
    {
        get
        {
            return defaultPos;
        }

        set
        {
            defaultPos = value;
        }
    }

    public RootMotionController ObjectPreview
    {
        get
        {
            return objectPreview;
        }

        set
        {
            objectPreview = value;
        }
    }

    public Vector3[] CachePos
    {
        get
        {
            return cachePos;
        }

        set
        {
            cachePos = value;
        }
    }

    public void resetDefaultPos()
    {
        transform.localPosition = defaultPos;
    }

    public void init()
    {
        
        if (_listCustom == null)
        {
            _listCustom = new List<CustomMoveTarget>();
        }
        _offsetToAnimation = Vector3.zero;
        if (animator)
        {
            //animator.applyRootMotion = false;
        }
    }


    public void reNew()
    {
        Awake();
        Start();
    }
    int indexIgnore = -1;
    private void Awake()
    {
        if (ignoreActive)
        {
           // indexIgnore = RootManager.Instance.addIgnoreUpdate(this);
        }
        init();
    }

    private void OnEnable()
    {
        if (_listCustom == null)
        {
            _listCustom = new List<CustomMoveTarget>();
        }
    }

    void Start()
    {
        resetData();
        if (_arrayAction != null)
        {
            for (int i = 0; i < _arrayAction.Count; ++i)
            {
                if (_arrayAction[i].IsDefault)
                {
                    runActionExist(_arrayAction[i].Name);
                }
            }
        }
    }

    public void stopAllAction(bool pBoolInCuleChild = false)
    {
        enableRootMotion = TypeRoot.NONE;
        if (_listCustom != null)
        {
            _listCustom.Clear();
            RootMotionController[] pChild = GetComponentsInChildren<RootMotionController>();
            for (int i = 0; i < pChild.Length; i++)
            {
                if (pChild[i] != this && pBoolInCuleChild)
                {
                    pChild[i].stopAllAction();
                }
            }
        }
    }
	
    public void stopAllActionWithTarget(GameObject pObject)
    {
        for (int i = _listCustom.Count - 1; i >= 0; i--)
        {
            if (_listCustom[i]._targetObject == pObject)
            {
                _listCustom.RemoveAt(i);
            }
        }
    }

    void stopActionIndex(int pIndex)
    {
        for (int i = _listCustom.Count - 1; i >= 0; i--)
        {
            if (_listCustom[i]._action._index == pIndex)
            {
                _listCustom.RemoveAt(pIndex);
            }
        }
    }

    //void OnAnimatorMove()
    //{
    //    Animator anim = GetComponent<Animator>();    
    //    if (anim && enableRootMotion == TypeRoot.NONE)
    //    {
    //        _offsetToAnimation = anim.rootPosition;
    //    }
    //}

    void  updateSkinAll()
    {
        //transform.localPosition = pInfo._pos;
        //transform.localScale = pInfo._scale;
        //transform.localRotation = Quaternion.Euler(pInfo._rotation);
        //if (TextUI)
        //{
        //    TextUI.color = pInfo._color;
        //}
        //if (this.renderer)
        //{
        //    this.SpriteRenderer.color = pInfo._color;
        //    renderer.sharedMaterial.SetColor("_Color", pInfo._color);
        //    renderer.sharedMaterial.color = pInfo._color;

        //}
        //if (pInfo.isChangeSprite && this.SpriteRenderer)
        //{
        //    this.SpriteRenderer.sprite = pInfo._sprite;
        //}
    }

    public void updateSkin(EazyAction pMove)
    {
       pMove.extendCallBack(gameObject);       
    }
    public static void stopAllAction(GameObject pObject)
    {
        if (pObject != null)
        {
            RootMotionController root = pObject.GetComponent<RootMotionController>();
            if (!root)
            {
                return;
            }
            root.stopAllAction();
        }
    }
    public static void runAction(GameObject pObject, params EazyAction[] pAction)
    {
        if (pObject != null)
        {
            RootMotionController root = pObject.GetComponent<RootMotionController>();
            if (!root)
            {
                root = pObject.AddComponent<RootMotionController>();
            }
            root.runAction(pAction);
        }
    }
    public void runActionByName(string name)
    {
        runActionExist(name, true);
    }

    public void runActionExist(string name,bool isStopAll = false)
    {
        for(int i = 0; i < _arrayAction.Count; ++i)
        {
            if(_arrayAction[i].Name == name)
            {
                EazyActionWithTarget[] actionInfos = _arrayAction[i].ArrayAction.ToArray();
                foreach(EazyActionWithTarget actionInfo in actionInfos)
                {
                    if (isStopAll)
                    {
                        if (actionInfo.Target.GetComponent<RootMotionController>())
                        {
                            actionInfo.Target.GetComponent<RootMotionController>().stopAllAction();
                        }
                    }
                    RootMotionController.runAction(actionInfo.Target, actionInfo.Info.covertAction());
                }
            }
        }
    }

    public void runActionPreview(params EazyAction[] pAction)
    {
        ObjectPreview.runAction(pAction);
    }

    public void runAction(params EazyAction[] pAction)
    {
        if(_listCustom == null)
        {
            init();
        }
        for (int i = 0; i < pAction.Length; i++)
        {
            _listCustom.Add(new CustomMoveTarget(pAction[i], null));
            pAction[i].setUpAction(this);
        }
        if (enableRootMotion != TypeRoot.ACTION)
        {
            _beforeRootType = enableRootMotion;
        }
        enableRootMotion = TypeRoot.ACTION;
    }

    //public void runAction(GameObject pObject, params EazyAction[] pAction)
    //{
    //    for (int i = 0; i < pAction.Length; i++)
    //    {
    //        EazyAction pMove = pAction[i];
    //        _listCustom.Add(new CustomMoveTarget(pMove, pObject));
    //        pMove.setUpAction(new InfoTranform(pObject));
    //    }
    //}
    public void stopActionByName(string pName)
    {
        foreach (CustomMoveTarget pObject in _listCustom)
        {
            if (pObject._action.getName() == pName)
            {
                pObject._action.IsStop = true;
            }
        }
    }

    public void pauseActionByName(string pName)
    {
        for(int i = _listCustom.Count -1; i >= 0; i--)
        {
            CustomMoveTarget pObject = _listCustom[i];
            if (pObject._action.getName() == pName)
            {
                pObject._action._pause = true;
            }
        }
    }

    public void resumeActionByName(string pName)
    {
        for (int i = _listCustom.Count - 1; i >= 0; i--)
        {
            CustomMoveTarget pObject = _listCustom[i];
            if (pObject._action.getName() == pName)
            {
                pObject._action._pause = false;
            }
        }
    }
    //void OnEnable()
    //{
    //    EditorApplication.update += Update;
    //}


    private void OnDisable()
    {
        if (!Application.isPlaying)
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
            resetData();
        }
    }

    public void EditorUpdate()
    {
        for (int i = _listCustom.Count - 1; i >= 0; i--)
        {
            EazyAction pAction = _listCustom[i]._action;
            if (!pAction.IsStop)
            {
                if (!pAction._pause)
                {
                    float pSec = pAction.apply(!_isRunOnRealTime ? (float)Time.deltaTime : Time.fixedUnscaledDeltaTime);
                    updateSkin(pAction);
                    if (pAction._evMoving != null)
                    {
                        pAction._evMoving(pAction._currentTime);
                    }
                    if (pSec >= 0)
                    {
                        if (pAction._endMove != null)
                        {
                            pAction._endMove();
                        }
                        _listCustom.RemoveAt(i);
                    }
                }
            }
            else
            {
                _listCustom.RemoveAt(i);
            }
        }
        if (_listCustom.Count == 0 && enableRootMotion == TypeRoot.ACTION)
        {
            enableRootMotion = _beforeRootType;
        }
    }


    public void Update()
    {
        if (Application.isPlaying)
        {
            for (int i = _listCustom.Count - 1; i >= 0; i--)
            {
                EazyAction pAction = _listCustom[i]._action;
                if (!pAction.IsStop)
                {
                    if (!pAction._pause)
                    {
                        float pSec = pAction.apply(!_isRunOnRealTime ? (float)Time.deltaTime : Time.fixedUnscaledDeltaTime);
                        updateSkin(pAction);
                        if (pAction._evMoving != null)
                        {
                            pAction._evMoving(pAction._currentTime);
                        }
                        if (pSec >= 0)
                        {
                            if (pAction._endMove != null)
                            {
                                pAction._endMove();
                            }
                            _listCustom.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    _listCustom.RemoveAt(i);
                }
            }
            if (_listCustom.Count == 0 && enableRootMotion == TypeRoot.ACTION)
            {
                enableRootMotion = _beforeRootType;
            }
        }
        else
        {
            if (!_applyDefaultPos)
            {
                DefaultPos = transform.localPosition;
            }
        }
    }

    void LateUpdate()
    {
        if (Application.isPlaying)
        {
            //    if (animator)
            //    {
            //        if (enableRootMotion == TypeRoot.NONE)
            //        {
            //            transform.position = _offsetToAnimation;
            //        }
            //        else if (enableRootMotion == TypeRoot.ANIMATOR)
            //        {
            //            transform.position = new Vector3(transform.localPosition.x + _offsetToAnimation.x, transform.localPosition.y + _offsetToAnimation.y, transform.position.z);
            //        }
            //        else
            //        {
            //            // transform.position = _infoTranformAction._pos;
            //        }
            //    }
            //    if (_parrentMoveBy)
            //    {
            //        _pointPivotPos = _parrentMoveBy.transform.position;
            //        _pointPivotRotate = _parrentMoveBy.transform.position;
            //        _rotateRoot = _parrentMoveBy.transform.localRotation.eulerAngles.z;
            //    }

            //    if (!IsLock)
            //    {
            //        transform.position += _pointPivotPos;
            //    }
            //    else
            //    {
            //        transform.position = _lockPosition + _pointPivotPos;
            //    }
            //    if (_parrentMoveBy)
            //    {
            //        _offsetParrent = _parrentMoveBy.transform.position;
            //    }
            //    if (_onLaterUpdate != null)
            //    {
            //        _onLaterUpdate.Invoke();
            //    }
        }
    }


    public bool isEnabled()
    {
        return !gameObject.activeSelf ? false : enabled;
    }
}
