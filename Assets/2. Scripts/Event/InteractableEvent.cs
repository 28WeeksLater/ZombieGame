using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractableEvent : EventHandler
{
    private XRGrabInteractable _interactable;

    private void Start()
    {
        _interactable = GetComponent<XRGrabInteractable>();
    }

    private void Update()
    {
        if (_interactable && !isActivated)
        { 
            if (_interactable.isSelected)
            {
                isActivated = true;
                _event.Invoke();
            }
        }
    }
}
