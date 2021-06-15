using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChainSawActivationEvent : EventHandler
{
    public float fuel = 0;
    public float fillTime = 1f;
    public int fillValue = 50;
    public float consumptionTime = 1f;
    public float consumptionValue = 1;
    public Transform chainSaw;
    public Transform gasCan;
    public Transform gasPumpPos;
    public Transform chainSawSavedPos;
    public AudioSource audio;
    public AudioClip startSound;
    public AudioClip loopSound;
    public AudioClip stopSound;
    public AudioClip fillSound;
    private bool usingChainSaw = false;
    private float time = 0;
    
    private Vector3 chainSawOriginPos;
    private Quaternion chainSawOriginRot;
    private Vector3 gasCanOriginPos;
    private Vector3 gasPumpOriginPos;
    
    public UnityEvent activeEvent;
    public UnityEvent inactiveEvent;

    public DamageSystemComponent damageSystemComponent;

    private void OnEnable()
    {
        chainSawOriginPos = chainSawSavedPos.position;
        chainSawOriginRot = chainSawSavedPos.rotation;
        gasCanOriginPos = gasCan.position;
        gasPumpOriginPos = gasPumpPos.position;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isClear && isActivated && !usingChainSaw && other.transform.CompareTag("GasCan"))
        {
            if (fuel < 500)
            {
                if (time > fillTime)
                {
                    fuel += fillValue;
                    time = 0;
                }
                else
                {
                    time += Time.deltaTime;
                }
                
                if (!audio.isPlaying)
                {
                    PlaySound(fillSound, true);
                }
            }
            else
            {
                usingChainSaw = true;
                _event.Invoke();
                activeEvent.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!usingChainSaw && other.transform.CompareTag("GasCan"))
        {
            audio.Stop();
        }
    }

    IEnumerator UseChainSaw()
    {
        yield return new WaitForSeconds(PlaySound(startSound, false));
        yield return new WaitForSeconds(PlaySound(loopSound, true));
        while(isActivated && fuel > 0)
        {
            yield return new WaitForSeconds(consumptionTime);
            fuel -= consumptionValue;
        }
        yield return new WaitForSeconds(PlaySound(stopSound, false));
        audio.Stop();
        inactiveEvent.Invoke();
    }

    public void TurnOnChainSaw()
    {
        StopAllCoroutines();
        damageSystemComponent.isTicker = true;
        IEnumerator routine = UseChainSaw();
        StartCoroutine(routine);
    }

    public void StartFill()
    {
        isActivated = true;
    }

    public void Restart()
    {
        StopAllCoroutines();
        audio.Stop();
        audio.loop = false;
        fuel = 0;
        gasCan.position = gasCanOriginPos;
        chainSaw.position = chainSawOriginPos;
        chainSaw.rotation = chainSawOriginRot;
        gasPumpPos.position = gasPumpOriginPos;
        damageSystemComponent.isTicker = false;
        usingChainSaw = false;
    }

    private float PlaySound(AudioClip clip, bool loop)
    {
        audio.clip = clip;
        audio.loop = loop;
        audio.Play();

        return clip.length;
    }
}
