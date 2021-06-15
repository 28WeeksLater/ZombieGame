using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : EventHandler
{
    private void OnTriggerEnter(Collider other)
    {
        if (!isClear && other.transform.CompareTag("Player"))
        {
            _event.Invoke();
            isClear = true;
        }
    }
}
