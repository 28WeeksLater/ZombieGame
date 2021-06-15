using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomOutEvent : EventHandler
{
    private void OnTriggerEnter(Collider other)
    {
        if (!isClear && other.transform.CompareTag("Player") && isActivated)
        {
            isClear = true;
            _event.Invoke();
        }
    }

    public void Active()
    {
        isActivated = true;
    }
}
