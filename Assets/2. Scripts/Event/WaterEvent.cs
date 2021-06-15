using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaterEvent : MonoBehaviour
{
    public UnityEvent Drown;
    private float initTime = 0.0f;
    private float delay = 3.0f;
    private bool isDrown;
    private void Update()
    {
        if (initTime < delay && !isDrown)
        {
            initTime += Time.deltaTime;
        }
        else if (!isDrown)
        {
            isDrown = true;
            initTime = 0.0f;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && isDrown)
        {
            Drown.Invoke();
            isDrown = false;
            Debug.Log("Collide");
        }
        else if(other.transform.CompareTag("Zombie") || other.transform.CompareTag("Food"))
        {
            Destroy(other.gameObject);
        }
    }
}
