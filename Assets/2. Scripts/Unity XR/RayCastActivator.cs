using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class RayCastActivator : MonoBehaviour
{ 
    [Header("Raycast Interactor")]
    [SerializeField]
    private GameObject raycastInteractor;

    private XRBaseInteractor interactor;
    private XRController controller;

    private void Awake()
    {
        interactor = GetComponent<XRBaseInteractor>();
        controller = GetComponent<XRController>();
        raycastInteractor.SetActive(false);
    }

    void FixedUpdate()
    {
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue) && gripValue >= 0.7 && interactor.selectTarget == null)
        {
            raycastInteractor.SetActive(true);
        }
        else
        {
            raycastInteractor.SetActive(false);
        }

    }
}
