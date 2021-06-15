using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GasCanEvent : EventHandler
{
    public int amount = 0;
    public int fillValue = 10;
    public float fillTime = 1.0f;
    private float time = 0;

    public AudioSource audio;
    public AudioClip clip;
    
    public UnityEvent ropeEvent;

    private Vector3 originPos;
    public Transform gasPumpPos;
    private Vector3 pumpOriginPos;
    private void OnEnable()
    {
        originPos = transform.parent.position;
        pumpOriginPos = gasPumpPos.position;
        audio.loop = true;
        audio.clip = clip;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isClear && isActivated && other.transform.CompareTag("GasPump"))
        {
            if (amount < 100)
            {
                if (time > fillTime)
                {
                    amount += fillValue;
                    time = 0;
                }
                else
                {
                    time += Time.deltaTime;
                }
                
                if (!audio.isPlaying)
                {
                    audio.Play();
                }
            }
            else
            {
                _event.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("GasPump"))
        {
            audio.Stop();
        }
    }

    public void GasFilled()
    { 
        audio.Stop();
        ropeEvent.Invoke();
        isClear = true;
        GameManager.Instance.UpdateSavePoint();
    }

    public void StartFill()
    {
        isActivated = true;
    }

    public void Restart()
    {
        StopAllCoroutines();
        amount = 0;
        transform.parent.position = originPos;
        gasPumpPos.position = pumpOriginPos;
    }
}
