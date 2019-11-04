using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorCache : MonoBehaviour {

    public Color[] colors;
#if NGUI
    UIWidget widget;
    UIWidget Widget
    {
        get
        {
            return widget ? widget : widget = GetComponent<UIWidget>();
        }
    }
#endif
    public void setColorIndex(int index)
    {
        if(colors  != null && index < colors.Length)
        {
#if NGUI
            Widget.color = colors[index];
#endif
            if (GetComponent<Image>())
            {
                GetComponent<Image>().color = colors[index];
            }
            if (GetComponent<Text>())
            {
                GetComponent<Text>().color = colors[index];
            }
        }
    }
}
