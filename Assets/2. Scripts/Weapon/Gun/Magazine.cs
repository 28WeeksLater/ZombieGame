using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Magazine : MonoBehaviour
{
    [Header("Magazine Info")]
    public string magName;
    public GameObject[] bullet;
    public int fullAmmo = 0;
    public int ammoCount = 0;

    public bool isDeleteable = true;
    private float timer = 0;
    private XRBaseInteractable interactable;

    private void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();
        if(isDeleteable)
            StartCoroutine(DestroyTimer());
        interactable.activated.AddListener(DeleteMagazine);
    }

    private void Update()
    {
        if(ammoCount == 0 && bullet!=null)
        {
            foreach(var num in bullet)
            {
                num.SetActive(false);
            }
        }
    }

    public void DeleteMagazine(ActivateEventArgs args)
    {
        if(ammoCount >0)
        {
            var player = interactable.selectingInteractor.GetComponentInParent<PlayerCtrl>();
            player.ammoCount += ammoCount;
            player.SetText(ammoCount,1);
        }
        Destroy(gameObject);
    }


    IEnumerator DestroyTimer()
    {
        while(true)
        {
            yield return new WaitForSeconds(1.0f);
            if (!interactable.isSelected)
                timer++;
            else if (interactable.isSelected && timer > 0)
                timer--;

            if (timer >= 5.0f)
                Destroy(gameObject);
        }
    }
}
