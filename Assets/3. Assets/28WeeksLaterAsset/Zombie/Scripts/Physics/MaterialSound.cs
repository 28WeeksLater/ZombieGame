using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSound : MonoBehaviour
{
    public SoundData.Type type;
    public SoundData.Swing swing;
    public SoundData.Size size;

    private SoundData swingData;
    private SoundData impactObjectLight;
    private SoundData impactBodyLight;
    private SoundData impactObjectHeavy;
    private SoundData impactBodyHeavy;

    [HideInInspector]public GameObject audioTarget;
    [HideInInspector]public AudioSource audioSource;
    private Rigidbody _rigidbody;

    private bool swingPlayed = false;
    private void Awake()
    {
        if (audioTarget == null)
        {
            audioTarget = GameObject.Find("FX_SoundTarget");
            if (audioTarget == null)
            {
                Debug.LogError("FX_SoundTarget is missing in " + name);
            }
        }
        audioSource = audioTarget.GetComponent<AudioSource>();

        swingData = new SoundData()
            .SetMaterial(type)
            .SetSoundType(swing)
            .SetSize(size);
        
        impactObjectLight = new SoundData()
            .SetMaterial(type)
            .SetSoundType(SoundData.Impact.Light)
            .SetTarget(SoundData.Target.Object);
        
        impactBodyLight = new SoundData()
            .SetMaterial(type)
            .SetSoundType(SoundData.Impact.Light)
            .SetTarget(SoundData.Target.Body);
        
        impactObjectHeavy = new SoundData()
            .SetMaterial(type)
            .SetSoundType(SoundData.Impact.Heavy)
            .SetTarget(SoundData.Target.Object);
        
        impactBodyHeavy = new SoundData()
            .SetMaterial(type)
            .SetSoundType(SoundData.Impact.Heavy)
            .SetTarget(SoundData.Target.Body);

        _rigidbody = GetComponent<Rigidbody>();
    }

    private void PlaySound(SoundData data, Vector3 pos)
    {
        audioTarget.transform.position = pos;
        var audioClip = SoundManager.Instance
            .GetRandomSound(data);
        audioSource.PlayOneShot(audioClip);
    }

    private void Update()
    {
        if (_rigidbody.velocity.magnitude > 5f && !swingPlayed)
        {
            PlaySound(swingData, Vector3.zero);
            swingPlayed = true;
        }else if (_rigidbody.velocity.magnitude < 3f)
        {
            swingPlayed = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_rigidbody.velocity.magnitude < 2f) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Hand")) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("VRPlayer")) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Inventory")) return;
        
        var position = other.transform.position;
        var speed = other.impulse.magnitude / _rigidbody.mass;
        
        if (speed > 4f)
        {
            PlaySound(impactObjectHeavy, position);   
            if (other.gameObject.CompareTag($"Body"))
            {
                PlaySound(impactBodyHeavy, position);
            }
        }
        else if (speed > 0.01f)
        {
            PlaySound(impactObjectLight, position);   
            if (other.gameObject.CompareTag($"Body"))
            {
                PlaySound(impactBodyLight, position);
            }
        }
    }
}
