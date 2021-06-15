using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BloodData",menuName = "Scriptable Object/Blood Data",order = int.MaxValue)]
public class BloodData : ScriptableObject
{
    [SerializeField]private GameObject bloodAttach;

    public GameObject BloodAttach
    {
        get { return bloodAttach; }
    }
    
    [SerializeField] private GameObject[] bloodParticles;

    public GameObject[] BloodParticles
    {
        get { return bloodParticles; }
    }
}
