using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HealEvent : EventHandler
{
    public XRGrabInteractable interactable;

    private void Update()
    {
        if (!isClear && interactable.isSelected)
        {
            _event.Invoke();
            isClear = true;
        }
    }
}
