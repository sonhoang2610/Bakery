using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EazyCustomAction;
public enum EffectDisplay { NONE,FADE,FADE_SCALE}
public class UIElement : MonoBehaviour {
    [SerializeField]
    EffectDisplay effect;
    [SerializeField]
    UnityEvent onEnableEvent;
    [SerializeField]
    UnityEvent onDisableEvent;
    [SerializeField]
    UnityEvent onStartEvent;
    [SerializeField]
    UnityEvent onInitEvent;
    [SerializeField]
    UnityEvent onEnableLateUpdateEvent;

    public Action _actionOnClose;
    public int relative = 0;

    public void handleEffect(bool active)
    {
        //if (effect == EffectDisplay.FADE_SCALE)
        //{
        //    if (active)
        //    {
        //        transform.localScale = new Vector3(0, 0, 0);
        //        GetComponent<UIWidget>().alpha = 0;
        //        RootMotionController.runAction(gameObject, NGUIFadeAction.to(1, 0.5f));
        //        RootMotionController.runAction(gameObject, EazyScale.to(new Vector3(1, 1, 1), 0.5f).setEase(EaseCustomType.easeOutExpo));
        //    }
        //    else
        //    {

        //    }
        //}
    }

    public void showRelative()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        relative++;
    }

    public void hideRelative()
    {
        relative--;
        if(relative <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    public void resetTween()
    {
        //UITweener[] tweenChild = GetComponentsInChildren<UITweener>();
        //foreach(var tween in tweenChild)
        //{
        //   tween.ResetToBeginning();
        //}
    }

    public void stepEnable()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public virtual void show()
    {
        gameObject.SetActive(true);
    }

    public virtual void close()
    {
        gameObject.SetActive(false);
        if (_actionOnClose != null)
        {
            _actionOnClose.Invoke();
        }
    }

    public void change()
    {
        if (gameObject.activeSelf)
        {
            close();
        }
        else
        {
            show();
        }
    }
    bool isFirst = false;

    private void LateUpdate()
    {
        if (isFirst)
        {
            onEnableLateUpdateEvent.Invoke();
            isFirst = false;
        }
    }
    private void OnEnable()
    {
        isFirst = true;
        if (onEnableEvent != null)
        {
            onEnableEvent.Invoke();
        }
        onStartEvent.Invoke();
    }

    private void OnDisable()
    {
        if(onDisableEvent != null)
        {
            onDisableEvent.Invoke();
        }
    }
    protected virtual void Start()
    {
        onInitEvent.Invoke();
    }

    public IEnumerator delayAction(float pDelay,UnityEvent pEvent)
    {
        yield return new WaitForSeconds(pDelay);
        pEvent.Invoke();
    }
    private void Update()
    {
        DoUpdate();
    }

    // Update is called once per frame
    protected virtual void DoUpdate () {
    
	}
}
