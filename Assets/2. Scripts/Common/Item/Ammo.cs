using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Ammo : MonoBehaviour
{
    private XRBaseInteractable interactable;
    public int ammo = 0;

    private void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.activated.AddListener(AddAmmo);
    }

    public void AddAmmo(ActivateEventArgs args)
    {
        if(interactable.isSelected)
        {
            var player = args.interactable.selectingInteractor.GetComponentInParent<PlayerCtrl>();
            player.ammoCount += ammo;
            player.SetText(ammo,1);
            player.audioSource.PlayOneShot(player.ammoSFX);
            Destroy(gameObject);
        }
    }
}
