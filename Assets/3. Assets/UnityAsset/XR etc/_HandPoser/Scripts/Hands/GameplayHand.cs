using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class GameplayHand : BaseHand
{
    // The interactor we react to
    [SerializeField] private XRBaseInteractor targetInteractor = null;
    
    [Header("Hand Animation")]
    private InputDevice targetDevice;
    public InputDeviceCharacteristics controllerCharacteristics;
    private Animator handAnimator;

    private void Start()
    {
        handAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateHandAnimation();
    }

    void UpdateHandAnimation() //손 모델의 애니메이션 조정
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        if (devices.Count > 0) //디바이스가 있는 경우
        {
            targetDevice = devices[0];

            if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            {
                handAnimator.SetFloat("Trigger", triggerValue);
            }
            else
            {
                handAnimator.SetFloat("Trigger", 0);
            }

            if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            {
                handAnimator.SetFloat("Grip", gripValue);
            }
            else
            {
                handAnimator.SetFloat("Grip", 0);
            }
        }
    }

    [System.Obsolete]
    private void OnEnable()
    {
        // Subscribe to selected events
        targetInteractor.onSelectEntered.AddListener(TryApplyObjectPose);
        targetInteractor.onSelectExited.AddListener(TryApplyDefaultPose);
    }

    [System.Obsolete]
    private void OnDisable()
    {
        // Unsubscribe to selected events
        targetInteractor.onSelectEntered.RemoveListener(TryApplyObjectPose);
        targetInteractor.onSelectExited.RemoveListener(TryApplyDefaultPose);
    }

    private void TryApplyObjectPose(XRBaseInteractable interactable)
    {
        // Try and get pose container, and apply
        if (interactable.TryGetComponent(out PoseContainer poseContainer))
            ApplyPose(poseContainer.pose);
    }

    private void TryApplyDefaultPose(XRBaseInteractable interactable)
    {
        // Try and get pose container, and apply
        if (interactable.TryGetComponent(out PoseContainer poseContainer))
            ApplyDefaultPose();
    }

    public override void ApplyOffset(Vector3 position, Quaternion rotation)
    {
        // Invert since the we're moving the attach point instead of the hand
        Vector3 finalPosition = position * -1.0f;
        Quaternion finalRotation = Quaternion.Inverse(rotation);

        // Since it's a local position, we can just rotate around zero
        finalPosition = finalPosition.RotatePointAroundPivot(Vector3.zero, finalRotation.eulerAngles);

        // Set the position and rotach of attach
        targetInteractor.attachTransform.localPosition = finalPosition;
        targetInteractor.attachTransform.localRotation = finalRotation;
    }

    private void OnValidate()
    {
        // Let's have this done automatically, but not hide the requirement
        if (!targetInteractor)
            targetInteractor = GetComponentInParent<XRBaseInteractor>();
    }

   
}