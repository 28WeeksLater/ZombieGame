using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class XROffsetGrabInteractable : XRGrabInteractable
{
    private Vector3 initialAttachLocalPos;
    private Quaternion initialAttachLocalRot;

    [SerializeField] private XRBaseInteractable baseInteractable;
    [Header("Hand Position")]
    public Transform handPos;
    public bool onOffset = false;

    void Start()
    {
        if(!attachTransform)
        {
            GameObject grab = new GameObject("Grab Piovt");
            grab.transform.SetParent(transform, false);
            attachTransform = grab.transform;
        }

        initialAttachLocalPos = attachTransform.localPosition;
        initialAttachLocalRot = attachTransform.localRotation;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        onOffset = true;
        transform.SetParent(baseInteractable.transform);
        if (args.interactor is XRDirectInteractor)
        {
            attachTransform.position = args.interactor.transform.position;
            attachTransform.rotation = args.interactor.transform.rotation;
          
        }
        else
        {
            attachTransform.position = initialAttachLocalPos;
            attachTransform.rotation = initialAttachLocalRot;
        }

        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        onOffset = false;

        var mesh = args.interactor.transform?.Find("Mesh");
        if (mesh)
        {
            mesh.transform.localPosition = Vector3.zero;
            mesh.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        mesh = null;
        base.OnSelectExited(args);
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (selectingInteractor)
        {
            if(handPos)
            {
                var mesh = selectingInteractor.transform?.Find("Mesh");
                if(mesh != null)
                {
                    mesh.transform.position = handPos.position;
                    mesh.transform.rotation = selectingInteractor.GetComponent<XRController>().controllerNode == UnityEngine.XR.XRNode.LeftHand || handPos.localRotation.z == 0 ? handPos.rotation : handPos.rotation * Quaternion.Euler(0, 0, 180);
                }
            }

            if (!baseInteractable.isSelected)
                Drop(); 
        }
        

        base.ProcessInteractable(updatePhase);
    }

    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        return base.IsSelectableBy(interactor) && baseInteractable.isSelected;
    }

}


