using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using UnityEditor;

namespace Helper.Editor
{


#if UNITY_EDITOR


    #region Docker
    public static class Docker
    {

        #region Reflection Types
        private class _EditorWindow
        {
            private EditorWindow instance;
            private Type type;

            public _EditorWindow(EditorWindow instance)
            {
                this.instance = instance;
                type = instance.GetType();
            }

            public object m_Parent
            {
                get
                {
                    //for (int i = 0; i < type.GetMembers().Length; i++)
                    //{
                    //    Debug.Log("abcd:" + type.GetMembers()[i]);
                    //}
                    var field = type.GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);
                    return field.GetValue(instance);
                }
            }
        }

        private class _DockArea
        {
            private object instance;
            private Type type;

            public _DockArea(object instance)
            {
                this.instance = instance;
                type = instance.GetType();
            }

            public int CountChild()
            {
              
                var method = type.GetMethod("get_allChildren", BindingFlags.Instance | BindingFlags.Public);
                object pAllChild =  method.Invoke(instance, null);
                if(pAllChild != null)
                {
                    object[] allChild =(object[]) pAllChild;
                    return allChild.Length;
                }
                return 0;
            }

            public void PerformDrop(EditorWindow child, object dropInfo, Vector2 screenPoint)
            {
       
                var method = type.GetMethod("PerformDrop", BindingFlags.Instance | BindingFlags.Public);
                method.Invoke(instance, new object[] { child, dropInfo, screenPoint });
            }

            public object window
            {
                get
                {
                    var property = type.GetProperty("window", BindingFlags.Instance | BindingFlags.Public);
                    return property.GetValue(instance, null);
                }
            }

            public object s_OriginalDragSource
            {
                set
                {
                    var field = type.GetField("s_OriginalDragSource", BindingFlags.Static | BindingFlags.NonPublic);
                    field.SetValue(null, value);
                }
            }
        }

        private class _ContainerWindow
        {
            private object instance;
            private Type type;

            public _ContainerWindow(object instance)
            {
                this.instance = instance;
                type = instance.GetType();
            }


            public object rootSplitView
            {
                get
                {
     
                    var property = type.GetProperty("rootSplitView", BindingFlags.Instance | BindingFlags.Public);

                    return property.GetValue(instance, null);
                }
            }
        }

        private class _SplitView
        {
            private object instance;
            private Type type;

            public _SplitView(object instance)
            {
                this.instance = instance;
                type = instance.GetType();
            }

            public object DragOver(EditorWindow child, Vector2 screenPoint)
            {

                object abc = type.GetProperty("allChildren").GetValue(instance, null);
                var method = type.GetMethod("DragOver", BindingFlags.Instance | BindingFlags.Public);
                return method.Invoke(instance, new object[] { child, screenPoint });
            }

            public void PerformDrop(EditorWindow child, object dropInfo, Vector2 screenPoint)
            {
                var method = type.GetMethod("PerformDrop", BindingFlags.Instance | BindingFlags.Public);
                method.Invoke(instance, new object[] { child, dropInfo, screenPoint });
            }
        }
        #endregion

        public enum DockPosition
        {
            Left,
            Top,
            Right,
            Bottom,
            Tab
        }

        /// <summary>
        /// Docks the second window to the first window at the given position
        /// </summary>
        public static void Dock(this EditorWindow wnd, EditorWindow other, DockPosition position)
        {
            var mousePosition = GetFakeMousePosition(wnd, position);

            var parent = new _EditorWindow(wnd);
            var child = new _EditorWindow(other);
            var dockArea = new _DockArea(parent.m_Parent);
            var containerWindow = new _ContainerWindow(dockArea.window);
            var splitView = new _SplitView(containerWindow.rootSplitView);
            var dropInfo = splitView.DragOver(other, mousePosition);
            dockArea.s_OriginalDragSource = child.m_Parent;
            if (position != DockPosition.Tab)
            {
                splitView.PerformDrop(other, dropInfo, mousePosition);
            }
            else
            {
                dockArea.PerformDrop(other, dropInfo, mousePosition);
            }
        }

        public static int CountChild(this EditorWindow v)
        {
            var parent = new _EditorWindow(v);
            var dockArea = new _DockArea(parent.m_Parent);
            return  dockArea.CountChild();          
        }

        private static Vector2 GetFakeMousePosition(EditorWindow wnd, DockPosition position)
        {
            Vector2 mousePosition = Vector2.zero;
            Vector2 pPos = new Vector2(wnd.position.x, wnd.position.y);
            // The 20 is required to make the docking work.
            // Smaller values might not work when faking the mouse position.
            switch (position)
            {
                case DockPosition.Left:
                    mousePosition = new Vector2(20, wnd.position.size.y / 2);
                    break;
                case DockPosition.Top:
                    mousePosition = new Vector2(wnd.position.size.x / 2, 20);
                    break;
                case DockPosition.Right:
                    mousePosition = new Vector2(wnd.position.size.x - 20, wnd.position.size.y / 2);
                    break;
                case DockPosition.Bottom:
                    mousePosition = new Vector2(wnd.position.size.x / 2, wnd.position.size.y - 20);
                    break;
                case DockPosition.Tab:
                    mousePosition = new Vector2(wnd.position.size.x  - 20, 18);
                    break;
            }

            return pPos + mousePosition;
        }
    }
    #endregion
#endif
}
