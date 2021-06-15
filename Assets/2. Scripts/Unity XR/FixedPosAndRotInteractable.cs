using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FixedPosAndRotInteractable : XRGrabInteractable
{
    public Transform target;
    private Transform mesh = null;
    
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        mesh = args.interactor.transform?.Find("Mesh");
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (selectingInteractor && mesh)
        {
            mesh.position = target.position;
            mesh.rotation = target.rotation;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (mesh)
        {
            mesh.localPosition = Vector3.zero;
            mesh.localRotation = Quaternion.Euler(0, 0, 0);
        }
        mesh = null;
    }
}
