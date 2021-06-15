using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandGrabInteractable : XRGrabInteractable
{
    [Header("Second Grab XRSimpleInteractable")]
    [SerializeField]private List<XRSimpleInteractable> secondGrabbablePoints = new List<XRSimpleInteractable>();
    [HideInInspector] public XRBaseInteractor secondInteractor;
    private Quaternion attachtInitialRotation;
    [Header("Positioning Hand")]
    [SerializeField] private Transform firstPos;
    [SerializeField] private Transform secondPos;
    private enum TwoHandRotationType { None, First, Second, Third }
    [SerializeField] private TwoHandRotationType twoHandRotationType = TwoHandRotationType.Third;

    [SerializeField] private bool snapToSecondHand = true;
    private Quaternion initalRotationOffset;

    public bool onSecond = false;

    private Transform first = null;
    private Transform second = null;
    public bool canTakeByOther;

    private void Start()
    {
        foreach(var item in secondGrabbablePoints)
        {
            item.selectEntered.AddListener(OnSecondGrabbableEnter);
            item.selectExited.AddListener(OnSecondGrabbableExit);
        }
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if(selectingInteractor)
        {
            transform.position = selectingInteractor.attachTransform.position;
            transform.rotation = selectingInteractor.attachTransform.rotation;
        }

        if (secondInteractor && selectingInteractor)
        {
            onSecond = true;

            if (snapToSecondHand)
                selectingInteractor.attachTransform.rotation = GetTwoHandRotation();
            else if (!snapToSecondHand)
                selectingInteractor.attachTransform.rotation = GetTwoHandRotation() * initalRotationOffset;

            if (secondInteractor && second)
            {
                    second.position = secondPos.position;
                    second.rotation = secondInteractor.GetComponent<XRController>().controllerNode == UnityEngine.XR.XRNode.LeftHand || secondPos.localRotation.z == 0 ? secondPos.rotation : secondPos.rotation * Quaternion.Euler(0, 0, 180);
            }
            if (selectingInteractor && first)
            {
                    first.position = firstPos.position;
                    first.rotation = selectingInteractor.GetComponent<XRController>().controllerNode == UnityEngine.XR.XRNode.LeftHand || firstPos.localRotation.z == 0 ? firstPos.rotation : firstPos.rotation * Quaternion.Euler(0, 0, 180);
            }
        }
        else if (!secondInteractor && selectingInteractor)
        {
            onSecond = false;
            if (selectingInteractor && first)
            {
                    first.position = firstPos.position;
                    first.rotation = selectingInteractor.GetComponent<XRController>().controllerNode == UnityEngine.XR.XRNode.LeftHand || firstPos.localRotation.z == 0 ? firstPos.rotation : firstPos.rotation * Quaternion.Euler(0, 0, 180);
            }
        }
        base.ProcessInteractable(updatePhase);
    }

    public Quaternion GetTwoHandRotation()
    {
       Quaternion targetRotation;

        if(twoHandRotationType == TwoHandRotationType.None)
        {
            targetRotation = Quaternion.LookRotation(secondInteractor.attachTransform.position - selectingInteractor.attachTransform.position);
        }
        else if(twoHandRotationType == TwoHandRotationType.First)
        {
            targetRotation = Quaternion.LookRotation(secondInteractor.attachTransform.position - selectingInteractor.attachTransform.position, selectingInteractor.attachTransform.up);
        }
        else if(twoHandRotationType == TwoHandRotationType.Second)
        {
            targetRotation = Quaternion.LookRotation(secondInteractor.attachTransform.position - selectingInteractor.attachTransform.position, secondInteractor.attachTransform.up);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(secondInteractor.transform.position - firstPos.position);
            Vector3 gripRotation = Vector3.zero;
            gripRotation.z = selectingInteractor.transform.eulerAngles.z;

            targetRotation *= Quaternion.Euler(gripRotation);
        }
        return targetRotation;
    }

    public void OnSecondGrabbableEnter(SelectEnterEventArgs args)
    {
        if(selectingInteractor && selectingInteractor is XRDirectInteractor)
        {
            secondInteractor = args.interactor;
            second = secondInteractor.transform?.Find("Mesh");
            initalRotationOffset = Quaternion.Inverse(GetTwoHandRotation()) * selectingInteractor.attachTransform.rotation;
        } 
    }

    public void OnSecondGrabbableExit(SelectExitEventArgs args)
    {
        if (secondInteractor && second)
        {
                second.transform.localPosition = Vector3.zero;
                second.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (selectingInteractor && first)
        {
                first.transform.localPosition = Vector3.zero;
                first.transform.localRotation = Quaternion.Euler(0, 0, 0);
                selectingInteractor.attachTransform.rotation = Quaternion.LookRotation(selectingInteractor.transform.forward);
        }
        second = null; 
        secondInteractor = null;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        attachtInitialRotation = args.interactor.attachTransform.localRotation;
        first = selectingInteractor.transform?.Find("Mesh");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (secondInteractor && second)
        {
                second.transform.localPosition = Vector3.zero;
                second.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (args.interactor && first)
        {
                first.transform.localPosition = Vector3.zero;
                first.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        first = second = null;
        secondInteractor = null;
        base.OnSelectExited(args);
    }

    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        if (!canTakeByOther)
        {
            if (isSelected && interactor != selectingInteractor)
                return false;
            else
                return true;
        }
        else
            return true;

    }
}

