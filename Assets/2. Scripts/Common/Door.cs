using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Door : MonoBehaviour
{
    private HingeJoint hinge;
    private JointLimits limits;
    private Rigidbody rigidbody;
    private bool hold = true;
    public float doorAngle = 0;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        hinge = GetComponent<HingeJoint>();
        limits = hinge.limits;
        limits.max = doorAngle;
    }

    private void Update()
    {
        if (hold)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }

    public void DoorOpen()
    {
        hinge.limits = limits;
        hold = false;
    }
}
