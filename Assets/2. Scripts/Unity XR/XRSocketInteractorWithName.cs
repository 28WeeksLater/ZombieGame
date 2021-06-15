
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
public class XRSocketInteractorWithName : XRSocketInteractor
{
    [Header("Target's Name")]
    public string targetName;

    [Header("HoverMeshMaterial Color Setting")]
    [SerializeField] private Color canHoverColor = Color.blue;
    [SerializeField] private Color cantHoverColor = Color.red;
    public bool canSelect = true;

    public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractor(updatePhase);
        if (selectTarget)
        {
            selectTarget.transform.position = attachTransform.position;
            selectTarget.transform.rotation = attachTransform.rotation;
        }
            
    }
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (interactableHoverMeshMaterial)
        {
            var mag = args.interactable?.GetComponent<Magazine>();
            if (mag && mag.magName == targetName)
            {
                interactableHoverMeshMaterial.color = canHoverColor;
            }
            else if (!mag || mag.magName != targetName)
            {
                interactableHoverMeshMaterial.color = cantHoverColor;
            }
        }
        base.OnHoverEntered(args);
    }

    public override bool CanSelect(XRBaseInteractable interactable)
    {
        var mag = interactable?.GetComponent<Magazine>();
        if (mag && canSelect)
        {
            return base.CanSelect(interactable) && mag.magName == targetName;
        }
        else
            return false;
    }
}

