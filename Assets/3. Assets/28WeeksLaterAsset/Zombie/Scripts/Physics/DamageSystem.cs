using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class DamageSystem : MonoBehaviour
{
    public float HP;
    public bool isAlive = true;
    private CopyMotion CopyMotion;
    
    public BloodParticleSystem bloodParticleSystem;
    
    public bool isInvincible = false;
    public bool useAsWeapon = true;
    public float baseDamage;
    public float minDamageMultiplier = 1f;
    public float maxDamageMultiplier = 1f;
    
    private bool isCopyMotionTarget = false;

    private float lastDamageTime;

    public List<GameObject> targetComponents = new List<GameObject>();

    private PlayerCtrl _playerCtrl;

    private bool isZombie = false;

    private ZombieCtrl _zombieController;

    private XRGrabInteractable _interactable;
    private bool isInteractable = false;
    
    private void Awake()
    {
        var blood = GetComponent<BloodParticleSystem>();
        if (blood != null)
        {
            bloodParticleSystem = blood;
        }
        
        var copyMotion = GetComponent<CopyMotion>();
        if (copyMotion != null)
        {
            CopyMotion = copyMotion;
            isCopyMotionTarget = true;
            AddScriptToChild(transform);
        }
        else
        {
            AddScript(transform);
            AddScriptToChild(transform);
        }
        if (gameObject.CompareTag("Player"))
        {
            _playerCtrl = GetComponent<PlayerCtrl>();
        }

        var interactive = GetComponent<XRGrabInteractable>();
        if (interactive != null)
        {
            _interactable = interactive;
            isInteractable = true;
        }
    }

    private void Start()
    {
        if (GetComponent<ZombieCtrl>() == null) return;
        isZombie = true;
        _zombieController = GetComponent<ZombieCtrl>();
        HP = _zombieController.HP;
        useAsWeapon = true;
    }

    private void AddScriptToChild(Transform target)
    {
        foreach (Transform child in target)
        {
            AddScript(child);
            AddScriptToChild(child.transform);
        }
    }

    private void AddScript(Transform tf)
    {
        var childGameObject = tf.gameObject;
        if (childGameObject.GetComponent<DamageSystemComponent>() != null) return;
        
        var childRigidBody = childGameObject.GetComponent<Rigidbody>();
        if (childRigidBody == null) return;
        
        childGameObject.AddComponent<DamageSystemComponent>();
        targetComponents.Add(childGameObject);

        var dsc = childGameObject.GetComponent<DamageSystemComponent>();
        dsc.rigidbody = childRigidBody;
        dsc.damageSystem = this;
        dsc.maxDamageMultiplier = maxDamageMultiplier;
        dsc.minDamageMultiplier = minDamageMultiplier;
        dsc.bloodParticleSystem = bloodParticleSystem;
    }

    private void Update()
    {
        if (isZombie)
        {
            useAsWeapon = _zombieController.behavior.CurrentState == ZombieBehavior.ZombieState.ATTACK;
        }
        else if (isInteractable)
        {
            useAsWeapon = _interactable.isSelected;
        }
    }

    private void OnDestroy()
    {
        foreach (var targetComponent in targetComponents)
        {
            DestroyImmediate(targetComponent.GetComponent<DamageSystemComponent>());
        }
    }

    private void LateUpdate()
    {
        if (!isCopyMotionTarget) return;
        if (isAlive) return;
        
        CopyMotion.isActive = false;
        _zombieController.KillZombie();
    }

    public bool IsSelfCollision(GameObject collisionGameObject)
    {
        return targetComponents.Any(targetComponent => targetComponent == collisionGameObject);
    }

    public void OnDamageTaken(float damage, GameObject collisionGameObject, Transform childTransform)
    {
        if (isInvincible) return;
        if (Time.time - lastDamageTime < 0.3f) return;
        lastDamageTime = Time.time;
        
        if (CompareTag("Player"))
        {
            _playerCtrl.Hit(damage);
        }
        else
        {
            _zombieController.DamageTaken();
            HP -= damage;
            if (HP <= 0)
            {
                isAlive = false;
            }   
        }
    }
    
    public void OnGunDamageTaken(float damage, Transform childTransform)
    {
        if (isInvincible) return;
        HP -= damage;
        _zombieController.DamageTaken();
        if (HP <= 0)
        {
            isAlive = false;
        }
    }

}
