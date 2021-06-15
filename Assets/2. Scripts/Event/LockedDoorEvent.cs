using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LockedDoorEvent : MonoBehaviour
{
    public EventHandler handler;
    public AudioSource speaker;
    public AudioSource Door;
    private XRGrabInteractable grab;
    private HingeJoint hinge;
    private void Start()
    {
        grab = GetComponent<XRGrabInteractable>();
        hinge = GetComponent<HingeJoint>();
    }
    public void openDoor()
    {
        StartCoroutine(open());
    }

    IEnumerator open()
    {
        yield return new WaitUntil(() => handler.isClear);
        Door.Play();
        grab.enabled = true;
        speaker.enabled = false;
        JointLimits limits = hinge.limits;
        limits.max = 100;

        hinge.limits = limits;
    }
}
