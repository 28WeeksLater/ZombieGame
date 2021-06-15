using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Heal : MonoBehaviour
{
    private XRBaseInteractable interactable;
    [SerializeField] private float heal = 0;

    void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();
    }

    public void HealPlayer()
    {
        var player = interactable.selectingInteractor.GetComponentInParent<PlayerCtrl>();
       
        if (player.HP + heal > player.fullHP && player.HP != player.fullHP)
        {
            player.SetText((int)(player.fullHP - player.HP), 2);
            player.HP = player.fullHP;
            player.audioSource.PlayOneShot(player.healSFX);
            Destroy(gameObject);
        }
        else if (player.HP != player.fullHP)
        {
            player.SetText((int)heal, 2);
            player.HP += heal;
            player.audioSource.PlayOneShot(player.healSFX);
            Destroy(gameObject);
        }
    }
}
