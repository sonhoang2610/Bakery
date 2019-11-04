using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EazyEngine.Tools;

public class BoxDialog : Singleton<BoxDialog>
{
    public Button buttonOK;

    public void showWarning(System.Action pACtion)
    {
        gameObject.SetActive(true);
        buttonOK.onClick.RemoveAllListeners();
        buttonOK.onClick.AddListener(delegate { pACtion(); });
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
