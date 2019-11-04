using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityToolbag;

public enum TypeProgress { Horizontal, Vertical, Circle };
//[ExecuteInEditMode]
public class ProgressController : CacheBehaviour
{
    private CacheTexture2D _cacheTexture;
    public Sprite _Sprite;
    public string _currentSprite;
    [SerializeField]
    ProgressChild _childProgress;
    [SerializeField]
    private TypeProgress _typeProgress = TypeProgress.Vertical;

	public TypeProgress TypeCut { get { return _typeProgress; } set {
		if(_childProgress && _childProgress.renderer.sharedMaterial){
			_childProgress.renderer.sharedMaterial.SetFloat("_TypeFill",(float)value);
		}
		_typeProgress = value;
	} }

    [SerializeField]
    [Range(0, 1)]
    private float _percentage = 0;

	public float Percentage { get { return _percentage; } set {
		if(_childProgress && _childProgress.renderer.sharedMaterial){
			_childProgress.renderer.sharedMaterial.SetFloat("_FillAmount",value);
		}
		_percentage = value;
	} }

    [SerializeField]
    private Vector2 _midPoint = Vector2.zero;

    public Vector2 MidPoint
    {
        get { return _midPoint; }
        set
        {
            value.x = ((value.x != 0 && value.x != 1) ? 0.5f : value.x);
            value.y = ((value.y != 0 && value.y != 1) ? 0.5f : value.y);
            if (_childProgress && _childProgress.renderer.sharedMaterial)
            {
                _childProgress.renderer.sharedMaterial.SetVector("_MidPoint", value);
            }
            _midPoint = value;
        }
    }

    [SerializeField]
    private Vector2 _anchorPoint  = new Vector2(0.5f, 0.5f);

    public Vector2 AnchorPoint 
    {
        get { return _anchorPoint; }
        set
        {
            if (_childProgress)
            {
                Vector3 pPos = _childProgress.transform.localPosition;
                pPos.x = (0.5f -value.x) * _childProgress.transform.localScale.x;
                pPos.y = (0.5f - value.y) * _childProgress.transform.localScale.y;
                _childProgress.transform.localPosition = pPos;
            }
            _anchorPoint = value;
        }
    }

    [SerializeField]
    private int _side = 1;

    public int SideCircle { get { return _side; } set { _side = value; } }


    // This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
    protected void OnValidate()
    {
	    //updateSkin();
	    //TypeCut = _typeProgress;
	    //Percentage = _percentage;
     //   MidPoint = _midPoint;
     //   AnchorPoint = _anchorPoint;
    }

    void updateSkin()
    {
        if (_Sprite && _childProgress)
        {
            Texture2D croppedTexture = _Sprite.texture;
            if (croppedTexture.width != _Sprite.rect.width || croppedTexture.height != _Sprite.rect.height)
            {
                croppedTexture = _cacheTexture.getTextureByName(_Sprite.name);
        
                if (!croppedTexture)
                {
                    croppedTexture = new Texture2D((int)_Sprite.textureRect.width, (int)_Sprite.textureRect.height);
                    croppedTexture.name = _Sprite.name;
                    var pixels = _Sprite.texture.GetPixels((int)_Sprite.textureRect.x,
                    (int)_Sprite.textureRect.y,
                    (int)_Sprite.textureRect.width,
                    (int)_Sprite.textureRect.height);
                    croppedTexture.SetPixels(pixels);
                    croppedTexture.Apply();
                    _cacheTexture.addTexture(croppedTexture);
                }
            }
            _childProgress.renderer.sharedMaterial.SetTexture("_MainTex", croppedTexture);
            _childProgress.transform.localScale = new Vector3(_Sprite.bounds.size.x, _Sprite.bounds.size.y, 1);
        }
    }

    public static ProgressController create(string pPathSprite)
    {
        ProgressController pProgress = Resources.Load("CustomSupport/prefab/ProgressControll", typeof(ProgressController)) as ProgressController;
        pProgress.setUpInfo(pPathSprite);
        return pProgress;
    }

    public void setUpInfo(string pStrSprite, TypeProgress pType = TypeProgress.Horizontal)
    {
        _Sprite = Resources.Load(pStrSprite, typeof(Sprite)) as Sprite;
        TypeCut = pType;
    }

    // Awake is called when the script instance is being loaded.
    protected void Awake()
	{
		
        _cacheTexture = new CacheTexture2D();
        Material material = new Material(Shader.Find("Custom/Cropped"));
        _childProgress.renderer.sharedMaterial = material;
    }

    // Use this for initialization
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (_Sprite && _currentSprite != _Sprite.name)
        {
            _currentSprite = _Sprite.name;
            updateSkin();
        }
    }
}
