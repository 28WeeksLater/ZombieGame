using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Outline))]
public class OutlineManager : MonoBehaviour
{
    private Outline outline;
    private XRBaseInteractable interactable;

    private void Start()
    {
        outline = GetComponent<Outline>();
        interactable = GetComponent<XRBaseInteractable>();
    }

    private void Update()
    {
        CheckOutline();
    }

    private void CheckOutline()
    {
        if (interactable.isSelected)
            outline.enabled = false;
        else if (interactable.isHovered)
            outline.enabled = true;
        else
            outline.enabled = false;
    }
}
