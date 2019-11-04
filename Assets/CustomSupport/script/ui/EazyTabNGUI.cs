using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
[System.Serializable]
public class UnityEventInt : UnityEvent<int>
{

}

[ExecuteInEditMode]
public class EazyTabNGUI : MonoBehaviour
{
    Button _button;
    Image _image;
    bool _pressed = false;
    public Color colorNormal = Color.white;
    public Color colorPressed = Color.white;
    public Sprite normalSprite2D;
    public Sprite pressedSprite2D;
    //public bool includeChild = false;
   // public bool isSetParameter = false;

    [SerializeField]
    protected UnityEventInt eventOnPressed;
    [SerializeField]
    protected UnityEventInt eventOnUnPress;

    [HideInInspector]
    private EazyGroupTabNGUI parentTab;

    private int _index;
    public Button ButtonTab
    {
        get
        {
            return _button ? _button : _button = GetComponent<Button>();
        }
    }

    public Image ImageSkin
    {
        get
        {
            return ButtonTab.image;
        }
    }

    public bool Pressed
    {
        get
        {
            return this._pressed;
        }

        set
        {
            this._pressed = value;
            ImageSkin.color = value ? colorPressed : colorNormal;
            //if (includeChild)
            //{
            //    var widgets = GetComponentsInChildren<UIWidget>();
            //    for (int i = 0; i < widgets.Length; ++i)
            //    {
            //        widgets[i].color = value ? colorPressed : colorNormal;
            //    }
            //}
            ImageSkin.sprite = value ? pressedSprite2D : normalSprite2D;

            if (_pressed)
            {
                EventOnPressed.Invoke(Index);
            }
            else
            {
                EventOnUnPress.Invoke(Index);
            }
            doSomeThingOnPressed();
        }
    }


    public void performClick()
    {
        if (parentTab)
        {
            parentTab.changeTab(Index);
        }
    }

    public int Index
    {
        get
        {
            return _index;
        }

        set
        {
            _index = value;
        }
    }

    public UnityEventInt EventOnPressed
    {
        get
        {
            return eventOnPressed;
        }

        set
        {
            eventOnPressed = value;
        }
    }

    public UnityEventInt EventOnUnPress
    {
        get
        {
            return eventOnUnPress;
        }

        set
        {
            eventOnUnPress = value;
        }
    }

    public EazyGroupTabNGUI ParentTab
    {
        get
        {
            return parentTab;
        }

        set
        {
            parentTab = value;
        }
    }

    protected virtual void doSomeThingOnPressed()
    {

    }

    // Use this for initialization
    private void Awake()
    {
        ButtonTab.onClick.AddListener(performClick);
    }

    //private void OnValidate()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
    }
}
