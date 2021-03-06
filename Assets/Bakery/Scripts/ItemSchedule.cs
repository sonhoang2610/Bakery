﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ProductStatusInfo
{
    public string productID;
    public int quantitySchedule, quantityUsed;
    public ProductStatusInfo(string productID, int quantitySchedule, int quantityUsed)
    {
        this.productID = productID;
        this.quantitySchedule = quantitySchedule;
        this.quantityUsed = quantityUsed;
    }
    public ProductStatusInfo()
    {
        this.productID = "";
        this.quantitySchedule = 0;
        this.quantityUsed = 0;
    }
}
public class ItemSchedule : BaseItem<ProductStatusInfo>
{
    [SerializeField]
    Image skin;
    [SerializeField]
   public  InputField labelQuantity;

    public override void SetData(ProductStatusInfo pInfo)
    {
        if (_info == pInfo)
        {
            return;
        }
        base.SetData(pInfo);
    
        string pic = ProductConfig.nameProduct[pInfo.productID].pic;
        if (!string.IsNullOrEmpty(pic))
        {
            string pFileName = pic.Replace("/", string.Empty).Replace(".", string.Empty).Replace(":", string.Empty);
            Texture2D pText = null;
            if (!(pText = LoadTextureToFile(pFileName)))
            {
                StartCoroutine(loadSpriteImageFromUrl(pic));
            }
            else
            {
                skin.sprite = Sprite.Create(pText, new Rect(0.0f, 0.0f, pText.width, pText.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
        }
        labelQuantity.text =  pInfo.quantitySchedule.ToString();
        labelQuantity.onEndEdit.AddListener((string pEdit) => {
            int pNumber = 0;
            if(int.TryParse(pEdit,out pNumber))
            {
                _info.quantitySchedule = pNumber;
            }
        });
    }

    static Dictionary<string, Texture2D> cachePic = new Dictionary<string, Texture2D>();

    public Texture2D LoadTextureToFile(string filename)
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/Pic/" + filename))
        {
            if (!cachePic.ContainsKey(filename))
            {
                byte[] bytes;
                bytes = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/Pic/" + filename);
                Texture2D load_s01_texture = new Texture2D(1, 1);
                load_s01_texture.LoadImage(bytes);
                cachePic.Add(filename, load_s01_texture);
                return load_s01_texture;
            }
            else
            {
                return cachePic[filename];
            }
        }
        else
        {
            return null;
        }
    }

    IEnumerator loadSpriteImageFromUrl(string URL)
    {

        WWW www = new WWW(URL);
        while (!www.isDone)
        {
            Debug.Log("Download image on progress" + www.progress);
            yield return null;
        }

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Download failed");
        }
        else
        {
            Debug.Log("Download succes");
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
            texture.LoadImage(www.bytes);
            texture.Apply();
            string pFileName = URL.Replace("/", string.Empty).Replace(".", string.Empty).Replace(":", string.Empty);
            //     FacebookManager.Instance.SaveNonPNGTextureToFile(texture, pFileName);
            skin.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }
}