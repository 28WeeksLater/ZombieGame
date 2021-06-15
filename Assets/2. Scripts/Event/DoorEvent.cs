using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorEvent : EventHandler
{
    public float rotY;
    protected HingeJoint hinge;
    public float angle;
    protected virtual void Start()
    {
        hinge = GetComponent<HingeJoint>();
    }

    public void Update()
    {
        angle = hinge.angle;
        if (!isActivated && rotY <= Mathf.Abs(hinge.angle))
        {
            _event.Invoke();
            isActivated = true;
        }
    }
}
