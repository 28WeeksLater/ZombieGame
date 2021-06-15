using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class AmmoCtrl : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private XRBaseInteractor[] hand;
    [SerializeField] private GameObject[] magazine;
    [SerializeField] private Vector3 offset;
    private XRBaseInteractor interactor;
    private PlayerCtrl player;
    private Canvas canvas;
    private float initTime = 0;
    private float delay = 1.5f;

    void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        player = GetComponentInParent<PlayerCtrl>();
        canvas.enabled = false;
    }

    private void Update()
    {
        transform.rotation = new Quaternion(0, 0, 0, target.rotation.w-transform.rotation.w);
        transform.position = target.position + Vector3.up * offset.y + Vector3.ProjectOnPlane(target.right, Vector3.up).normalized * offset.x
            + Vector3.ProjectOnPlane(target.forward, Vector3.up).normalized * offset.z; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Array.Exists(hand, i => i.Equals(other.GetComponent<XRBaseInteractor>())))
        {
            player.GetAmmoCount();
            canvas.enabled = true;
            interactor = other.GetComponent<XRBaseInteractor>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Array.Exists(hand, i => i.Equals(other.GetComponent<XRBaseInteractor>())))
        {
            player.GetAmmoCount();
            GetMag();
            canvas.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Array.Exists(hand, i => i.Equals(other.GetComponent<XRBaseInteractor>())))
        {
            canvas.enabled = false;
            interactor = null;
        }
    }


    private GameObject CheckMag(string str)
    {
        GameObject temp = null;
        foreach(var etc in magazine)
        {
            if (etc.GetComponent<Magazine>()?.magName == str)
                temp = etc;
        }
        
        return temp;
    }

    private void GetMag()
    {
        if(player.ammoCount > 0)
        {
            if (interactor && !interactor.selectTarget && interactor.GetComponent<XRController>().inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue) && gripValue >= 0.7 && initTime <= Time.time)
            {
                if (interactor == player.right && player.left .selectTarget)
                {
                    var temp = Instantiate(CheckMag(player.left.selectTarget.GetComponentInChildren<XRSocketInteractorWithName>().targetName), interactor.transform.position, interactor.transform.rotation);
                    var tempMag = temp?.GetComponent<Magazine>();
                    if (tempMag.fullAmmo > player.ammoCount)
                    {
                        tempMag.ammoCount = player.ammoCount;
                        player.ammoCount = 0;
                    }    
                    else
                    {
                        tempMag.ammoCount = tempMag.fullAmmo;
                        player.ammoCount -= tempMag.fullAmmo;
                    }
                    initTime = Time.time + delay;
                }
                else if (interactor == player.left && player.right.selectTarget)
                {
                    var temp = Instantiate(CheckMag(player.right.selectTarget.GetComponentInChildren<XRSocketInteractorWithName>().targetName), interactor.transform.position, interactor.transform.rotation);
                    var tempMag = temp?.GetComponent<Magazine>();
                    if (tempMag.fullAmmo > player.ammoCount)
                    {
                        tempMag.ammoCount = player.ammoCount;
                        player.ammoCount = 0;
                    }
                    else
                    {
                        tempMag.ammoCount = tempMag.fullAmmo;
                        player.ammoCount -= tempMag.fullAmmo;
                    }
                    initTime = Time.time + delay;
                }
            }
        }
    }
}
