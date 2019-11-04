using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace EazyGUI {

    public class EazyTreeElement
    {
        List<EazyTreeElement> _arrayTree = null;
        EazyTreeElement parrent;
        object _objecTarget;
        string name = "";
        int layer = 0;
        bool open = false;
        bool focus = false;
        public delegate void TreeElementCallBack(EazyTreeElement target);
        TreeElementCallBack _callBackTreeElement,_callBackRightClick;
        GUIStyle styleElement;
        public Action clearFocusCallBack;
        public EazyTreeElement()
        {
            Name = "none";
        }
        public EazyTreeElement(string name, object target = null, TreeElementCallBack callback = null)
        {
            ObjecTarget = target;
            CallBackTreeElement = callback;
            Name = name;
        }
#region property
        public int ChildCount
        {
            get
            {
                return _arrayTree == null ? 0:  _arrayTree.Count;
            }
        }
        public bool MainTree
        {
            set {
                if (value)
                {
                    clearFocusCallBack = clearFocus;
                }
            }
            get
            {
                return clearFocusCallBack == null ? false : true;
            }
        }

        public object ObjecTarget
        {
            get
            {
                return _objecTarget;
            }

            set
            {
                _objecTarget = value;
            }
        }

        public TreeElementCallBack CallBackTreeElement
        {
            get
            {
                return _callBackTreeElement;
            }

            set
            {
                _callBackTreeElement = value;
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

        public EazyTreeElement Parrent
        {
            get
            {
                return parrent;
            }

            set
            {
                parrent = value;
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

        public bool Open
        {
            get
            {
                return open;
            }

            set
            {
                open = value;
            }
        }

        public TreeElementCallBack CallBackRightClick
        {
            get
            {
                return _callBackRightClick;
            }

            set
            {
                _callBackRightClick = value;
            }
        }
#endregion

        public void clearFocus()
        {
            focus = false;
            if (_arrayTree != null)
            {
                for (int i = 0; i < _arrayTree.Count; i++)
                {
                    _arrayTree[i].clearFocus();
                }
            }
        }

        public void clearChild()
        {
            if (_arrayTree != null)
            {
                _arrayTree.Clear();
            }
        }

        public void addChild(EazyTreeElement element,bool newElement = true)
        {
            if (_arrayTree == null)
            {
                _arrayTree = new List<EazyTreeElement>();
            }
            element.Parrent = this;
            element.Layer = Layer + 1;
            element.clearFocusCallBack = clearFocusCallBack;
            _arrayTree.Add(element);
            if (newElement)
            {
                onAddElement(element);
            }
        }

        public bool addChild(EazyTreeElement element,string nameParrent)
        {

            if (_arrayTree == null)
            {
                _arrayTree = new List<EazyTreeElement>();
            }
           if(Name == nameParrent)
            {
                addChild(element);
                return true;
            }
           for(int i = 0; i < _arrayTree.Count; ++i)
            {
                if (_arrayTree[i].addChild(element, nameParrent))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void onAddElement(EazyTreeElement element)
        {
    
        }

        public void removeChild(EazyTreeElement element)
        {
            onRemoveElement(element);
            _arrayTree.Remove(element);       
        }

        public virtual void onRemoveElement(EazyTreeElement element)
        {

        }
        
        Texture2D bgBlue;
        public virtual void onDrawTreeElement(Vector2 pos,Vector2 sizeOut)
        {

            //pos.x = (_arrayTree != null && _arrayTree.Count > 0) ? pos.x : pos.x + 10;
            if (styleElement == null)
            {
                styleElement = new GUIStyle("Label");
          
            }
            styleElement.padding.left = 10 * layer;
           
            if (GUI.Button(new Rect(pos.x, pos.y, sizeOut.x, sizeOut.y),(ChildCount > 0 ? (!Open ? ">| " : "v| ") : "-|") + Name, styleElement))
            {      
                clearFocusCallBack();
                Open = !Open;
                focus = !focus;
                if ((Open || ChildCount == 0) )
                {

                    if (Event.current.button == 1 && focus)
                    {
                        if (CallBackRightClick != null)
                        {
                            CallBackRightClick(this);
                        }
                    }
                    if (CallBackTreeElement != null)
                    {
                        CallBackTreeElement(this);
                    }
                }
    
            }
            if (focus)
            {
                if (bgBlue == null)
                {
                    bgBlue = Resources.Load("asset/eazy_focus", typeof(Texture2D)) as Texture2D;
                }
                styleElement.normal.background = bgBlue;
            }
            else
            {
                styleElement.normal.background = null;
            }
            // open = EditorGUI.Foldout(new Rect(pos.x, pos.y, 10, sizeOut.y), open, Name, true);
            if (Open)
            {
                if (_arrayTree != null)
                {
                    for (int i = 0; i < _arrayTree.Count; ++i)
                    {
                        _arrayTree[i].onDrawTreeElement(new Vector2(pos.x , pos.y + sizeOut.y*(i+1)), sizeOut);
                    }
                }
            }
            if (MainTree)
            {

            }
        }
    }

    public class EazyGUITools {
        public static EazyTreeElement drawTree(EazyTreeElement element,Vector2 pos,Vector2 size)
        {
            element.onDrawTreeElement(pos, size);
            return element;
        }
    }
}
