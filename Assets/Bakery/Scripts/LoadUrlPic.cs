using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadUrlPic : MonoBehaviour {
    [SerializeField]
    Image skin;

    Dictionary<string, Texture2D> cachePic = new Dictionary<string, Texture2D>();

    public void setLink(string pic)
    {
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
                skin.sprite = Sprite.Create( pText,new Rect(0,0,pText.width,pText.height),new Vector2(0,0));
            }
        }
        else
        {
            skin.sprite = null;
        }
    }

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
            skin.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        }
    }
}
