using EazyCustomAction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using EazyReflectionSupport;


namespace EazyCustomAction
{

    // public enum ActionOption { MoveTo = 0, MoveBy, RotateTo, RotateBy, ScaleTo, ScaleBy, FadeTo, FadeBy, TintBy, TintTo, Sequence, None };
    public enum TypeActionEditorBehavior { None = 0, Add, Edit, Remove, Show, Close };
    public enum TypeAction { NONE = 0, POSITION, ROTATION, FADE, COLOR, SCALE, ANIMATION };
    public enum TypeBehaviorAction { TO = 1, BY = 2, TOandBY = 3, FROM = 4 };


    [Serializable]
    public class TypeInfoAction
    {

        [SerializeField]
        EazyMethodInfo methodInfo;
        [SerializeField]
        private Vector3 vector3;
        [SerializeField]
        private Color color;
        [SerializeField]
        private float @float;
        [SerializeField]
        bool isMethod;
        public Vector3 Vector3
        {
            get
            {
                if (IsMethod)
                {
                    methodInfo.getValue(ref vector3);
                }
                return vector3;
            }

            set
            {
                vector3 = value;
            }
        }

        public Color Color
        {
            get
            {
                if (IsMethod)
                {
                    methodInfo.getValue(ref color);
                }
                return color;
            }

            set
            {
                color = value;
            }
        }

        public float Float
        {
            get
            {
                if (IsMethod)
                {
                    methodInfo.getValue(ref @float);
                }
                return @float;
            }

            set
            {
                @float = value;
            }
        }

        public EazyMethodInfo MethodInfo
        {
            get
            {
                return methodInfo;
            }

            set
            {
                methodInfo = value;
                IsMethod = true;
            }
        }

        public bool IsMethod
        {
            get
            {
                return isMethod;
            }

            set
            {
                isMethod = value;
            }
        }
    }

    public enum EaseCustomType
    {
        easeInQuad,
        easeOutQuad,
        easeInOutQuad,
        easeInCubic,
        easeOutCubic,
        easeInOutCubic,
        easeInQuart,
        easeOutQuart,
        easeInOutQuart,
        easeInQuint,
        easeOutQuint,
        easeInOutQuint,
        easeInSine,
        easeOutSine,
        easeInOutSine,
        easeInExpo,
        easeOutExpo,
        easeInOutExpo,
        easeInCirc,
        easeOutCirc,
        easeInOutCirc,
        linear,
        spring,
        /* GFX47 MOD START */
        //bounce,
        easeInBounce,
        easeOutBounce,
        easeInOutBounce,
        /* GFX47 MOD END */
        easeInBack,
        easeOutBack,
        easeInOutBack,
        /* GFX47 MOD START */
        //elastic,
        easeInElastic,
        easeOutElastic,
        easeInOutElastic,
        /* GFX47 MOD END */
        punch
    }


    public enum MyCustomMoveType { NONE, CIRCLE }
    public delegate void EventCustomMove();
    public delegate void EventCustomMoving(float pSec);


    //public class InfoTranform : ICloneable
    //{
    //    public Vector3 _pos, _scale;
    //    public Vector3 _rotation;
    //    public Vector3 _velocity;
    //    public Color _color;
    //    public Sprite _sprite;
    //    public bool isChangeSprite;
    //    public InfoTranform(InfoTranform pInfo)
    //    {
    //        updateCurrentInfo(pInfo);
    //    }
    //    public InfoTranform(RootMotionController _root)
    //    {
    //        setInfo(_root);
    //    }
    //    public void setInfo(RootMotionController _root)
    //    {
    //        _pos = _root.transform.localPosition;
    //        _rotation = _root.transform.localRotation.eulerAngles;
    //        _scale = _root.transform.localScale;
    //        if (_root.SpriteRenderer)
    //        {
    //            _color = _root.SpriteRenderer.color;
    //        }
    //        else if (_root.renderer)
    //        {
    //            if (!_root.GetComponent<ParticleSystem>())
    //            {
    //                _color = _root.renderer.material.color;
    //            }
    //        }
    //        else if (_root.light)
    //        {
    //            _color = _root.light.color;
    //        }
    //        else if (_root.GetComponent(typeof(GUITexture)))
    //        {
    //            _color = _root.guiTexture.color;
    //        }
    //        else if (_root.GetComponent(typeof(GUIText)))
    //        {
    //            _color = _root.guiText.material.color;
    //        }

    //    }

    //    public void updateCurrentInfo(InfoTranform pInfo)
    //    {
    //        _pos = pInfo._pos;
    //        _rotation = pInfo._rotation;
    //        _scale = pInfo._scale;
    //        _color = pInfo._color;
    //    }

    //    public object Clone()
    //    {
    //        return this.MemberwiseClone();
    //    }

    //}

    public class EazyAction
    {
        private string _nameAction;
        public bool IsStop = false;
        public bool IsDone = false;
        protected bool _islocal = true;
        private int loop1 = 1;
        protected int _currentLoop = 1;
        public EventCustomMove _endMove = null;
        public EventCustomMoving _evMoving = null;
        public float _speed = 0;
        public float _time = 0;
        public float _unit = 0;
        public float _currentTime = 0;
        public bool _pause = false;
        public bool _calculByTime = true;
        public bool _isContinious = true;
        public int _index = -99999;
        //  public RootMotionController _root;
        public TypeAction _typeAction = TypeAction.NONE;
        public float _rotateRoot = 0;
        protected TypeBehaviorAction _typeActionBehavior = TypeBehaviorAction.TO;
        //protected Vector3 _pointPivotPos = Vector3.zero;
        //protected Vector3 _pointPivotRotate = Vector3.zero;
        protected delegate float EasingFunction(float start, float end, float Value);
        protected EasingFunction ease;
        private AnimationCurve curve;
        protected float percentage;
        protected bool reverse = false;
        protected List<EventHandlerAction> _eventOnUpdate = new List<EventHandlerAction>();
        protected EventHandlerAction _eventOnBegin;
        protected EventHandlerAction _eventOnEnd;

        public virtual void copyFromInfo(EazyActionInfo actionInfo)
        {
            _unit = actionInfo.Unit;
            _calculByTime = actionInfo.UnitType == 0 ? true : false;
            setLocal(actionInfo.IsLocal);
            if (actionInfo.LoopType == 0)
            {
                loop(actionInfo.LoopTime);
            }
            else
            {
                loop(-1);
            }
            if (actionInfo.EaseTypePopUp == 0)
            {
                setEase(actionInfo.EaseType);
            }
            else
            {
                Curve = actionInfo.CurveEase;
            }
        }

        public virtual void extendCallBack(GameObject pObject)
        {

        }

        public AnimationCurve Curve
        {
            get
            {
                return curve;
            }

            set
            {
                curve = value;
                ease = new EasingFunction(manual);
            }
        }

        public int Loop
        {
            get
            {
                return loop1;
            }

            set
            {
                loop1 = value;
            }
        }

        public EazyAction()
        {

        }
        public virtual void resetTime()
        {
            IsDone = false;
            _currentTime = 0;
            _currentLoop = Loop;
        }

        public EazyAction loop(int pLoop)
        {
            Loop = pLoop;
            _currentLoop = Loop;
            return this;
        }

        public virtual void addEvent(EventHandlerAction pEvent, RuntimeAction pRunTime, float pTime, bool includeChild = false)
        {
            pEvent.runAt(pRunTime, pTime);
            if (pEvent._timeRunType > RuntimeAction.BEGIN && pEvent._timeRunType < RuntimeAction.END)
            {
                _eventOnUpdate.Add(pEvent);

            }
            else if (pEvent._timeRunType == RuntimeAction.BEGIN)
            {
                _eventOnBegin = pEvent;
            }
            else
            {
                _eventOnEnd = pEvent;
            }
        }

        public virtual bool isLocal()
        {
            return _islocal;
        }

        public virtual EazyAction setLocal(bool pBool)
        {
            _islocal = pBool;
            return this;
        }
        public virtual float getTime()
        {
            return _time;
        }
        public virtual float apply(float pTime)
        {
            if (ease == null)
            {
                setEase(EaseCustomType.linear);
            }
            if (_currentLoop <= 0)
            {
                IsDone = true;
                return 0;
            }
            return -1;
        }
        protected void updateTime(float pTime)
        {
            _currentTime += pTime;
            if (getTime() == 0)
            {
                percentage = 1;
                excuteEvent(_eventOnBegin);
                excuteEvent(_eventOnEnd);
                return;
            }
            if (reverse)
            {
                percentage = 1 - _currentTime / getTime();
            }
            else
            {
                percentage = _currentTime / getTime();
            }
            if (percentage >= 1)
            {
                excuteEvent(_eventOnEnd);
            }
            else
            {
                if (_currentTime == pTime)
                {
                    excuteEvent(_eventOnBegin);
                }
                else
                {
                    for (int i = 0; i < _eventOnUpdate.Count; i++)
                    {
                        bool pAble = false;
                        _eventOnUpdate[i].curremtTime += pTime;
                        if (_eventOnUpdate[i]._timeRunType < RuntimeAction.UPDATE_ALLWAY)
                        {
                            if (_eventOnUpdate[i].curremtTime >= _eventOnUpdate[i].time)
                            {
                                _eventOnUpdate[i].curremtTime = 0;
                                pAble = true;
                            }
                        }
                        else if (_eventOnUpdate[i]._timeRunType > RuntimeAction.UPDATE_ALLWAY)
                        {
                            if (_eventOnUpdate[i].curremtTime >= _eventOnUpdate[i].time && _eventOnUpdate[i].curremtTime - pTime < _eventOnUpdate[i].time)
                            {
                                pAble = true;
                            }
                        }
                        if (_eventOnUpdate[i]._timeRunType == RuntimeAction.UPDATE_ALLWAY || pAble)
                        {
                            excuteEvent(_eventOnUpdate[i]);
                        }
                    }
                }
            }
        }
        public void excuteEvent(EventHandlerAction pEvent)
        {
            if (pEvent != null)
            {
                if (pEvent._conditioner == null || pEvent._conditioner())
                {
                    if (pEvent._eventSimple != null)
                    {
                        pEvent._eventSimple();
                    }
                    if (pEvent._event != null)
                    {
                        //pEvent._event(_root, this, _eventOnBegin);
                    }
                }
            }
        }
        public EazyAction setName(string pName) { _nameAction = pName; return this; }
        public string getName() { return _nameAction; }

        //public virtual object Clone()
        //{
        //    object pObject = this.MemberwiseClone();

        //    if (_infoRootClone != null)
        //    {
        //        ((EazyAction)pObject)._infoRootClone = new InfoTranform(_infoRootClone);
        //    }
        //    else if (_infoRoot != null && _infoRootClone == null)
        //    {
        //        ((EazyAction)pObject)._infoRootClone = new InfoTranform(_infoRoot);
        //    }
        //    return pObject;
        //}

        public virtual void setUpAction(RootMotionController pRoot)
        {
        }


        public virtual TypeBehaviorAction getTypeBehaviorAction()
        {
            return _typeActionBehavior;
        }

        public EazyAction setRotate(float pRotate, Vector3 pPosPivotRotate)
        {
            _rotateRoot = pRotate;
            return this;
        }

        public EazyAction setEase(EaseCustomType EaseCustomType)
        {
            switch (EaseCustomType)
            {
                case EaseCustomType.easeInQuad:
                    ease = new EasingFunction(easeInQuad);
                    break;
                case EaseCustomType.easeOutQuad:
                    ease = new EasingFunction(easeOutQuad);
                    break;
                case EaseCustomType.easeInOutQuad:
                    ease = new EasingFunction(easeInOutQuad);
                    break;
                case EaseCustomType.easeInCubic:
                    ease = new EasingFunction(easeInCubic);
                    break;
                case EaseCustomType.easeOutCubic:
                    ease = new EasingFunction(easeOutCubic);
                    break;
                case EaseCustomType.easeInOutCubic:
                    ease = new EasingFunction(easeInOutCubic);
                    break;
                case EaseCustomType.easeInQuart:
                    ease = new EasingFunction(easeInQuart);
                    break;
                case EaseCustomType.easeOutQuart:
                    ease = new EasingFunction(easeOutQuart);
                    break;
                case EaseCustomType.easeInOutQuart:
                    ease = new EasingFunction(easeInOutQuart);
                    break;
                case EaseCustomType.easeInQuint:
                    ease = new EasingFunction(easeInQuint);
                    break;
                case EaseCustomType.easeOutQuint:
                    ease = new EasingFunction(easeOutQuint);
                    break;
                case EaseCustomType.easeInOutQuint:
                    ease = new EasingFunction(easeInOutQuint);
                    break;
                case EaseCustomType.easeInSine:
                    ease = new EasingFunction(easeInSine);
                    break;
                case EaseCustomType.easeOutSine:
                    ease = new EasingFunction(easeOutSine);
                    break;
                case EaseCustomType.easeInOutSine:
                    ease = new EasingFunction(easeInOutSine);
                    break;
                case EaseCustomType.easeInExpo:
                    ease = new EasingFunction(easeInExpo);
                    break;
                case EaseCustomType.easeOutExpo:
                    ease = new EasingFunction(easeOutExpo);
                    break;
                case EaseCustomType.easeInOutExpo:
                    ease = new EasingFunction(easeInOutExpo);
                    break;
                case EaseCustomType.easeInCirc:
                    ease = new EasingFunction(easeInCirc);
                    break;
                case EaseCustomType.easeOutCirc:
                    ease = new EasingFunction(easeOutCirc);
                    break;
                case EaseCustomType.easeInOutCirc:
                    ease = new EasingFunction(easeInOutCirc);
                    break;
                case EaseCustomType.linear:
                    ease = new EasingFunction(linear);
                    break;
                case EaseCustomType.spring:
                    ease = new EasingFunction(spring);
                    break;
                /* GFX47 MOD START */
                /*case EaseCustomType.bounce:
                    ease = new EasingFunction(bounce);
                    break;*/
                case EaseCustomType.easeInBounce:
                    ease = new EasingFunction(easeInBounce);
                    break;
                case EaseCustomType.easeOutBounce:
                    ease = new EasingFunction(easeOutBounce);
                    break;
                case EaseCustomType.easeInOutBounce:
                    ease = new EasingFunction(easeInOutBounce);
                    break;
                /* GFX47 MOD END */
                case EaseCustomType.easeInBack:
                    ease = new EasingFunction(easeInBack);
                    break;
                case EaseCustomType.easeOutBack:
                    ease = new EasingFunction(easeOutBack);
                    break;
                case EaseCustomType.easeInOutBack:
                    ease = new EasingFunction(easeInOutBack);
                    break;
                /* GFX47 MOD START */
                /*case EaseCustomType.elastic:
                    ease = new EasingFunction(elastic);
                    break;*/
                case EaseCustomType.easeInElastic:
                    ease = new EasingFunction(easeInElastic);
                    break;
                case EaseCustomType.easeOutElastic:
                    ease = new EasingFunction(easeOutElastic);
                    break;
                case EaseCustomType.easeInOutElastic:
                    ease = new EasingFunction(easeInOutElastic);
                    break;
                    /* GFX47 MOD END */
            }
            return this;
        }

        #region Easing Curves

        private float manual(float start, float end, float value)
        {
            float current = start + (Curve.Evaluate(value) * (end - start));
            return current;
        }
        private float linear(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, value);
        }

        protected float clerp(float start, float end, float value)
        {
            float min = 0.0f;
            float max = 360.0f;
            float half = Mathf.Abs((max - min) * 0.5f);
            float retval = 0.0f;
            float diff = 0.0f;
            if ((end - start) < -half)
            {
                diff = ((max - start) + end) * value;
                retval = start + diff;
            }
            else if ((end - start) > half)
            {
                diff = -((max - end) + start) * value;
                retval = start + diff;
            }
            else retval = start + (end - start) * value;
            return retval;
        }

        private float spring(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
            return start + (end - start) * value;
        }

        private float easeInQuad(float start, float end, float value)
        {
            end -= start;
            return end * value * value + start;
        }

        private float easeOutQuad(float start, float end, float value)
        {
            end -= start;
            return -end * value * (value - 2) + start;
        }

        private float easeInOutQuad(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * value * value + start;
            value--;
            return -end * 0.5f * (value * (value - 2) - 1) + start;
        }

        private float easeInCubic(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value + start;
        }

        private float easeOutCubic(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value + 1) + start;
        }

        private float easeInOutCubic(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * value * value * value + start;
            value -= 2;
            return end * 0.5f * (value * value * value + 2) + start;
        }

        private float easeInQuart(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value + start;
        }

        private float easeOutQuart(float start, float end, float value)
        {
            value--;
            end -= start;
            return -end * (value * value * value * value - 1) + start;
        }

        private float easeInOutQuart(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * value * value * value * value + start;
            value -= 2;
            return -end * 0.5f * (value * value * value * value - 2) + start;
        }

        private float easeInQuint(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value * value + start;
        }

        private float easeOutQuint(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value * value * value + 1) + start;
        }

        private float easeInOutQuint(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * value * value * value * value * value + start;
            value -= 2;
            return end * 0.5f * (value * value * value * value * value + 2) + start;
        }

        private float easeInSine(float start, float end, float value)
        {
            end -= start;
            return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
        }

        private float easeOutSine(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
        }

        private float easeInOutSine(float start, float end, float value)
        {
            end -= start;
            return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
        }

        private float easeInExpo(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (value - 1)) + start;
        }

        private float easeOutExpo(float start, float end, float value)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * value) + 1) + start;
        }

        private float easeInOutExpo(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
            value--;
            return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
        }

        private float easeInCirc(float start, float end, float value)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
        }

        private float easeOutCirc(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * Mathf.Sqrt(1 - value * value) + start;
        }

        private float easeInOutCirc(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
            value -= 2;
            return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
        }

        /* GFX47 MOD START */
        private float easeInBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            return end - easeOutBounce(0, end, d - value) + start;
        }
        /* GFX47 MOD END */

        /* GFX47 MOD START */
        //private float bounce(float start, float end, float value){
        private float easeOutBounce(float start, float end, float value)
        {
            value /= 1f;
            end -= start;
            if (value < (1 / 2.75f))
            {
                return end * (7.5625f * value * value) + start;
            }
            else if (value < (2 / 2.75f))
            {
                value -= (1.5f / 2.75f);
                return end * (7.5625f * (value) * value + .75f) + start;
            }
            else if (value < (2.5 / 2.75))
            {
                value -= (2.25f / 2.75f);
                return end * (7.5625f * (value) * value + .9375f) + start;
            }
            else
            {
                value -= (2.625f / 2.75f);
                return end * (7.5625f * (value) * value + .984375f) + start;
            }
        }
        /* GFX47 MOD END */

        /* GFX47 MOD START */
        private float easeInOutBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            if (value < d * 0.5f) return easeInBounce(0, end, value * 2) * 0.5f + start;
            else return easeOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
        }
        /* GFX47 MOD END */

        private float easeInBack(float start, float end, float value)
        {
            end -= start;
            value /= 1;
            float s = 1.70158f;
            return end * (value) * value * ((s + 1) * value - s) + start;
        }

        private float easeOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value = (value) - 1;
            return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
        }

        private float easeInOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value /= .5f;
            if ((value) < 1)
            {
                s *= (1.525f);
                return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
            }
            value -= 2;
            s *= (1.525f);
            return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
        }

        private float punch(float amplitude, float value)
        {
            float s = 9;
            if (value == 0)
            {
                return 0;
            }
            else if (value == 1)
            {
                return 0;
            }
            float period = 1 * 0.3f;
            s = period / (2 * Mathf.PI) * Mathf.Asin(0);
            return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
        }

        /* GFX47 MOD START */
        private float easeInElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d) == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        }
        /* GFX47 MOD END */

        /* GFX47 MOD START */
        //private float elastic(float start, float end, float value){
        private float easeOutElastic(float start, float end, float value)
        {
            /* GFX47 MOD END */
            //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d) == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
        }

        /* GFX47 MOD START */
        private float easeInOutElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d * 0.5f) == 2) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
            return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
        }
        /* GFX47 MOD END */

        #endregion

    }



    [DrawEzActionBaseOn(typeof(Vector3))]
    public class EazyVector3Action : EazyAction
    {
        protected Vector3 _from;
        protected Vector3 _desnity;
        protected Vector3 _current;
        protected Vector3 _nextMove;
        public EazyMethodInfo _getDestiny, _getFrom;

        //       protected delegate void setVector3Method(Vector3 pVec, bool pEnd);
        //protected getVector3Method _getVector3Method;
        //protected setVector3Method _setVector3Method;

        public override void copyFromInfo(EazyActionInfo actionInfo)
        {
            _getFrom = actionInfo.infoFrom.MethodInfo;
            _getDestiny = actionInfo.InfoStep.MethodInfo;
            _from = actionInfo.infoFrom.Vector3;
            if (actionInfo.IsEnableBy)
            {
                _nextMove = actionInfo.InfoStep.Vector3;
                _typeActionBehavior = TypeBehaviorAction.BY;
            }
            else
            {
                _desnity = actionInfo.InfoStep.Vector3;
                _typeActionBehavior = TypeBehaviorAction.TO;
            }
            if (actionInfo.TypeInfoIn != 0)
            {
                _typeActionBehavior += (int)TypeBehaviorAction.FROM;
            }
            base.copyFromInfo(actionInfo);
        }

        public void setFrom(EazyMethodInfo pFrom)
        {
            _typeActionBehavior += (int)TypeBehaviorAction.FROM;
            _isContinious = false;
            _getFrom = pFrom;
        }
        public void setFrom(Vector3 pFrom)
        {
            _typeActionBehavior += (int)TypeBehaviorAction.FROM;
            _isContinious = false;
            _from = pFrom;
        }
        protected void setTo(EazyMethodInfo pDestiny, float pUnit, bool calculByTime = true)
        {
            _unit = pUnit;
            _getDestiny = pDestiny;
            _calculByTime = calculByTime;
        }
        protected void setTo(Vector3 pDestiny, float pUnit, bool calculByTime = true)
        {
            _unit = pUnit;
            _desnity = pDestiny;
            _calculByTime = calculByTime;
        }
        protected void setBy(Vector3 pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyVector3Action pMove = this;
            pMove._unit = pUnit;
            pMove._nextMove = pMoveBy;
            pMove._calculByTime = calculByTime;
        }
        protected void setBy(EazyMethodInfo pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyVector3Action pMove = this;
            pMove._unit = pUnit;
            pMove._getDestiny = pMoveBy;
            pMove._calculByTime = calculByTime;
        }
        // Update is called once per frame
        public override float apply(float pTime)
        {
            float pSec = base.apply(pTime);
            if (pSec >= 0)
            {
                return pSec;
            }
            updateTime(pTime);
            float pPercent = percentage;
            if (pPercent > 1)
            {
                pPercent = 1;
            }
            _current.x = ease(_from.x, _desnity.x, pPercent);
            _current.y = ease(_from.y, _desnity.y, pPercent);
            _current.z = ease(_from.z, _desnity.z, pPercent);
            if (percentage >= 1)
            {
                if (_currentLoop > 0)
                {
                    _currentLoop--;
                    if (_currentLoop <= 0)
                    {
                        IsDone = true;
                        return 0;
                    }
                    if (getTypeBehaviorAction() < TypeBehaviorAction.FROM)
                    {
                        _from = _current;
                        setUpAction(null);
                    }
                    int templeLoop = _currentLoop;
                    resetTime();
                    _currentLoop = templeLoop;
                    return -1;
                }
                return 0;
            }


            return -1;
        }

        public override void resetTime()
        {
            base.resetTime();
            _current = _from;
        }

        public override void setUpAction(RootMotionController pRoot)
        {
            base.setUpAction(pRoot);
            TypeBehaviorAction pTypeBehaviorTo = _typeActionBehavior;
            if ((int)_typeActionBehavior < (int)TypeBehaviorAction.FROM)
            {
            }
            else
            {
                pTypeBehaviorTo = _typeActionBehavior - (int)TypeBehaviorAction.FROM;
            }
            if (_getFrom.typeReflection != TypeReflection.None)
            {
                _getFrom.getValue(ref _from);
            }
            if (_getDestiny.typeReflection != TypeReflection.None)
            {
                _getDestiny.getValue(ref _desnity);
            }
            if (pTypeBehaviorTo == TypeBehaviorAction.BY)
            {
                _desnity = _from + _nextMove;
            }
            float pDistance = Vector3.Distance(_desnity, _from);
            _speed = !_calculByTime ? _unit : pDistance / _unit;
            _time = _calculByTime ? _unit : pDistance / _unit;
            _current = _from;
        }

    }

    [EazyActionNew("EazyMove", true, true, typeof(Transform))]
    public class EazyMove : EazyVector3Action
    {

        public EazyMove()
        {
        }
        public EazyMove(TypeBehaviorAction pBehavior) : this()
        {
            _typeActionBehavior = pBehavior;
        }


        public EazyMove from(EazyMethodInfo pFrom)
        {
            base.setFrom(pFrom);
            return this;
        }

        public EazyMove from(Vector3 pFrom)
        {
            base.setFrom(pFrom);
            return this;
        }

        public static EazyMove to(EazyMethodInfo pDestiny, float pUnit, bool calculByTime = true)
        {
            EazyMove pMove = new EazyMove(TypeBehaviorAction.TO);
            pMove.setTo(pDestiny, pUnit, calculByTime);
            return pMove;
        }
        public static EazyMove to(Vector3 pDestiny, float pUnit, bool calculByTime = true)
        {

            EazyMove pMove = new EazyMove(TypeBehaviorAction.TO);
            pMove.setTo(pDestiny, pUnit, calculByTime);
            return pMove;
        }
        public static EazyMove by(Vector3 pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyMove pMove = new EazyMove(TypeBehaviorAction.BY);
            pMove.setBy(pMoveBy, pUnit, calculByTime);
            return pMove;
        }
        public static EazyMove by(EazyMethodInfo pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyMove pMove = new EazyMove(TypeBehaviorAction.BY);
            pMove.setBy(pMoveBy, pUnit, calculByTime);
            return pMove;
        }

        public override void extendCallBack(GameObject pObject)
        {
            if (isLocal())
            {
                pObject.transform.localPosition = _current;
            }
            else
            {
                pObject.transform.position = _current;

            }
        }

        public override void setUpAction(RootMotionController pRoot)
        {
            if (pRoot)
            {
                if (_typeActionBehavior < TypeBehaviorAction.FROM)
                {
                    if (isLocal())
                    {
                        _from = pRoot.transform.localPosition;
                    }
                    else
                    {
                        _from = pRoot.transform.position;
                    }
                }
            }
            base.setUpAction(pRoot);
        }
    }

    [EazyActionNew("EazyRotate", true, true, typeof(Transform))]
    public class EazyRotate : EazyVector3Action
    {

        public EazyRotate()
        {
        }
        public EazyRotate(TypeBehaviorAction pBehavior) : this()
        {
            _typeActionBehavior = pBehavior;
        }

        public override void extendCallBack(GameObject pObject)
        {
            if (isLocal())
            {
                pObject.transform.localRotation = Quaternion.Euler(_current);
            }
            else
            {
                pObject.transform.rotation = Quaternion.Euler(_current);
            }
        }

        public EazyRotate from(EazyMethodInfo pFrom)
        {
            base.setFrom(pFrom);
            return this;
        }

        public EazyRotate from(Vector3 pFrom)
        {
            base.setFrom(pFrom);
            return this;
        }

        public static EazyRotate to(EazyMethodInfo pDestiny, float pUnit, bool calculByTime = true)
        {
            EazyRotate pMove = new EazyRotate(TypeBehaviorAction.TO);
            pMove.setTo(pDestiny, pUnit, calculByTime);
            return pMove;
        }
        public static EazyRotate to(Vector3 pDestiny, float pUnit, bool calculByTime = true)
        {

            EazyRotate pMove = new EazyRotate(TypeBehaviorAction.TO);
            pMove.setTo(pDestiny, pUnit, calculByTime);
            return pMove;
        }
        public static EazyRotate by(Vector3 pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyRotate pMove = new EazyRotate(TypeBehaviorAction.BY);
            pMove.setBy(pMoveBy, pUnit, calculByTime);
            return pMove;
        }
        public static EazyRotate by(EazyMethodInfo pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyRotate pMove = new EazyRotate(TypeBehaviorAction.BY);
            pMove.setBy(pMoveBy, pUnit, calculByTime);
            return pMove;
        }
        // Update is called once per frame
        public override void setUpAction(RootMotionController pRoot)
        {
            if (pRoot)
            {
                if (getTypeBehaviorAction() < TypeBehaviorAction.FROM)
                {
                    if (isLocal())
                    {
                        _from = pRoot.transform.localRotation.eulerAngles;
                    }
                    else
                    {
                        _from = pRoot.transform.rotation.eulerAngles;
                    }
                }
            }
            base.setUpAction(pRoot);
        }
    }
    [EazyActionNew("EazyColor3F", true, false, typeof(SpriteRenderer), typeof(Renderer), typeof(Light))]
    public class EazyColor3F : EazyVector3Action
    {
        SpriteRenderer spriteRenderer;
        Renderer renderer;
        Light light;
        public EazyColor3F()
        {
        }
        public EazyColor3F(TypeBehaviorAction pBehavior) : this()
        {
            _typeActionBehavior = pBehavior;
        }

        public EazyColor3F from(EazyMethodInfo pFrom)
        {
            base.setFrom(pFrom);
            return this;
        }

        public EazyColor3F from(Vector3 pFrom)
        {
            base.setFrom(pFrom);
            return this;
        }

        public static EazyColor3F to(EazyMethodInfo pDestiny, float pUnit, bool calculByTime = true)
        {
            EazyColor3F pMove = new EazyColor3F(TypeBehaviorAction.TO);
            pMove.setTo(pDestiny, pUnit, calculByTime);
            return pMove;
        }
        public static EazyColor3F to(Vector3 pDestiny, float pUnit, bool calculByTime = true)
        {

            EazyColor3F pMove = new EazyColor3F(TypeBehaviorAction.TO);
            pMove.setTo(pDestiny, pUnit, calculByTime);
            return pMove;
        }
        public static EazyColor3F by(Vector3 pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyColor3F pMove = new EazyColor3F(TypeBehaviorAction.BY);
            pMove.setBy(pMoveBy, pUnit, calculByTime);
            return pMove;
        }
        public static EazyColor3F by(EazyMethodInfo pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyColor3F pMove = new EazyColor3F(TypeBehaviorAction.BY);
            pMove.setBy(pMoveBy, pUnit, calculByTime);
            return pMove;
        }

        public override void extendCallBack(GameObject pObject)
        {
            if (spriteRenderer)
            {
                spriteRenderer.color = spriteRenderer.color.coppyFromVector3(_current);
            }
            else if (renderer)
            {
                renderer.material.color = renderer.material.color.coppyFromVector3(_current);
            }
            else if (light)
            {
                light.color = light.color.coppyFromVector3(_current);
            }
        }

        // Update is called once per frame
        public override void setUpAction(RootMotionController pRoot)
        {
            if (pRoot)
            {
                if (!spriteRenderer)
                {
                    spriteRenderer = pRoot.GetComponent<SpriteRenderer>();
                }
                else if (!renderer)
                {
                    renderer = pRoot.GetComponent<Renderer>();
                }
                else if (!light)
                {
                    light = pRoot.GetComponent<Light>();
                }
            }
            if (_typeActionBehavior < TypeBehaviorAction.FROM)
            {
                if (spriteRenderer)
                {
                    _from = spriteRenderer.color.convertColortoVector3();
                }
                else if (renderer)
                {
                    _from = renderer.material.color.convertColortoVector3();
                }
                else if (light)
                {
                    _from = light.color.convertColortoVector3();
                }
            }
            base.setUpAction(pRoot);
        }
    }
    [EazyActionNew("EazyScale", true, false, typeof(Transform))]
    public class EazyScale : EazyVector3Action
    {

        public EazyScale()
        {
        }
        public EazyScale(TypeBehaviorAction pBehavior) : this()
        {
            _typeActionBehavior = pBehavior;
        }

        public EazyScale from(EazyMethodInfo pFrom)
        {
            base.setFrom(pFrom);
            return this;
        }

        public EazyScale from(Vector3 pFrom)
        {
            base.setFrom(pFrom);
            return this;
        }

        public static EazyScale to(EazyMethodInfo pDestiny, float pUnit, bool calculByTime = true)
        {
            EazyScale pMove = new EazyScale(TypeBehaviorAction.TO);
            pMove.setTo(pDestiny, pUnit, calculByTime);
            return pMove;
        }
        public static EazyScale to(Vector3 pDestiny, float pUnit, bool calculByTime = true)
        {

            EazyScale pMove = new EazyScale(TypeBehaviorAction.TO);
            pMove.setTo(pDestiny, pUnit, calculByTime);
            return pMove;
        }
        public static EazyScale by(Vector3 pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyScale pMove = new EazyScale(TypeBehaviorAction.BY);
            pMove.setBy(pMoveBy, pUnit, calculByTime);
            return pMove;
        }
        public static EazyScale by(EazyMethodInfo pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyScale pMove = new EazyScale(TypeBehaviorAction.BY);
            pMove.setBy(pMoveBy, pUnit, calculByTime);
            return pMove;
        }
        public override void extendCallBack(GameObject pObject)
        {
            pObject.transform.localScale = _current;
        }

        public override void setUpAction(RootMotionController pRoot)
        {
            if (pRoot)
            {
                if (_typeActionBehavior < TypeBehaviorAction.FROM)
                {
                    _from = pRoot.transform.localScale;
                }
            }
            base.setUpAction(pRoot);
        }
    }
    [DrawEzActionBaseOn(typeof(Color))]
    [EazyActionNew("EazyColor4F", true, false, typeof(SpriteRenderer), typeof(Renderer), typeof(Light))]
    public class EazyColor4F : EazyAction
    {
        protected Color _current;
        protected Color _from;
        protected Color _desnity;
        protected Color _nextMove;
        protected Transform _trans;
        public delegate Color ColorMethod();
        public EazyMethodInfo _getDestiny, _getFrom;

        SpriteRenderer spriteRenderer;
        Renderer renderer;
        Light light;


        public override void copyFromInfo(EazyActionInfo actionInfo)
        {
            _from = actionInfo.infoFrom.Color;
            if (actionInfo.IsEnableBy)
            {
                _nextMove = actionInfo.InfoStep.Color;
                _typeActionBehavior = TypeBehaviorAction.BY;
            }
            else
            {
                _desnity = actionInfo.InfoStep.Color;
                _typeActionBehavior = TypeBehaviorAction.TO;
            }
            if (actionInfo.TypeInfoIn != 0)
            {
                _typeActionBehavior += (int)TypeBehaviorAction.FROM;
            }
            base.copyFromInfo(actionInfo);
        }

        public EazyColor4F()
        {
        }
        public EazyColor4F(TypeBehaviorAction pBehavior) : this()
        {
            _typeActionBehavior = pBehavior;
        }


        public EazyColor4F from(Color pFrom)
        {
            setFrom(pFrom);
            return this;
        }

        public EazyColor4F from(EazyMethodInfo pFrom)
        {
            setFrom(pFrom);
            return this;
        }

        public static EazyColor4F to(EazyMethodInfo pDestiny, float pUnit, bool calculByTime = true)
        {
            EazyColor4F pMove = new EazyColor4F(TypeBehaviorAction.TO);
            pMove.setTo(pDestiny, pUnit, calculByTime);
            return pMove;
        }
        public static EazyColor4F to(Color pDestiny, float pUnit, bool calculByTime = true)
        {

            EazyColor4F pMove = new EazyColor4F(TypeBehaviorAction.TO);
            pMove.setTo(pDestiny, pUnit, calculByTime);
            return pMove;
        }
        public static EazyColor4F by(Color pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyColor4F pMove = new EazyColor4F(TypeBehaviorAction.BY);
            pMove.setBy(pMoveBy, pUnit, calculByTime);
            return pMove;
        }
        public static EazyColor4F by(EazyMethodInfo pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyColor4F pMove = new EazyColor4F(TypeBehaviorAction.BY);
            pMove.setBy(pMoveBy, pUnit, calculByTime);
            return pMove;
        }

        protected void setFrom(EazyMethodInfo pFrom)
        {
            _typeActionBehavior += (int)TypeBehaviorAction.FROM;
            _isContinious = false;
            _getFrom = pFrom;
        }
        protected void setFrom(Color pFrom)
        {
            _typeActionBehavior += (int)TypeBehaviorAction.FROM;
            _isContinious = false;
            _from = pFrom;
        }
        protected void setTo(EazyMethodInfo pDestiny, float pUnit, bool calculByTime = true)
        {
            _unit = pUnit;
            _getDestiny = pDestiny;
            _calculByTime = calculByTime;
        }
        protected void setTo(Color pDestiny, float pUnit, bool calculByTime = true)
        {
            _unit = pUnit;
            _desnity = pDestiny;
            _calculByTime = calculByTime;
        }
        protected void setBy(Color pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyColor4F pMove = this;
            pMove._unit = pUnit;
            pMove._nextMove = pMoveBy;
            pMove._calculByTime = calculByTime;
        }
        protected void setBy(EazyMethodInfo pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyColor4F pMove = this;
            pMove._unit = pUnit;
            pMove._getDestiny = pMoveBy;
            pMove._calculByTime = calculByTime;
        }
        // Update is called once per frame
        public override float apply(float pTime)
        {
            float pSec = base.apply(pTime);
            if (pSec >= 0)
            {
                return pSec;
            }
            updateTime(pTime);
            float pPercent = percentage;
            if (pPercent > 1)
            {
                pPercent = 1;
            }
            _current.r = ease(_from.r, _desnity.r, pPercent);
            _current.b = ease(_from.b, _desnity.b, pPercent);
            _current.g = ease(_from.g, _desnity.g, pPercent);
            _current.a = ease(_from.a, _desnity.a, pPercent);

            // _infoRoot.currentInfo._pos.z = ease(startInfo._pos.z, endInfo._pos.z, percentage);
            if (percentage >= 1)
            {
                if (_currentLoop > 0)
                {
                    _currentLoop--;
                    if (_currentLoop <= 0)
                    {
                        IsDone = true;
                        return 0;
                    }
                    if (getTypeBehaviorAction() < TypeBehaviorAction.FROM)
                    {
                        _from = _current;
                    }
                    int templeLoop = _currentLoop;
                    resetTime();
                    _currentLoop = templeLoop;
                    return -1;
                }
                return 0;
            }


            return -1;
        }


        public override void extendCallBack(GameObject pObject)
        {
            if (spriteRenderer)
            {
                spriteRenderer.color = _current;
            }
            else if (renderer)
            {
                renderer.material.color = _current;
            }
            else if (light)
            {
                light.color = _current;
            }
        }

        public override void setUpAction(RootMotionController pRoot)
        {
            if (pRoot)
            {
                if (!spriteRenderer)
                {
                    spriteRenderer = pRoot.GetComponent<SpriteRenderer>();
                }
                if (!renderer)
                {
                    renderer = pRoot.GetComponent<Renderer>();
                }
                if (!light)
                {
                    light = pRoot.GetComponent<Light>();
                }
            }
            if (_typeActionBehavior < TypeBehaviorAction.FROM)
            {
                if (spriteRenderer)
                {
                    _from = spriteRenderer.color;
                }
                else if (renderer)
                {
                    _from = renderer.material.color;
                }
                else if (light)
                {
                    _from = light.color;
                }
            }
            base.setUpAction(pRoot);

            TypeBehaviorAction pTypeBehaviorTo = _typeActionBehavior;
            if ((int)_typeActionBehavior < (int)TypeBehaviorAction.FROM)
            {

            }
            else
            {
                pTypeBehaviorTo = _typeActionBehavior - (int)TypeBehaviorAction.FROM;
            }
            if (_getFrom.typeReflection != TypeReflection.None)
            {
                _getFrom.getValue(ref _from);
            }
            if (_getDestiny.typeReflection != TypeReflection.None)
            {
                _getDestiny.getValue(ref _desnity);
            }
            if (pTypeBehaviorTo == TypeBehaviorAction.BY)
            {
                _desnity = _from + _nextMove;
            }
            float pDistance = Vector4.Distance(_desnity, _from);
            _speed = !_calculByTime ? _unit : pDistance / _unit;
            _time = _calculByTime ? _unit : pDistance / _unit;
        }
    }

    [DrawEzActionBaseOn(typeof(float))]
    public class EazyFloatAction : EazyAction
    {
        protected float _from;
        protected float _desnity;
        protected float _current;
        protected float _nextFloat;
        public delegate float FloatMethod();
        public EazyMethodInfo _getDestiny, _getFrom;

        //       protected delegate void setVector3Method(Vector3 pVec, bool pEnd);
        //protected getVector3Method _getVector3Method;
        //protected setVector3Method _setVector3Method;

        public override void copyFromInfo(EazyActionInfo actionInfo)
        {
            _from = actionInfo.infoFrom.Float;
            if (actionInfo.IsEnableBy)
            {
                _nextFloat = actionInfo.InfoStep.Float;
                _typeActionBehavior = TypeBehaviorAction.BY;
            }
            else
            {
                _desnity = actionInfo.InfoStep.Float;
                _typeActionBehavior = TypeBehaviorAction.TO;
            }
            if (actionInfo.TypeInfoIn != 0)
            {
                _typeActionBehavior += (int)TypeBehaviorAction.FROM;
            }
            base.copyFromInfo(actionInfo);

        }
        public void setFrom(EazyMethodInfo pFrom)
        {
            _typeActionBehavior += (int)TypeBehaviorAction.FROM;
            _isContinious = false;
            _getFrom = pFrom;
        }
        public void setFrom(float pFrom)
        {
            _typeActionBehavior += (int)TypeBehaviorAction.FROM;
            _isContinious = false;
            _from = pFrom;
        }
        protected void setTo(EazyMethodInfo pDestiny, float pUnit, bool calculByTime = true)
        {
            _unit = pUnit;
            _getDestiny = pDestiny;
            _calculByTime = calculByTime;
        }
        protected void setTo(float pDestiny, float pUnit, bool calculByTime = true)
        {
            _unit = pUnit;
            _desnity = pDestiny;
            _calculByTime = calculByTime;
        }
        protected void setBy(float floatBy, float pUnit, bool calculByTime = true)
        {

            EazyFloatAction pMove = this;
            pMove._unit = pUnit;
            pMove._nextFloat = floatBy;
            pMove._calculByTime = calculByTime;
        }
        protected void setBy(EazyMethodInfo floatBy, float pUnit, bool calculByTime = true)
        {

            EazyFloatAction pMove = this;
            pMove._unit = pUnit;
            pMove._getDestiny = floatBy;
            pMove._calculByTime = calculByTime;
        }
        // Update is called once per frame
        public override float apply(float pTime)
        {
            float pSec = base.apply(pTime);
            if (pSec >= 0)
            {
                return pSec;
            }

            updateTime(pTime);
            float pPercent = percentage;
            if (pPercent > 1)
            {
                pPercent = 1;
            }
            _current = ease(_from, _desnity, pPercent);
            if (_from == _desnity)
            {
                _current = _from;
            }
            if (percentage >= 1)
            {
                if (_currentLoop > 0)
                {
                    _currentLoop--;
                    if (_currentLoop <= 0)
                    {
                        IsDone = true;
                        return 0;
                    }
                    if (getTypeBehaviorAction() < TypeBehaviorAction.FROM)
                    {
                        _from = _current;
                    }
                    int templeLoop = _currentLoop;
                    resetTime();
                    _currentLoop = templeLoop;
                    return -1;
                }
                return 0;
            }


            return -1;
        }
        public override void setUpAction(RootMotionController pRoot)
        {
            base.setUpAction(pRoot);

            TypeBehaviorAction pTypeBehaviorTo = _typeActionBehavior;
            if ((int)_typeActionBehavior < (int)TypeBehaviorAction.FROM)
            {
            }
            else
            {
                pTypeBehaviorTo = _typeActionBehavior - (int)TypeBehaviorAction.FROM;
            }
            if (_getFrom.typeReflection != TypeReflection.None)
            {
                _getFrom.getValue(ref _from);
            }
            if (_getDestiny.typeReflection != TypeReflection.None)
            {
                _getDestiny.getValue(ref _desnity);
            }
            if (pTypeBehaviorTo == TypeBehaviorAction.BY)
            {
                _desnity = _from + _nextFloat;
            }
            float pDistance = Math.Abs(_desnity - _from);
            _speed = !_calculByTime ? _unit : pDistance / _unit;
            _time = _calculByTime ? _unit : pDistance / _unit;
            _current = _from;
        }
    }
    [EazyActionNew("EazyFade", true, false, typeof(SpriteRenderer), typeof(Renderer), typeof(Light))]
    public class EazyFade : EazyFloatAction
    {
        SpriteRenderer spriteRenderer;
        Renderer renderer;
        Light light;
        public EazyFade() : base()
        {
        }
        public EazyFade(TypeBehaviorAction pBehavior) : this()
        {
            _typeActionBehavior = pBehavior;
        }


        public EazyFade from(EazyMethodInfo pFrom)
        {
            base.setFrom(pFrom);
            return this;
        }

        public EazyFade from(float pFrom)
        {
            base.setFrom(pFrom);
            return this;
        }

        public static EazyFade to(EazyMethodInfo pDestiny, float pUnit, bool calculByTime = true)
        {
            EazyFade pMove = new EazyFade(TypeBehaviorAction.TO);
            pMove.setTo(pDestiny, pUnit, calculByTime);
            return pMove;
        }
        public static EazyFade to(float pDestiny, float pUnit, bool calculByTime = true)
        {

            EazyFade pMove = new EazyFade(TypeBehaviorAction.TO);
            pMove.setTo(pDestiny, pUnit, calculByTime);
            return pMove;
        }
        public static EazyFade by(float pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyFade pMove = new EazyFade(TypeBehaviorAction.BY);
            pMove.setBy(pMoveBy, pUnit, calculByTime);
            return pMove;
        }
        public static EazyFade by(EazyMethodInfo pMoveBy, float pUnit, bool calculByTime = true)
        {

            EazyFade pMove = new EazyFade(TypeBehaviorAction.BY);
            pMove.setBy(pMoveBy, pUnit, calculByTime);
            return pMove;
        }

        public override void extendCallBack(GameObject pObject)
        {
            if (spriteRenderer)
            {
                spriteRenderer.color = spriteRenderer.color.setAlpha(_current);
            }
            else if (renderer)
            {
                renderer.material.color = renderer.material.color.setAlpha(_current);
            }
            else if (light)
            {
                light.color = light.color.setAlpha(_current);
            }
        }
        // Update is called once per frame
        public override void setUpAction(RootMotionController pRoot)
        {
            if (pRoot)
            {
                if (!spriteRenderer)
                {
                    spriteRenderer = pRoot.GetComponent<SpriteRenderer>();
                }
                else if (!renderer)
                {
                    renderer = pRoot.GetComponent<Renderer>();
                }
                else if (!light)
                {
                    light = pRoot.GetComponent<Light>();
                }
            }
            if (_typeActionBehavior < TypeBehaviorAction.FROM)
            {
                if (spriteRenderer)
                {
                    _from = spriteRenderer.color.a;
                }
                else if (renderer)
                {
                    _from = renderer.material.color.a;
                }
                else if (light)
                {
                    _from = light.color.a;
                }
            }
            base.setUpAction(pRoot);
        }
    }



    public abstract class EazyExtends<T> : ScriptableObject
    {
        public T value;
    }
    public class SpriteAnimationExtends : EazyExtends<SpriteAnimationInfoExtends>
    {

    }

    [System.Serializable]
    public class SpriteAnimationInfoExtends
    {
        [SerializeField]
        public List<AnimateSprite> _spriteSheets = new List<AnimateSprite>();
    }
    [System.Serializable]
    public class AnimateSprite
    {
        public AnimateSprite(Sprite pSpr, float pDuration)
        {
            _sprite = pSpr;
            _duration = pDuration;
        }
        public Sprite _sprite;
        public float _duration;
    }




    [System.Serializable]
    public class DelayExtendInfo
    {
        [SerializeField]
        public float delay;
        [SerializeField]
        public EazyGetBoolValue value;
    }



    //[EazyActionNew("Delay",typeof(DelayExtendObject), false,false, typeof(EazyAllType))]
    //public class DelayTime : EazyAction
    //{
    //    public DelayTime()
    //    {
    //    }
    //    public static EazyAction create(float pTime)
    //    {
    //        DelayTime pDelay = new DelayTime();
    //        pDelay._time = pTime;
    //        return pDelay;
    //    }

    //    public static EazyAction createForever()
    //    {
    //        DelayTime pDelay = new DelayTime();
    //        pDelay._time = -1;
    //        return pDelay;
    //    }
    //    public override void extendCallBack(GameObject pObject)
    //    {
    //        base.extendCallBack(pObject);
    //    }
    //}
    public class DelayTime : EazyAction
    {
        public float _timeDestiny;
        public DelayTime()
        {
        }
        public static EazyAction create(float pTime)
        {
            DelayTime pDelay = new DelayTime();
            pDelay._timeDestiny = pTime;
            pDelay._time = pTime;
            return pDelay;
        }

        public static EazyAction createForever()
        {
            DelayTime pDelay = new DelayTime();
            pDelay._timeDestiny = -1;
            pDelay._time = -1;
            return pDelay;
        }

        // Update is called once per frame
        public override float apply(float pTime)
        {
            updateTime(pTime);
            if (_currentTime >= _timeDestiny && _timeDestiny >= 0)
            {
                IsDone = true;
                return 0;
            }
            return -1;
        }
    }
    public class CallFunc : EazyAction
    {
        public UnityEvent _event;
        public object _target;
        public string _funcName;
        public object _parameter;

        public CallFunc()
        {
            _event = new UnityEvent();
            _typeAction = TypeAction.NONE;
        }
        public static CallFunc create(params UnityAction[] pACtion)
        {
            CallFunc pCall = new CallFunc();
            for (int i = 0; i < pACtion.Length; i++)
            {
                pCall._event.AddListener(pACtion[i]);
            }
            return pCall;
        }
        public static CallFunc create(UnityAction pACtion)
        {
            CallFunc pCall = new CallFunc();
            pCall._event.AddListener(pACtion);
            return pCall;
        }
        public static CallFunc create(object pTarget, string pFuncName, object pValue)
        {
            CallFunc pCall = new CallFunc();
            pCall._target = pTarget;
            pCall._funcName = pFuncName;
            pCall._parameter = pValue;
            return pCall;
        }

        public void addAction(UnityAction pACtion)
        {
            if (_event != null)
            {
                _event.AddListener(pACtion);
            }
        }
        // Update is called once per frame
        public override float apply(float pTime)
        {
            updateTime(pTime);
            excute();
            return 0;
        }
        public void excute()
        {
            if (_event != null)
            {
                _event.Invoke();
            }
            if (_target != null)
            {
                if (_target.GetType().IsSubclassOf(typeof(MonoBehaviour)) || _target.GetType() == typeof(MonoBehaviour))
                {
                    ((MonoBehaviour)_target).SendMessage(_funcName, _parameter);
                }
                else if (_target.GetType().IsSubclassOf(typeof(GameObject)) || _target.GetType() == typeof(GameObject))
                {
                    ((GameObject)_target).SendMessage(_funcName, _parameter);
                }
            }
        }
    }

    public abstract class EazyActionManager : EazyAction
    {
        protected RootMotionController mainTarget;

        public override void setUpAction(RootMotionController pRoot)
        {
            mainTarget = pRoot;
            base.setUpAction(pRoot);
        }
    }

    //public class EazyRepeat : EazyActionManager
    //{
    //    public EazyAction _action;
    //    public EazyAction _actionClone;
    //    public int _loopCount = 0;

    //    public EazyRepeat()
    //    {
    //        //       _updateInfo = setInfo;
    //    }

    //    public override void resetTime()
    //    {
    //        base.resetTime();
    //        _action.resetTime();
    //    }

    //    void setInfo(InfoTranform pInfo, bool pIn)
    //    {
    //        _action._updateInfo(pInfo, pIn);
    //    }
    //    public override void extendCallBack(GameObject pObject)
    //    {
    //        _action.extendCallBack(pObject);
    //    }
    //    public static EazyRepeat create(EazyAction pAction, int pLoop)
    //    {
    //        EazyRepeat pRepeat = new EazyRepeat();
    //        pRepeat._action = pAction;
    //        // pRepeat._actionClone = (MyCustomMove)pRepeat._action.Clone();
    //        pRepeat._loopCount = pLoop;
    //        return pRepeat;
    //    }
    //    public static EazyRepeat createForever(EazyAction pAction)
    //    {
    //        EazyRepeat pRepeat = new EazyRepeat();
    //        pRepeat._action = pAction;
    //        pRepeat._loopCount = -1;
    //        return pRepeat;
    //    }
    //    // Update is called once per frame
    //    public override float apply(float pTime)
    //    {
    //        updateTime(pTime);
    //        float pSec = _action.apply(pTime);
    //        _infoRoot = _action._infoRoot;
    //        _typeAction = _action._typeAction;
    //        if (pSec >= 0)
    //        {
    //            if (_loopCount > 0)
    //            {
    //                _loopCount--;
    //            }

    //            if (_loopCount == 0)
    //            {
    //                IsDone = true;
    //                return 0;
    //            }
    //            else
    //            {
    //                //_action = (EazyAction)_actionClone.Clone();
    //                //if (!_action._isContinious)
    //                //{
    //                //    _action._infoRoot.updateCurrentInfo(_action._infoRootClone);
    //                //}
    //                if (getTypeBehaviorAction() < TypeBehaviorAction.FROM)
    //                {
    //                    _action.setUpAction(mainTarget);
    //                }
    //                _action.resetTime();
    //            }
    //        }
    //        return -1;
    //    }



    //    public override TypeBehaviorAction getTypeBehaviorAction()
    //    {
    //        _typeActionBehavior = _action.getTypeBehaviorAction();
    //        return _typeActionBehavior;
    //    }


    //    public override void setUpAction(RootMotionController pRoot)
    //    {
    //        base.setUpAction(pRoot);
    //        mainTarget = pRoot;
    //        _action.setUpAction(pRoot);
    //    }
    //    public override float getTime()
    //    {
    //        float pTime = base.getTime();
    //        pTime += _action.getTime() * _loopCount;
    //        return pTime;
    //    }
    //}

    [EazyActionNew("Sequences", false, false, typeof(EazyAllType))]
    public class Sequences : EazyActionManager
    {
        public EazyAction[] _listAction;
        private int _currentIndex = 0;

        public Sequences()
        {
            // _updateInfo = setInfo;
        }
        public override void resetTime()
        {
            base.resetTime();
            _currentIndex = 0;
            for (int i = 0; i < _listAction.Length; ++i)
            {
                _listAction[i].resetTime();
            }
        }

        public void resetChild()
        {
            for (int i = 0; i < _listAction.Length; ++i)
            {
                _listAction[i].resetTime();
            }
        }

        public override void extendCallBack(GameObject pObject)
        {
            if (_listAction != null && _currentIndex < _listAction.Length)
            {
                _listAction[_currentIndex].extendCallBack(pObject);
            }
        }

        public static Sequences create(params EazyAction[] pCustomAction)
        {
            Sequences pSequence = new Sequences();
            pSequence._listAction = pCustomAction;
            return pSequence;
        }

        public static Sequences create()
        {
            Sequences pSequence = new Sequences();
            return pSequence;
        }
        bool nextAction = false;
        // Update is called once per frame
        public override float apply(float pTime)
        {
            //updateTime(pTime);
            float pSec = base.apply(pTime);
            if (pSec >= 0)
            {
                return pSec;
            }
            if (_currentIndex < _listAction.Length)
            {
                if (_listAction[_currentIndex].IsDone)
                {
                    _currentIndex++;
                    if (_currentIndex > 0 && _currentIndex + 1 <= _listAction.Length)
                    {
                        if (_listAction[_currentIndex] != null)
                        {
                            _listAction[_currentIndex].setUpAction(mainTarget);
                        }
                        else
                        {
                            _currentIndex++;
                        }
                    }
                }
                if (_currentIndex >= _listAction.Length)
                {
                    _currentIndex = _listAction.Length - 1;

                    if (_currentLoop > 0)
                    {
                        _currentLoop--;
                        if (_currentLoop <= 0)
                        {
                            IsDone = true;
                            return 0;
                        }
                        _currentIndex = 0;
                        int pTempleLoop = _currentLoop;
                        if (getTypeBehaviorAction() < TypeBehaviorAction.FROM)
                        {
                            _listAction[_currentIndex].setUpAction(mainTarget);
                        }
                        resetTime();
                        _currentLoop = pTempleLoop;
                    }
                    else
                    {
                        IsDone = true;
                        return 0;
                    }

                }
                else
                {
                    pSec = _listAction[_currentIndex].apply(pTime);
                    _typeAction = _listAction[_currentIndex]._typeAction;
                    if (pSec >= 0)
                    {
                        _listAction[_currentIndex].IsDone = true;
                    }
                }
            }

            return -1;
        }

        public override bool isLocal()
        {
            if (_listAction.Length > _currentIndex)
            {
                //if (nextAction && _currentIndex > 0)
                //{
                //    _islocal = _listAction[_currentIndex - 1].isLocal();
                //}
                //else
                //{
                //    _islocal = _listAction[_currentIndex].isLocal();
                //}
                _islocal = _listAction[_currentIndex].isLocal();
                return _islocal;
            }
            return base.isLocal();
        }

        public override TypeBehaviorAction getTypeBehaviorAction()
        {

            if (_listAction.Length > _currentIndex)
            {
                if (nextAction && _currentIndex > 0)
                {
                    _typeActionBehavior = _listAction[_currentIndex - 1].getTypeBehaviorAction();
                }
                else
                {
                    _typeActionBehavior = _listAction[_currentIndex].getTypeBehaviorAction();
                }
                return _typeActionBehavior;
            }
            return base.getTypeBehaviorAction();
        }

        public override void setUpAction(RootMotionController pRoot)
        {
            _currentIndex = 0;
            base.setUpAction(pRoot);
            for (int i = 0; i < _listAction.Length; i++)
            {
                if (_listAction[i] != null)
                {
                    _listAction[i].setUpAction(pRoot);
                }
            }
        }


        public override float getTime()
        {
            float pTime;
            pTime = base.getTime();
            for (int i = 0; i < _listAction.Length; i++)
            {
                if (_listAction[i] != null)
                {
                    pTime += _listAction[i].getTime();
                }
            }
            return pTime;
        }
    }

    public class EventHandlerAction
    {
        public EventHandlerAction()
        {
            _timeRunType = RuntimeAction.UPDATE_ALLWAY;
        }
        public static EventHandlerAction createEvent(EventActionSimple pEventAction, Condioner pCondition = null)
        {
            EventHandlerAction pEvent = new EventHandlerAction();
            pEvent._eventSimple = pEventAction;
            pEvent._conditioner = pCondition;
            return pEvent;
        }
        public static EventHandlerAction createEvent(EventAction pEventAction, Condioner pCondition = null)
        {
            EventHandlerAction pEvent = new EventHandlerAction();
            pEvent._event = pEventAction;
            pEvent._conditioner = pCondition;
            return pEvent;
        }
        public delegate bool Condioner();
        public delegate void EventAction(RootMotionController pRoot, EazyAction pAction, EventHandlerAction pHandle);
        public delegate void EventActionSimple();
        public EventActionSimple _eventSimple;
        public EventAction _event;
        public Condioner _conditioner = null;
        public object value = null;
        public RuntimeAction _timeRunType;
        public float time = 0;
        public float curremtTime = 0;
        public void runAt(RuntimeAction pTime, float value)
        {
            _timeRunType = pTime;
            time = value;
        }
    }

    public enum RuntimeAction { BEGIN = 0, UPDATE_DURATION, UPDATE_ALLWAY, UPDATE_AT_RUNTIME, END }

}