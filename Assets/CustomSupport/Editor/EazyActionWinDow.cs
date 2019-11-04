using EazyCustomAction;
using EazyGUI;
using EazyReflectionSupport;
using EazyCustomAction;
using Kit.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using EazyEditor;
using System.IO;

namespace EazyTweenEditor
{
    //
    // Summary:
    //     ///
    //     Tells an Editor class which run-time type it's an editor for.
    //     ///
    public class EazyActionDrawTree
    {
        EazyActionInfo _info;
        public bool _infoShow = false;
        public bool _focus = false;
        public Action<TypeActionEditorBehavior, EazyActionDrawTree> _windowActionBehavior;
        public Rect _rectFocus;
        public List<EazyActionDrawTree> _listDrawChild = new List<EazyActionDrawTree>();
        public EazyActionDrawTree _parrentDrawer;
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
        public EazyActionDrawTree(EazyActionInfo pInfo)
        {
            Info = pInfo;
        }

        public bool removeActionChild(EazyActionDrawTree pAction)
        {
            bool pRemove = _listDrawChild.Remove(pAction);
            if (!pRemove)
            {
                for (int i = _listDrawChild.Count - 1; i >= 0; i--)
                {
                    pRemove = _listDrawChild[i].removeActionChild(pAction);
                }
            }
            return pRemove;
        }


        //public void editActionChild(int pID, EazyActionDrawTree pAction)
        //{
        //    for (int i = _listDrawChild.Count - 1; i >= 0; i--)
        //    {
        //        if (_listDrawChild[i]._id == pID)
        //        {
        //            _listDrawChild[i] = pAction;
        //            return;
        //        }
        //    }
        //}

        public void onDrawInfoBase()
        {
            Handles.BeginGUI();
            Handles.color = Color.black;
            if (Info.Layer > 0)
            {
                Handles.DrawLine(new Vector3(_rectFocus.x + 3, _rectFocus.y + _rectFocus.height / 2), new Vector3(_rectFocus.x - 10, _rectFocus.y + _rectFocus.height / 2));
            }
            if (_infoShow)
            {
                if (Info.ListActionInfo.Count > 0)
                {
                    Handles.DrawLine(new Vector3(_rectFocus.x + 15, _rectFocus.y + _rectFocus.height / 2), new Vector3(_rectFocus.x + 15, _listDrawChild[_listDrawChild.Count - 1]._rectFocus.y + _listDrawChild[_listDrawChild.Count - 1]._rectFocus.height / 2));
                }
            }
            Handles.EndGUI();

            EditorGUILayout.BeginHorizontal(GUILayout.Height(20));
            for (int i = 0; i < Info.Layer; i++)
            {
                EditorGUILayout.LabelField("", GUILayout.Width(20));
            }
            bool pPressed = GUILayout.Button(Info.SelectedAction.name.ToString() + "(" + Info.Id.ToString() + ")", GUILayout.Width(100));
            Rect pRect = GUILayoutUtility.GetLastRect();
            if (pRect.y > 0)
            {
                _rectFocus = pRect;
            }
            if (pPressed)
            {

                GenericMenu _menuOptionAction = new GenericMenu();
                if (!_infoShow)
                {
                    _menuOptionAction.AddItem(new GUIContent("Show"), false, delegate
                    {
                        _windowActionBehavior(TypeActionEditorBehavior.Show, this);
                    });
                }
                else
                {
                    _menuOptionAction.AddItem(new GUIContent("Close"), false, delegate
                    {
                        _windowActionBehavior(TypeActionEditorBehavior.Close, this);
                    });
                }
                if (Info.SelectedAction.name.Equals(EazyActionContructor.Sequences.name))
                {

                    _menuOptionAction.AddItem(new GUIContent("Add"), false, delegate
                    {
                        _windowActionBehavior(TypeActionEditorBehavior.Add, this);
                    });
                }
                if (Info.SelectedAction.name != "")
                {
                    _menuOptionAction.AddItem(new GUIContent("Edit"), false, delegate
                    {
                        _windowActionBehavior(TypeActionEditorBehavior.Edit, this);
                    });
                }
                if (Info.Layer > 0)
                {
                    _menuOptionAction.AddItem(new GUIContent("Remove"), false, delegate
                    {
                        _windowActionBehavior(TypeActionEditorBehavior.Remove, this);
                    });
                }
                _menuOptionAction.ShowAsContext();

            }
            EditorGUILayout.EndHorizontal();
            if (_infoShow)
            {
                for (int i = 0; i < _listDrawChild.Count; i++)
                {
                    _listDrawChild[i].Info.Layer = Info.Layer + 1;
                    _listDrawChild[i].onDrawInfoBase();
                }
            }
        }
        public Rect getFocusRect()
        {
            if (_focus)
            {
                return _rectFocus;
            }
            else
            {
                for (int i = _listDrawChild.Count - 1; i >= 0; i--)
                {
                    Rect pRect = _listDrawChild[i].getFocusRect();
                    if (pRect.height > 1)
                    {
                        return pRect;
                    }
                }
            }
            return new Rect(0, 0, 1, 1);
        }

        public void clearFocus(EazyActionDrawTree pActionExcept)
        {
            if (this != pActionExcept)
            {
                _focus = false;
            }
            for (int i = _listDrawChild.Count - 1; i >= 0; i--)
            {
                if (_listDrawChild[i] != pActionExcept)
                {
                    _listDrawChild[i].clearFocus(pActionExcept);
                }
            }
        }

        public void showForce()
        {
            _infoShow = true;
            if (_parrentDrawer != null)
            {
                _parrentDrawer.showForce();
            }
        }
    }

    public static class ScriptableObjectUtility
    {
        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static ScriptableObject CreateAsset(Type pType)
        {
            var asset = ScriptableObject.CreateInstance(pType);

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + "extends" + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            return asset;
        }
    }

    public class EazyDrawTree
    {
        EazyActionDrawTree _drawer;
        EazyActionInfo _info;
        public EazyActionDrawTree Drawer
        {
            get
            {
                return _drawer;
            }

            set
            {
                _drawer = value;
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

        public EazyDrawTree(EazyActionDrawTree drawer, EazyActionInfo info)
        {
            Drawer = drawer;
            Info = info;
        }

        public void addChild(EazyDrawTree pChild, bool pNew = true)
        {
            if (pNew)
            {
                Info.ListActionInfo.Add(pChild.Info);
            }
            Drawer._listDrawChild.Add(pChild.Drawer);
        }

        public EazyDrawTree Parent
        {
            set { Info.ActionParrent = value.Info; Drawer._parrentDrawer = value.Drawer; }
        }


    }

    public class EazyAssetPrfabWindow : EditorWindow
    {
        string textPath = "";
        int count = 0;
        int col = 0;
        bool ishorizontal = true;
        Vector2 _sizeCell = new Vector2(100, 100);
        public Transform _target;
        private void OnGUI()
        {
            textPath = EditorGUILayout.TextField("Path:", textPath);
            count = EditorGUILayout.IntField("Count:", count);
            ishorizontal = EditorGUILayout.Toggle("Horizontal:", ishorizontal);
            _sizeCell = EditorGUILayout.Vector2Field("Cell Size:", _sizeCell);
            col = EditorGUILayout.IntField("Collum Limit:", col);
            if (GUILayout.Button("Create"))
            {
                EazyAsset.loadEazyPrefab(textPath, count, _sizeCell, col, ishorizontal, _target);
            }
        }
    }
    public class EazyActionWinDow : EditorWindow
    {
        // Add menu item named "My Window" to the Window menu
        [MenuItem("Window/My Window")]
        public static EazyActionWinDow ShowWindow()
        {

            //Show existing window instance. If one doesn't exist, make one.
            return EditorWindow.GetWindow<EazyActionWinDow>();
        }
        Rect rectLeft;

        #region properties
        EazyTreeElement tree;
        EazyDrawTree _mainAction;
        [SerializeField]
        EazyActionDraw _actionInfo;
        EazyActionInfo _actionInfoExport;
        List<EazyDrawTree> _arrayAction;
        int _newId = 0;
        private static int instance;
        Texture2D backgroundImage;
        Texture2D backgroundGrid;
        Rect _lastRect = Rect.zero;
        Vector2 _posScroll = Vector2.zero;
        TypeActionEditorBehavior _InfoActionBehavior = TypeActionEditorBehavior.None;
        string[] _arrayTab = { "Info" };
        int _chooseTabIndex = 0;
        EazyActionWithTarget _actionTarget;
        GameObject _objectExport;
        bool stopPreview = false;
        bool pausePreview = false;
        bool haveTarget = false;
        Scene _previewScene;
        string _stringButtonPreview = "Play";
        string _stringButtonPause = "Pause";
        public static bool isOpen
        {
            get
            {
                return instance > 0;
            }
            set
            {
                if (!value)
                {
                    instance = 0;
                }
            }
        }
        public EazyActionWithTarget ActionTarget
        {
            get
            {
                return _actionTarget;
            }

            set
            {
                _actionTarget = value;
                MainAction = value.Info;
            }
        }
        public List<EazyDrawTree> ArrayAction
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
        public bool HaveTarget
        {
            get
            {
                return haveTarget;
            }

            set
            {
                haveTarget = value;
            }
        }
        public EazyActionInfo ActionInfoExport
        {
            get
            {
                return _actionInfoExport;
            }

            set
            {
                _actionInfoExport = value;
            }
        }

        public EazyActionInfo MainAction
        {
            get
            {
                return _mainAction.Info;
            }

            set
            {
                _newId = 0;
                if (ArrayAction == null)
                {

                    ArrayAction = new List<EazyDrawTree>();
                }
                ArrayAction.Clear();
                addAction(-1, value);
            }
        }

        public EazyTreeElement Tree
        {
            get
            {
                return tree;
            }

            set
            {
                tree = value;
                tree.MainTree = true;
            }
        }

        public GameObject ObjectExport
        {
            get
            {
                return _objectExport;
            }

            set
            {
                _objectExport = value;
            }
        }
        #endregion
        #region Handle
        public Rect getWinDowRect()
        {
            return position;
        }
        private void addAction(int pIDParent, EazyActionInfo pChild, bool newElement = true)
        {
            if (pIDParent == -1)
            {
                _mainAction = new EazyDrawTree(new EazyActionDrawTree(pChild), pChild);
                ArrayAction.Add(_mainAction);
                pChild.setID(0);
                pChild.Layer = 0;
                _mainAction.Drawer._windowActionBehavior = activeBehavior;
                _newId++;
                EazyActionInfo[] actionChild = pChild.getAllChilds();
                if (actionChild.Length > 0)
                {
                    for (int i = 0; i < actionChild.Length; i++)
                    {
                        addAction(actionChild[i].ActionParrent.Id, actionChild[i], false);
                    }
                }

                return;
            }
            for (int i = 0; i < ArrayAction.Count; i++)
            {
                if (ArrayAction[i].Info.Id == pIDParent || pIDParent == -1)
                {
                    ArrayAction[i].Drawer._infoShow = true;
                    var newChild = new EazyDrawTree(new EazyActionDrawTree(pChild), pChild);
                    ArrayAction[i].addChild(newChild, newElement);
                    newChild.Drawer._windowActionBehavior = activeBehavior;
                    newChild.Parent = ArrayAction[i];
                    newChild.Info.Layer = ArrayAction[i].Info.Layer + 1;

                    ArrayAction.Add(newChild);

                    if (newElement)
                    {
                        newChild.Info.setID(_newId);
                        _newId++;
                        newChild.Drawer._focus = true;
                    }
                    else
                    {
                        if (newChild.Info.Id >= _newId)
                        {
                            _newId = newChild.Info.Id + 1;
                        }
                    }
                    closeAllExcept(newChild.Drawer);
                    //for (int j = 0; j < pChild._listActionInfo.Count; j++)
                    //{
                    //    addAction(pChild._id, pChild._listActionInfo[j]);
                    //}
                    break;
                }
            }
        }

        private void editAction(int pIDAction, EazyActionInfo pAction)
        {
            _actionInfo._disable = true;
            EazyDrawTree action = findDraw(pIDAction);
            if (action != null)
            {
                action.Drawer._infoShow = true;
                pAction.CopyAllTo(action.Info);
            }
        }

        private void removeAction(EazyDrawTree pAction)
        {
            pAction.Drawer._parrentDrawer.removeActionChild(pAction.Drawer);
            pAction.Info.ActionParrent.removeActionChild(pAction.Info);
            // Destroy(pAction.Info);
            ArrayAction.Remove(pAction);
        }

        public void reNew()
        {
            Tree.clearChild();
        }

        private EazyDrawTree findDraw(EazyActionInfo pAction)
        {
            foreach (EazyDrawTree draw in ArrayAction)
            {
                if (draw.Info == pAction)
                {
                    return draw;
                }
            }
            return null;
        }

        private EazyDrawTree findDraw(int id)
        {
            foreach (EazyDrawTree draw in ArrayAction)
            {
                if (draw.Info.Id == id)
                {
                    return draw;
                }
            }
            return null;
        }


        private void closeAllExcept(EazyActionDrawTree pAction)
        {
            for (int i = 0; i < ArrayAction.Count; i++)
            {

                if (ArrayAction[i].Drawer != pAction)
                {
                    ArrayAction[i].Drawer.clearFocus(pAction);
                    if (!ArrayAction[i].Info.SelectedAction.name.Equals(EazyActionContructor.Sequences.name) || ArrayAction[i].Info.ListActionInfo.Count == 0)
                    {
                        ArrayAction[i].Drawer._infoShow = false;
                    }
                }
            }
        }


        void onDrawRectFocus(Rect pRect)
        {
            if (pRect.height > 1)
            {
                GUI.DrawTexture(new Rect(0, pRect.y - 1, position.width, 20), backgroundImage, ScaleMode.ScaleAndCrop);
            }
        }

        public void chooseActionTarget(EazyActionWithTarget action)
        {
            ActionTarget = action;
        }

        public void activeBehavior(TypeActionEditorBehavior pBehavior, EazyActionDrawTree pActionInfo)
        {
            TypeActionEditorBehavior pOldBehavior = _InfoActionBehavior;
            _InfoActionBehavior = pBehavior;
            switch (pBehavior)
            {
                case TypeActionEditorBehavior.Add:
                    {
                        _actionInfo.Info =new EazyActionInfo();
                        _actionInfo.Info.init();
                        _actionInfo.Info.Layer = pActionInfo.Info.Layer + 1;
                        _actionInfo._disable = false;
                        _actionInfo.Info.Id = _newId + 1;
                        _actionInfo.Info.ActionParrent = pActionInfo.Info;
                        pActionInfo._focus = true;
                        closeAllExcept(pActionInfo);
                        break;
                    }
                case TypeActionEditorBehavior.Edit:
                    {
                        _actionInfo.Info = (EazyActionInfo)pActionInfo.Info.Clone();
                        _actionInfo._disable = false;
                        pActionInfo._focus = true;
                        pActionInfo._infoShow = true;
                        closeAllExcept(pActionInfo);
                        break;
                    }
                case TypeActionEditorBehavior.Remove:
                    {
                        removeAction(findDraw(pActionInfo.Info.Id));
                        _actionInfo.Info = null;
                        break;
                    }
                case TypeActionEditorBehavior.Show:
                    {
                        _actionInfo.Info = pActionInfo.Info;
                        pActionInfo._focus = true;
                        _actionInfo._disable = true;
                        pActionInfo._infoShow = true;
                        closeAllExcept(pActionInfo);
                        break;
                    }
                case TypeActionEditorBehavior.Close:
                    {

                        pActionInfo._infoShow = false;
                        _actionInfo.Info = null;
                        closeAllExcept(null);
                        break;
                    }
            }
        }

        public void addElementTreeActionGroup(EazyTreeElement element, bool newElement)
        {
            element.CallBackTreeElement = delegate (EazyTreeElement target)
            {
                chooseActionTarget((EazyActionWithTarget)target.ObjecTarget);
            };
            element.CallBackRightClick = delegate (EazyTreeElement target)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Remove Action"),false, delegate()
                {
                    Tree.removeChild(target);
                });
                menu.ShowAsContext();
            };
            Tree.addChild(element, newElement);
        }
        #endregion
        #region API
        void OnGUI()
        {

            //_guiStyleScroll.normal.background.
            rectLeft = position;
            rectLeft.width = position.width - 250;
            Rect rectRight = new Rect(position.width - 248, 0, 250, position.height);
            if (ActionTarget != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.DrawRect(new Rect(0, 0, rectLeft.width, 20), new Color(204.0f / 255.0f, 204.0f / 255.0f, 204.0f / 255.0f));

                EditorGUILayout.LabelField("Object Action:", GUILayout.MaxWidth(100));

                ActionTarget.Target = EditorGUILayout.ObjectField(ActionTarget.Target, typeof(GameObject), true, GUILayout.MaxWidth(rectLeft.width - 120)) as GameObject;

                EditorGUILayout.EndHorizontal();

                _posScroll = EditorGUILayout.BeginScrollView(_posScroll, GUILayout.Width(rectLeft.width), GUILayout.Height(200));
                Rect screenBounds = new Rect(0, 0, rectLeft.width, 200);
                GUI.DrawTextureWithTexCoords(screenBounds, backgroundGrid, new Rect(0, 0, screenBounds.width / (float)backgroundGrid.width, screenBounds.height / (float)backgroundGrid.height));
                EditorGUIUtility.AddCursorRect(new Rect(0, 190, rectLeft.width, 10), MouseCursor.ResizeVertical);
                onDrawRectFocus(_mainAction.Drawer.getFocusRect());
                _mainAction.Drawer.onDrawInfoBase();
                EditorGUILayout.EndScrollView();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(rectLeft.width - 8));
                if (GUILayout.Button(_stringButtonPreview, GUILayout.MaxWidth((rectLeft.width - 8) / 4)))
                {
                    if (ActionTarget.Target && _mainAction.Info.ListActionInfo.Count > 0)
                    {
                        stopPreview = !stopPreview;
                        RootMotionController pRoot = ActionTarget.Target.GetComponent<RootMotionController>();
                        if (!pRoot)
                        {
                            pRoot = ActionTarget.Target.AddComponent<RootMotionController>();
                        }
                        if (stopPreview)
                        {
                            _actionInfo.setPlaying(true);
                            _stringButtonPreview = "Stop";
                            pRoot.runInEditMode = true;
                            pRoot.reNew();
                            pRoot.saveData();
                            pRoot.ObjectPreview.runInEditMode = true;
                            pRoot.runActionPreview(_mainAction.Info.covertAction());
                            EditorApplication.update += ActionTarget.Target.GetComponent<RootMotionController>().ObjectPreview.EditorUpdate;

                        }
                        else
                        {
                            EditorApplication.update -= pRoot.ObjectPreview.EditorUpdate;
                            pRoot.ObjectPreview.runInEditMode = false;
                            _actionInfo.setPlaying(false);
                            pRoot.resetData();
                            _stringButtonPreview = "Play";
                            pRoot.runInEditMode = false;
                            _stringButtonPause = "Pause";
                        }
                    }

                }

                EditorGUI.BeginDisabledGroup(!stopPreview);
                if (GUILayout.Button(_stringButtonPause, GUILayout.MaxWidth((rectLeft.width - 8) / 4)))
                {
                    pausePreview = !pausePreview;
                    RootMotionController pRoot = ActionTarget.Target.GetComponent<RootMotionController>();
                    if (!pRoot)
                    {
                        pRoot = ActionTarget.Target.AddComponent<RootMotionController>();
                    }
                    if (pausePreview)
                    {
                        pRoot.pauseActionByName(_mainAction.Info.Name);
                        _stringButtonPause = "Resume";
                    }
                    else
                    {
                        pRoot.resumeActionByName(_mainAction.Info.Name);
                        _stringButtonPause = "Pause";
                    }

                }

                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(ArrayAction.Count <= 1);
                if (GUILayout.Button("Clear", GUILayout.MaxWidth((rectLeft.width - 8) / 4)))
                {

                }
                if (GUILayout.Button("Export", GUILayout.MaxWidth((rectLeft.width - 8) / 4)))
                {
                    Action pExportData = delegate { };
                    if (ActionTarget.Target)
                    {
                        GenericMenu pMenu = new GenericMenu();
                        pMenu.AddItem(new GUIContent("Object"), false, delegate
                        {
                            _mainAction.Info.CopyAllTo(_actionInfoExport);
                        });
                        pMenu.AddItem(new GUIContent("Folder"), false, delegate { pExportData(); });
                        pMenu.ShowAsContext();
                    }
                    else
                    {
                        pExportData();
                    }
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
                _chooseTabIndex = GUILayout.Toolbar(_chooseTabIndex, _arrayTab, GUILayout.Width(rectLeft.width - 4));

                //if (_mainAction._infoString.Length > 0)
                //{
           
                if (_actionInfo.Info != null)
                {
                    _actionInfo._objectTest = ActionTarget.Target;
                    if (_InfoActionBehavior != TypeActionEditorBehavior.None)
                    {
                        _actionInfo.OnGui(rectLeft);
                        //EazyActionDrawExtend draw = new EazyActionDrawExtend();
                        //draw.Target = ObjectExport.GetComponent<RootMotionController>();
                        //draw.OnEnable();
                        if (!_actionInfo._disable && _actionInfo.Info.SelectedAction.name != "")
                        {
                            if (_InfoActionBehavior == TypeActionEditorBehavior.Add)
                            {
                                if (GUILayout.Button("Add", GUILayout.Width(rectLeft.width - 4)))
                                {
                                    addAction(_actionInfo.Info.ActionParrent.Id, _actionInfo.Info);
                                    _actionInfo.Info = null;
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Edit", GUILayout.Width(rectLeft.width - 4)))
                                {
                                    editAction(_actionInfo.Info.Id, _actionInfo.Info);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                EditorGUI.LabelField(new Rect(rectRight.size / 2 - new Vector2(40, 30), new Vector2(150, 20)), new GUIContent("dont have any action yet"));
            }

            GUI.Box(new Rect(position.width - 248, 0, 250, position.height), "");
            EditorGUI.BeginDisabledGroup(HaveTarget);
            EditorGUI.LabelField(new Rect(position.width - 245, 2, 90, 16), new GUIContent("Object Export:"));
            ObjectExport = EditorGUI.ObjectField(new Rect(position.width - 160, 2, 158, 16), ObjectExport, typeof(GameObject), true) as GameObject;
            EditorGUI.EndDisabledGroup();
            EditorGUI.DrawRect(new Rect(position.width - 248, 20, 250, 1), Color.black);
            if (Tree != null)
            {
                Tree = EazyGUITools.drawTree(Tree, new Vector2(position.width - 245, 22), new Vector2(250, 20));
            }
            Event ev = Event.current;
            if (ev.rawType == EventType.MouseUp)
            {
                if (ev.button == 1 && rectRight.Contains(ev.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Add New Action"), false, delegate
                    {
                        EazyActionInfo info = new EazyActionInfo();
                        info.init(EazyActionContructor.Sequences);
                        info.Name = ((EazyActionInfoGroup)Tree.ObjecTarget).Name;
                        EazyActionWithTarget actionTarget = new EazyActionWithTarget(info);
                        //info._name = "Action" + Tree.ChildCount;
                        EazyTreeElement element = new EazyTreeElement("Action" + Tree.ChildCount);
                        element.ObjecTarget = actionTarget;
                        addElementTreeActionGroup(element, true);
                        Tree.Open = true;
                        chooseActionTarget(actionTarget);
                    });
                    menu.ShowAsContext();
                }
            }
            this.Repaint();
        }

        //private void OnGUI()
        //{
        //    tree = EazyGUITools.drawTree(tree, new Vector2(0, 0), new Vector2(position.width, 20));
        //}
        private void OnDisable()
        {
            if (ActionTarget != null && ActionTarget.Target && ActionTarget.Target.GetComponent<RootMotionController>() != null)
            {
                if (ActionTarget.Target.GetComponent<RootMotionController>().ObjectPreview != null)
                {
                    EditorApplication.update -= ActionTarget.Target.GetComponent<RootMotionController>().ObjectPreview.EditorUpdate;
                }
                ActionTarget.Target.GetComponent<RootMotionController>().runInEditMode = false;
                ActionTarget.Target.GetComponent<RootMotionController>().resetData();
            }
        }

        private void OnEnable()
        {
            Tree = new EazyActionInfoTree("Action Group");
            _actionInfo = new EazyActionDraw();
            backgroundImage = Resources.Load("asset/eazy_focus", typeof(Texture2D)) as Texture2D;
            backgroundGrid = Resources.Load("asset/rect_grid", typeof(Texture2D)) as Texture2D;
            if (instance < 0)
            {
                instance = 0;
            }
            instance++;
            hideFlags = HideFlags.None;
            //if (ArrayAction == null)
            //{
            //    ArrayAction = new List<EazyDrawTree>();
            //    EazyActionInfo actionChild = CreateInstance<EazyActionInfo>();
            //    actionChild.init(ActionOption.Sequence);
            //    addAction(-1, actionChild);
            //}



        }

        private void OnDestroy()
        {
            instance--;
        }
        #endregion

    }



    public class EazyActionList : ReorderableListExtend
    {
        RootMotionController _objectTest;
        public EazyActionList(IList list, Type pTypeList,
     bool dragable = true, bool displayHeader = true, bool displayAddButton = true, bool displayRemoveButton = true) : base(list, pTypeList,
      dragable, displayHeader, displayAddButton, displayRemoveButton)
        {

        }

        public RootMotionController ObjectTest
        {
            get
            {
                return _objectTest;
            }

            set
            {
                _objectTest = value;
            }
        }

        protected override void OnAdd(ReorderableList list, object newElement, Type pType)
        {

            base.OnAdd(list, newElement, pType);
            if (pType == typeof(EazyActionInfoGroup))
            {
                EazyActionInfoGroup action = newElement as EazyActionInfoGroup;
                action.init();
                action.Name = "Action" + list.count;
            }
            EditorUtility.SetDirty(_objectTest);
        }
        protected override void OnDrawElement(Rect rect, int index, bool active, bool focused, object pObject, Type pType)
        {
                EazyActionInfoGroup actionInfo = (EazyActionInfoGroup)pObject;
            rect.width += 30;
            Rect pRectPanel = new Rect(rect.x, rect.y, 100, rect.height);
            EditorGUI.LabelField(pRectPanel, "Eazy Action " + index.ToString() + ":");
            if (GUI.Button(new Rect(rect.width - 50, rect.y + 1, 50, 16), "Edit"))
            {
                bool isOpen = EazyActionWinDow.isOpen;
                EazyActionWinDow window = EazyActionWinDow.ShowWindow();
                window.HaveTarget = true;
                //window.ActionInfoExport = actionInfo.ac;
                // window.MainAction = (EazyActionInfo)window.ActionInfoExport.Clone();
                window.reNew();
                window.Tree.ObjecTarget = actionInfo;
                for (int i = 0; i < actionInfo.ArrayAction.Count; ++i)
                {
                    EazyTreeElement element = new EazyTreeElement("Action " + window.Tree.ChildCount);
                    element.ObjecTarget = actionInfo.ArrayAction[i];
                    window.addElementTreeActionGroup(element, false);
                }
                window.Tree.Name = "Action Group: " + actionInfo.Name;
                window.ObjectExport = ObjectTest.gameObject;
                //window.
                window.Show();
            }
            Rect pRectName = new Rect(pRectPanel.xMax, pRectPanel.y + 2, 100, pRectPanel.height - 4);
            string pNameAction = actionInfo.Name;
            if (pNameAction == "")
            {
                pNameAction = "None";
            }
            actionInfo.Name = EditorGUI.TextField(pRectName, pNameAction);
            Rect pRectRunDefault = new Rect(pRectName.xMax + 30, pRectPanel.y + 2, 20, pRectPanel.height - 4);
            actionInfo.IsDefault = EditorGUI.Toggle(pRectRunDefault, actionInfo.IsDefault);

        }
        protected override void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Eazy Action");
            rect.x += 110;
            EditorGUI.LabelField(rect, "Name");
            rect.x += 120;
            EditorGUI.LabelField(rect, "Default");
        }
    }

    [CustomEditor(typeof(RootMotionController))]
    [CanEditMultipleObjects]
    public class EazyActionInspector : Editor
    {
        List<EazyActionInfoGroup> _arrayAction;
        EazyActionList m_reorder_list;
        private void OnEnable()
        {
            if (((RootMotionController)target)._arrayAction == null)
            {
                ((RootMotionController)target)._arrayAction = new List<EazyActionInfoGroup>();
            }
            _arrayAction = ((RootMotionController)target)._arrayAction;
            m_reorder_list = new EazyActionList(_arrayAction, typeof(EazyActionInfoGroup));
            m_reorder_list.ObjectTest = (RootMotionController)target;
        }
        public override void OnInspectorGUI()
        {
            if (_arrayAction.Count > 0)
            {
                int ab = 0;
                ab++;
            }
            DrawDefaultInspector();
            serializedObject.Update();
            m_reorder_list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button(new GUIContent("clear Cache")))
            {
                //EazyActionInfo[] cache = new EazyActionInfo();
                //for (int i = 0; i < cache.Length; i++)
                //{
                //    DestroyImmediate(cache[i]);
                //}
            }
        }
    }

    public class EazyActionInfoTree : EazyTreeElement
    {
        public EazyActionInfoTree() : base()
        {
            Name = "none";
        }
        public EazyActionInfoTree(string name, object target = null, TreeElementCallBack callback = null) : base(name, target, callback)
        {
            ObjecTarget = target;
            CallBackTreeElement = callback;
            Name = name;
        }

        public override void onAddElement(EazyTreeElement element)
        {
            object target = element.ObjecTarget;
            if (target != null && ObjecTarget != null)
            {
                EazyActionWithTarget action = target as EazyActionWithTarget;
                EazyActionInfoGroup actionGroup = ObjecTarget as EazyActionInfoGroup;
                action.Info.Name = actionGroup.Name;
                actionGroup.ArrayAction.Add(action);
            }
        }

        public override void onRemoveElement(EazyTreeElement element)
        {
            object target = element.ObjecTarget;
            if (target != null && ObjecTarget != null)
            {
                EazyActionWithTarget action = target as EazyActionWithTarget;
                EazyActionInfoGroup actionGroup = ObjecTarget as EazyActionInfoGroup;
                actionGroup.ArrayAction.Remove(action);
            }
        }

        public override void onDrawTreeElement(Vector2 pos, Vector2 sizeOut)
        {
            if (MainTree && ObjecTarget != null)
            {
                Name = "Action Group: " + ((EazyActionInfoGroup)ObjecTarget).Name;
            }
            base.onDrawTreeElement(pos, sizeOut);
        }
    }
}

