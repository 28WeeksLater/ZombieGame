using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class OnTargetReached : MonoBehaviour
{
    //장전 관련 함수

    [Header("Threshold & Target Position Setting")]
    public float threshold = 0.02f;
    public Transform target;
    public UnityEvent OnReached;
    private XRBaseInteractable interactable;

    private bool wasReached = false;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
    }
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if(distance < threshold && !wasReached)
        {
            OnReached.Invoke();
            wasReached = true;
            if(interactable.selectingInteractor)
                interactable.selectingInteractor.GetComponent<XRBaseController>().SendHapticImpulse(0.7f, 0.05f);
        }
        else if(distance >= threshold)
        {
            wasReached = false;
        }
    }

}
