using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class XRSocketInteractorTag : XRSocketInteractor
{
    [Header("Target's Tag")]
    [SerializeField] private string targetTag;

    public override bool CanSelect(XRBaseInteractable interactable)
    {
        return base.CanSelect(interactable) && interactable.CompareTag(targetTag);
    }
}


