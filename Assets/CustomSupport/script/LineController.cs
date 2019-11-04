using EazyCustomAction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LineController : MonoBehaviour
{
    [SerializeField]
    GameObject[] _nodePoint;
    [SerializeField]
    Vector3[] _pointLines;
    [SerializeField]
    float _speed;
    [SerializeField]
    LineRenderer prefabLine;
    [SerializeField]
    GameObject _nodeLine;
    [SerializeField]
    bool isLocal = true;

    public GameObject[] NodePoint
    {
        get
        {
            return _nodePoint;
        }
    }

    LineRenderer[] _lines;
    int currentState = 0;
    bool isstartDraw = false;
    float currentPercent = 0;
    Vector3[] _pointLocals;
    [ReadOnly]
    [SerializeField]
    List<LineRenderer> cacheLine = new List<LineRenderer>();
    List<GameObject> nodes = new List<GameObject>();

    [ContextMenu("intial Line")]
    public void intialLineNow()
    {
        for (int i = 0; i < _nodePoint.Length - 1; ++i)
        {
            var pLine = Instantiate(prefabLine, transform);
            pLine.useWorldSpace = true;
            cacheLine.Add(pLine);
        }
    }

    [ContextMenu("Clear Line")]
    public void clearLine()
    {
        for (int i = cacheLine.Count - 1; i >= 0; --i)
        {
            var pLine = cacheLine[i];
            cacheLine.RemoveAt(i);
            if (pLine != null)
            {
                DestroyImmediate(pLine.gameObject);
            }
        }
    }
    Renderer[] _allRenders;
    private void Awake()
    {
        clearLine();
        intialLineNow();
        _allRenders = gameObject.GetComponentsInChildren<Renderer>(); 
    }

    public void setRenderOrder(int pOder)
    {
        for (int i = 0; i < cacheLine.Count; ++i)
        {
            var renders = cacheLine[i].GetComponentsInChildren<Renderer>();
            foreach (var pRender in renders)
            {
                pRender.material.renderQueue = pOder;
                if(pRender.GetType() == typeof(SpriteRenderer))
                {
                    pRender.material.renderQueue = pOder + 1;
                }
            }
            //var widgets = cacheLine[i].GetComponentsInChildren<UIWidget>();
            //foreach (var pWidget in widgets)
            //{
            //    if (pWidget.drawCall != null)
            //    {
            //        pWidget.drawCall.renderQueue = pOder;
            //    }
            //}
        }
    }
    public void setPoints(Vector3[] points, bool pLocal)
    {
        isLocal = pLocal;
        _pointLines = points;
        for (int i = 0; i < _pointLines.Length; ++i)
        {
            _pointLines[i] = isLocal ? transform.InverseTransformPoint(_pointLines[i]) : _pointLines[i];
        }
    }

    public LineRenderer obtainLine()
    {
        LineRenderer pLine = null;
        for (int i = 0; i < cacheLine.Count; ++i)
        {
            if (!cacheLine[i].gameObject.activeSelf)
            {
                pLine = cacheLine[i];
                break;
            }
        }
        if (pLine == null)
        {
            pLine = Instantiate(prefabLine, transform);
            cacheLine.Add(pLine);
        }
        return pLine;
    }

    public GameObject obtainNode()
    {
        GameObject pNode = null;
        for (int i = 0; i < nodes.Count; ++i)
        {
            if (!nodes[i].gameObject.activeSelf)
            {
                pNode = nodes[i];
                break;
            }
        }
        if (pNode == null)
        {
            pNode = Instantiate(_nodeLine, transform);
            pNode.name = "node";
            nodes.Add(pNode);
        }
        return pNode;
    }

    [ContextMenu("Start")]
    public void startDraw()
    {
        RootMotionController.stopAllAction(gameObject);
        currentState = 0;
        isstartDraw = true;
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                nodes[i].gameObject.SetActive(false);
            }
        }
        if (cacheLine != null)
        {
            for (int i = 0; i < cacheLine.Count; ++i)
            {
                Vector3[] pos = new Vector3[cacheLine[i].positionCount];
                for (int j = 0; j < pos.Length; ++j)
                {
                    pos[j] = Vector3.zero;
                }
                cacheLine[i].SetPositions(pos);
                cacheLine[i].gameObject.SetActive(false);
            }
        }
        if (_lines == null || _lines.Length == _pointLines.Length)
        {
            _pointLocals = new Vector3[_pointLines.Length];
            for (int i = 0; i < _pointLines.Length; ++i)
            {
                _pointLocals[i] =  _pointLines[i];
            }
            _lines = new LineRenderer[_pointLines.Length];
            for (int i = 0; i < _lines.Length; ++i)
            {
                _lines[i] = obtainLine();
                _lines[i].positionCount = 2;
                _lines[i].gameObject.SetActive(true);
                _lines[i].useWorldSpace = !isLocal;

            }
            for (int i = 0; i < _lines.Length; ++i)
            {
                if (i > 0)
                {
                    _lines[i].gameObject.SetActive(false);
                }
                else
                {
                    GameObject pNode = obtainNode();
                    pNode.SetActive(true);
                    if (isLocal)
                    {
                        pNode.transform.localPosition = _pointLocals[0];
                    }
                    else
                    {
                        pNode.transform.position = _pointLocals[0];
                    }
                }
            }
        }
    }
    private void Update()
    {
        if (Application.isPlaying)
        {
            if (isstartDraw)
            {
                float duration = Vector3.Distance(_pointLocals[currentState], _pointLocals[currentState + 1]) / _speed;
                currentPercent = (currentPercent * duration + Time.deltaTime) / duration;
                Vector3 pointDestiny = Vector3.Lerp(_pointLocals[currentState], _pointLocals[currentState + 1], currentPercent);
                if (currentPercent >= 1)
                {
                    pointDestiny = _pointLocals[currentState + 1];
                    GameObject pNode = obtainNode();
                    pNode.SetActive(true);
                    if (!isLocal)
                    {
                        pNode.transform.position = pointDestiny;
                    }
                    else
                    {
                        pNode.transform.localPosition = pointDestiny;
                    }
                    _lines[currentState].SetPositions(new Vector3[] { _pointLocals[currentState], pointDestiny });
                    currentState++;
                    _lines[currentState].gameObject.SetActive(true);
                    currentPercent = 0;
                    if (currentState >= _pointLocals.Length - 1)
                    {
                        isstartDraw = false;
                    }
                }
                else
                {
                    _lines[currentState].SetPositions(new Vector3[] { _pointLocals[currentState], pointDestiny });
                }

            }
        }
        if (_nodePoint != null)
        {
            if (cacheLine != null)
            {
                for (int i = 0; i < _nodePoint.Length - 1; ++i)
                {
                    if (i < cacheLine.Count)
                    {
                        cacheLine[i].gameObject.SetActive(true);
                        cacheLine[i].SetPositions(new Vector3[] { _nodePoint[i].transform.position, _nodePoint[i + 1].transform.position });
                    }
                }
            }
        }

    }

    float _alpha = 1;
    public float Alpha
    {
        set
        {
            for (int i = 0; i < cacheLine.Count; ++i)
            {
                if (cacheLine[i] == null) continue;
                Color[] pColors = new Color[] { cacheLine[i].startColor, cacheLine[i].endColor };
                pColors[0].a = value;
                pColors[1].a = value;
                cacheLine[i].startColor = pColors[0];
                cacheLine[i].endColor = pColors[1];
            }
            //for (int i = 0; i < nodes.Count; ++i)
            //{
            //    nodes[i].GetComponent<UIWidget>().alpha = value;
            //}
            _alpha = value;
        }
        get
        {
            return _alpha;
        }
    }

    int lastqueue = 0;
    public int RenderQueue
    {
        set
        {
            //if (lastqueue == value) return;
           // Debug.Log("change Queue Line");
            lastqueue = value;
            for (int i = 0; i < cacheLine.Count; ++i)
            {
                if (cacheLine[i] == null) continue;
                cacheLine[i].sharedMaterial.renderQueue = value;
            }
            if (_allRenders != null)
            {
                foreach (var pRender in _allRenders)
                {
                    if (pRender.GetType() == typeof(SpriteRenderer))
                    {
                        pRender.material.renderQueue = value + 1;
                    }
                }
            }
        }
    }

    public int SortOrder
    {
        set
        {
            if (cacheLine == null || cacheLine.Count == 0) return;
            for (int i = 0; i < cacheLine.Count; ++i)
            {
                if (cacheLine[i] != null)
                {
                    cacheLine[i].sortingOrder = value;
                }
            }
        }
    }
}

public class LineFade : EazyCustomAction.EazyFloatAction
{
    LineController line;
    public LineFade() : base()
    {
    }
    public LineFade(TypeBehaviorAction pBehavior) : this()
    {
        _typeActionBehavior = pBehavior;
    }

    public LineFade from(float pFrom)
    {
        base.setFrom(pFrom);
        return this;
    }
    public static LineFade to(float pDestiny, float pUnit, bool calculByTime = true)
    {

        LineFade pMove = new LineFade(TypeBehaviorAction.TO);
        pMove.setTo(pDestiny, pUnit, calculByTime);
        return pMove;
    }

    public override void extendCallBack(GameObject pObject)
    {
        if (line)
        {
            line.Alpha = _current;
        }
    }

   
    // Update is called once per frame
    public override void setUpAction(RootMotionController pRoot)
    {
        if (pRoot)
        {
            if (!line)
            {
                line = pRoot.GetComponent<LineController>();
            }
        }
        if (_typeActionBehavior < TypeBehaviorAction.FROM)
        {
            if (line)
            {
                _from = line.Alpha;
            }
        }
        base.setUpAction(pRoot);
    }
}
