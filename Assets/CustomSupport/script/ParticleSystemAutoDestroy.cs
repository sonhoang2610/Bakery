using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleSystemAutoDestroy : MonoBehaviour {

    private ParticleSystem ps;
    public float forceDestroyAfter = -1;
    public UnityEvent onStart;
    public UnityEvent onDestroy;
    float currentTime = 0;
    public void Start()
    {
        ps = GetComponent<ParticleSystem>();

    }
    private void OnEnable()
    {
        currentTime = forceDestroyAfter;
        onStart.Invoke();
    }
    public void Update()
    {
        if(currentTime >= 0)
        {
            currentTime -= Time.deltaTime;
            if(currentTime <= 0)
            {
                gameObject.SetActive(false);
                onDestroy.Invoke();
            }
        }
        if (ps)
        {
            if (!ps.IsAlive())
            {
                gameObject.SetActive(false);
                onDestroy.Invoke();
            }
        }
    }
}
