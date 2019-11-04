using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class FillColor : MonoBehaviour {
	[SerializeField]
	Color colorFill;
    [SerializeField]
    bool allowIgnoreSetting = false;
    public UnityEvent onStartLoad, onComplete;
    protected MaterialPropertyBlock block;
	protected Renderer meshRenderer;
#if NGUI
    UI2DSprite _sprite;
#endif
    public Renderer MeshRenderer
    {
		get{
			return meshRenderer ? meshRenderer : meshRenderer = GetComponent<Renderer>();
		}
	}
	// Awake is called when the script instance is being loaded.
	protected void Awake()
	{
        //if (GameManager.Instance.Graphic || allowIgnoreSetting)
        //{
        if (MeshRenderer && meshRenderer.GetType() != typeof(SpriteRenderer))
        {
            block = new MaterialPropertyBlock();
            MeshRenderer.SetPropertyBlock(block);
        }
#if NGUI
        _sprite = GetComponent<UI2DSprite>();
#endif
        //}
    }
    protected IEnumerator FlashRoutine(float duration)
	{
		
        // You can use these instead of strings.
		int fillAlpha = 0;
		int fillColor = 0;
		
		if (block != null)
		{
			fillAlpha = Shader.PropertyToID("_FillPhase");
			fillColor = Shader.PropertyToID("_FillColor");
			block.SetFloat(fillAlpha, 0.4f); // Make the fill opaque.
			
			block.SetColor(fillColor,colorFill); // Fill with white.
		}
			meshRenderer.SetPropertyBlock(block);
		yield return new WaitForSeconds(duration);
		if (block != null)
		{
			block.SetFloat(fillAlpha, 0f); // Remove the fill.
		}
        MeshRenderer.SetPropertyBlock(block);	
	}
	
	[ContextMenu("test")]
	public void flashTest(){
		flash(5);
	}
	
	public void flash(float pDuration){
            StartCoroutine(FlashRoutine(pDuration));
	}
	
	public void fill(float pAlpha){
		int fillAlpha = 0;
		int fillColor = 0;
        if(pAlpha > 0)
        {
            onStartLoad.Invoke();
        }
        else
        {
            onComplete.Invoke();
        } 
		if (block != null && MeshRenderer.GetType() != typeof(SpriteRenderer))
		{
			fillAlpha = Shader.PropertyToID("_FillPhase");
			fillColor = Shader.PropertyToID("_FillColor");
			block.SetFloat(fillAlpha,pAlpha); // Make the fill opaque.
			
			block.SetColor(fillColor,colorFill); // Fill with white.
            meshRenderer.SetPropertyBlock(block);
        }
        else if(MeshRenderer.GetType() == typeof(SpriteRenderer))
        {
            Color pColor = ((SpriteRenderer)MeshRenderer).color;
            pColor = colorFill;
            pColor.a = pAlpha;
            if(pAlpha == 0)
            {
                pColor = Color.white;
            }
            ((SpriteRenderer)MeshRenderer).color = pColor;
        }
        else 
        {
#if NGUI
            _sprite = GetComponent<UI2DSprite>();
            Color currentColor = _sprite.color;
            currentColor.a = pAlpha;
            _sprite.color = currentColor;
#endif
        }
      
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
