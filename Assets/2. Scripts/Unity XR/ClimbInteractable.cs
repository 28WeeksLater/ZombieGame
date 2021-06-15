using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbInteractable : XRBaseInteractable
{
    private Vector3 pos = Vector3.zero;
    private Quaternion rot = Quaternion.Euler(0, 0, 0);
    private Transform mesh = null;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (args.interactor is XRDirectInteractor)
            Climber.climbingHand = args.interactor.GetComponent<XRController>();

        mesh = args.interactor.transform?.Find("Mesh");
        if (mesh)
        {
            pos = mesh.position;
            rot = mesh.rotation;
        }
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (selectingInteractor && mesh)
        {
                mesh.position = pos;
                mesh.rotation = rot;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (args.interactor is XRDirectInteractor)
            if (Climber.climbingHand && Climber.climbingHand.name == args.interactor.name)
                Climber.climbingHand = null;

        if (mesh)
        {
            mesh.localPosition = Vector3.zero;
            mesh.localRotation = Quaternion.Euler(0, 0, 0);
        }
        mesh = null;
    }
}
