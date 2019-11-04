using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class ButtonCustom : MonoBehaviour
{
    public Color buttonNColor = Color.grey;
    public Color buttonHColor = Color.white;


    public bool isWorldSpace = false;
    public UnityEvent onClicked;
    private Vector3 sizeMain;
    public SpriteRenderer _render;
    void Start()
    {
        if (!gameObject.GetComponent<BoxCollider2D>())
        {
            BoxCollider2D colider = gameObject.AddComponent<BoxCollider2D>();
            colider.isTrigger = true;
        }
        sizeMain = new Vector3(Camera.main.orthographicSize * 2 * Screen.width / Screen.height, Camera.main.orthographicSize * 2, 0);
        _render = GetComponent<SpriteRenderer>();
        _render.color = buttonHColor;
    }
    void Update()
    {
         
            Vector2 size = new Vector2(0, 0);
            if (_render)
            {
                size = _render.bounds.size;
            }
            Vector3 mPos = new Vector3(-1000, -1000, 0);
#if UNITY_ANDROID || UNITY_IOS 
                Touch[] touches = Input.touches;
                if(touches.Length > 0)
                {
                    mPos = touches[0].position;
                }
#endif
#if UNITY_EDITOR 
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                mPos = Input.mousePosition;
            }
#endif
            if (mPos.x != -1000.0f)
            {
                Vector3 wp = Camera.main.ScreenToWorldPoint(mPos);
                Vector2 touchPos = new Vector2(wp.x, wp.y);
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector3.zero);
                if (hit && hit.collider.gameObject == gameObject)
                {  
               
                    if (_render)
                    {
                        if (_render.color == buttonHColor && Input.GetMouseButtonDown(0))
                        {
                            _render.color = buttonNColor;
                        }
                        else
                        {
                            handleHit();
                            _render.color = buttonHColor;
                        }
                    }
                }
                else
                {
                    if (_render)
                    {
                        _render.color = buttonHColor;
                    }
                }
            }
            else if (hit)
            {
                handleHit();
                hit = false;
            }
            else if (_render)
            {
               // _render.color = buttonHColor;
            }
        
    }

    void handleHit()
    {
    
#if UNITY_ANDROID || UNITY_IOS
        
                if(Input.touchCount > 0)
                {
                    Touch touches = Input.GetTouch(0);
                    if(touches.phase == TouchPhase.Ended)
                    {
                          Action();
                     }
                }
#endif
#if UNITY_EDITOR
        Action();
#endif
    }

    void Action()
    {
        if (onClicked != null)
        {
            onClicked.Invoke();
        }
    }
    private bool hit = false;
    void beingHit()
    {
        hit = true;
    }
}