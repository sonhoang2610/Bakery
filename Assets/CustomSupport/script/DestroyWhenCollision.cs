using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenCollision : MonoBehaviour
{
    [SerializeField]
    LayerMask _mask;
    [SerializeField]
    bool destoryObject = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (destoryObject)
        {
            Destroy(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
