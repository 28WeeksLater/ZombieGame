using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConcentEvent : EventHandler
{
    public Transform otherConcent;
    public Vector3 pos;
    public Vector3 rot;

    private void Update()
    {
        if(Vector3.Distance(otherConcent.position, transform.position) <= 0.3f && !isActivated)
        {
            isClear = true;
            isActivated = true;
            _event.Invoke();
        }
    }

    public void Contact()
    {
        var concentGrab = otherConcent.GetComponent<XRItemGrabInteractable>();
        concentGrab.DropIt();
        concentGrab.enabled = false;
        otherConcent.GetComponent<Rigidbody>().isKinematic = true;
        otherConcent.SetParent(transform);
        otherConcent.localPosition = pos;
        otherConcent.localRotation = new Quaternion(rot.x,rot.y,rot.z,otherConcent.localRotation.w);
    }
    
}
