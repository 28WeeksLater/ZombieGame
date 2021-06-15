using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class DamageSystemComponent : MonoBehaviour
{
    public Rigidbody rigidbody;

    public float minDamageMultiplier = 1f;
    public float maxDamageMultiplier = 1f;
    
    [HideInInspector] public DamageSystem damageSystem;
    public BloodParticleSystem bloodParticleSystem;
    public bool UseAsWeapon = true;
    public bool isTicker = false;
    private float Damage => damageSystem.useAsWeapon ? damageSystem.baseDamage : 0;

    public void TakeGunDamage(float damage, float power, Vector3 pos, Quaternion dir)
    {
        if (rigidbody != null)
        {
            rigidbody.AddForceAtPosition(-dir.eulerAngles * power,pos, ForceMode.Force);
            if (gameObject.CompareTag("Body"))
            {
                bloodParticleSystem.Create(pos, dir, transform);                
            }
        }
        damageSystem.OnGunDamageTaken(damage, transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        var opposite = collision.gameObject.GetComponent<DamageSystemComponent>();
        if (opposite != null)
        {
            UseAsWeapon = damageSystem.useAsWeapon;
            if (opposite.UseAsWeapon)
            {
                OnComponentCollisionEnter(opposite, collision);       
            }
        }
    }

    private void OnComponentCollisionEnter(DamageSystemComponent opposite, Collision collision)
    {
        if (damageSystem.IsSelfCollision(collision.gameObject)) return;
        if (gameObject.CompareTag("Player")) return;
        if (gameObject.CompareTag("Weapon") && collision.gameObject.CompareTag("Player")) return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Hand")) return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Inventory")) return;
        
        if (opposite.isTicker)
        {
            damageSystem.OnDamageTaken(opposite.Damage, collision.gameObject, transform);
            if (gameObject.CompareTag("Body") && !collision.gameObject.CompareTag("Body"))
            {
                bloodParticleSystem.Create(collision, transform);
            }
            return;
        }
        
        var speed = collision.impulse.magnitude / rigidbody.mass;
        if (speed < 0.05f) return;
        
        if (opposite.Damage == 0f) return;
        if (rigidbody.velocity.magnitude < 2f) return;
        var damageMultiplier = opposite.minDamageMultiplier + 
                               opposite.maxDamageMultiplier * (rigidbody.velocity.magnitude - 2f) / 5f;
        
        damageSystem.OnDamageTaken(opposite.Damage * damageMultiplier, collision.gameObject, transform);
        if (gameObject.CompareTag("Body") && !collision.gameObject.CompareTag("Body"))
        {
            bloodParticleSystem.Create(collision, transform);
        }
    }


}
