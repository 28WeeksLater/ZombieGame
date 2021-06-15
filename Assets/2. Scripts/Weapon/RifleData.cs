using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CreateAssetMenu(fileName = "Rifle Data",menuName = "Scriptable Object/Rifle Data",order = int.MaxValue)]
public class RifleData : ScriptableObject
{
    [Header("Constituent  Prefab")]
    [SerializeField] 
    private GameObject bulletPrefab;
    public GameObject BulletPrefab { get { return bulletPrefab; } }
    [SerializeField] 
    private GameObject castingPrefab;
    public GameObject CastingPrefab { get { return castingPrefab; } }
    [SerializeField] 
    private GameObject muzzleFlashPrefab;
    public GameObject MuzzleFlashPrefab { get { return muzzleFlashPrefab; } }
    [SerializeField] 
    private GameObject impactPrefab;
    public GameObject ImpactPrefab { get { return impactPrefab; } }

    [Header("Rifle Detail Info")]
    [SerializeField] 
    private float power = 10.0f;
    public float Power { get { return power; } }
    [SerializeField] 
    private float range = 40.0f;
    public float Range { get { return range; } }
    [SerializeField] 
    private float accuracy = 0.015f;
    public float Accuracy { get { return accuracy; } }
    [SerializeField] 
    private float muzzleVelocity = 0.07f;
    public float MuzzleVelocity { get { return muzzleVelocity; } }
    [SerializeField] 
    private float ejectPower = 300.0f;
    public float EjectPower { get { return ejectPower; } }
    [SerializeField] 
    private float impactDestroyTimer = 0.5f;
    public float ImpactDestroyTimer { get { return impactDestroyTimer; } }
    [SerializeField] 
    private float knockBackForce = 30.0f;
    public float KnockBackForce { get { return knockBackForce; } }
    [SerializeField] 
    private Vector3 recoil = Vector3.zero;
    public Vector3 Recoil { get { return recoil; } }
    [SerializeField] 
    private Vector3 increaseRecoil = Vector3.zero;
    public Vector3 IncreaseRecoil { get { return increaseRecoil; } }
    [SerializeField] 
    private float recoilAmount = 0.05f;
    public float RecoilAmount { get { return recoilAmount; } }
    [SerializeField] 
    private float recoilPosition = 0.01f;
    public float RecoilPosition { get { return recoilPosition; } }

    [Header("Sound SFX")]
    public AudioClip fireSfx;
    public AudioClip noAmmoSfx;
    public AudioClip reloadSfx;
}
