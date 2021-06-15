using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    public DamageSystem DamageSystem;
    public PlayerCtrl PlayerCtrl;
    
    void Start()
    {
        DamageSystem.HP = 50;
        PlayerCtrl.HP = 50;
    }
}
