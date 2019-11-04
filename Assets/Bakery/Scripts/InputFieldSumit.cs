using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputFieldSumit : InputField {
    [SerializeField]
    UnityEvent _onSubmit;

    protected override void Start()
    {
        base.Start();

        this.onEndEdit.AddListener(delegate (string text) {
            if (!EventSystem.current.alreadySelecting)
            {
                _onSubmit.Invoke();
            }
        });
    }
}
