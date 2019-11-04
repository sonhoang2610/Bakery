using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EazyAsset {

	public static void  loadEazyPrefab(string path,int pCount,Vector2 sizecell,int col,bool isHorizontal,Transform transform)
    {
        string _path = path;
        for(int i = 0; i < pCount; i++)
        {
            _path = string.Format(path, i);
            GameObject pObject = Resources.Load(_path, typeof(GameObject)) as GameObject;
            pObject = Object.Instantiate(pObject, transform);
            EazyObject eazyObject = pObject.GetComponent<EazyObject>();
            if (eazyObject)
            {
                eazyObject.initIndex(i);
            }
            pObject.name = _path + i;
            pObject.transform.setLocalPosition2D(new Vector2((i % col) * sizecell.x, (i / col) * sizecell.y));
        }    
    }
}
