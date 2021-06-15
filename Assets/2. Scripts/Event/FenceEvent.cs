using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FenceEvent : EventHandler
{
    public Transform leftFence;
    public Transform rightFence;
    public GameObject leftFenceDestroyed;
    public GameObject rightFenceDestroyed;

    public void FenceDestroy()
    {
        leftFence.rotation = Quaternion.Euler(85, -85, 0);
        rightFence.rotation = Quaternion.Euler(-85, -85, 0);
        Destroy(leftFenceDestroyed);
        Destroy(rightFenceDestroyed);
    }
}
