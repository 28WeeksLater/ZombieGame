using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunMag : MonoBehaviour
{
    public ShotgunCtrl shotgunCtrl;
    private XRSocketInteractorWithName socketInteractor;

    private void Start()
    {
        socketInteractor = GetComponent<XRSocketInteractorWithName>();
    }

    public void EraseMag()
    {
        if (socketInteractor.selectTarget)
        {
            shotgunCtrl.ammoCount = socketInteractor.selectTarget.GetComponent<Magazine>().fullAmmo;
            Destroy(socketInteractor.selectTarget.gameObject);
        }
    }
}
