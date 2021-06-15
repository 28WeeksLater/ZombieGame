using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class RopeEvent : EventHandler
{
    public Transform startPoint;
    public AudioSource engineSound;

    public UnityEvent mainEvent;
    public UnityEvent activeGasCanEvent;

    public AudioClip ropePullClip;
    public AudioClip stopClip;
    public AudioClip engineClip;
    public AudioClip engineLoopClip;

    public float pullDistance = 0.5f;
    public float pushDistance = 0.25f;

    public int eventStep = 3;

    private float distance;
    private bool pushMode = false;
    public int step = 0;

    private IEnumerator soundCoroutine;

    private Vector3 originPos;

    private void OnEnable()
    {
        originPos = transform.position;
        soundCoroutine = Sound1();
    }

    void Update()
    {
        if (transform.position.z < startPoint.position.z)
        {
            transform.position = startPoint.position;
        }

        if (!isClear && !isActivated)
        {
            distance = Vector3.Distance(transform.position, startPoint.position);

            if (!pushMode && distance > pullDistance)
            {
                step++;
                pushMode = true;
                _event.Invoke();
            }
            if (pushMode && distance < pushDistance)
            {
                pushMode = false;
            }

            if (step >= eventStep)
            {
                activeGasCanEvent.Invoke();
                isActivated = true;
            }
        }
    }

    public override void TriggerStart()
    {
        base.TriggerStart();
        if (!isActivated)
        {
            StopSound();
            soundCoroutine = GetCoroutine(step - 1);
            StartCoroutine(soundCoroutine);
        }
    }

    IEnumerator Sound1()
    {
        yield return new WaitForSeconds(PlaySound(ropePullClip));
        yield return new WaitForSeconds(PlaySound(stopClip));
    }

    IEnumerator Sound2()
    {
        yield return new WaitForSeconds(PlaySound(ropePullClip));
        yield return new WaitForSeconds(PlaySound(engineClip));
        yield return new WaitForSeconds(PlaySound(stopClip));
    }

    IEnumerator Sound3()
    {
        yield return new WaitForSeconds(PlaySound(ropePullClip));
        yield return new WaitForSeconds(PlaySound(engineClip));
        engineSound.clip = engineLoopClip;
        engineSound.loop = true;
        engineSound.Play();
    }

    IEnumerator ClearSound()
    {
        yield return new WaitForSeconds(PlaySound(stopClip));
    }

    private float PlaySound(AudioClip clip)
    {
        engineSound.Stop();
        engineSound.clip = clip;
        engineSound.Play();

        return clip.length;
    }

    private IEnumerator GetCoroutine(int index)
    {
        switch (index)
        {
            case 0:
                return Sound1();
            case 1:
                return Sound2();
            case 2:
                return Sound3();
        }

        return null;
    }

    private void StopSound()
    {
        StopAllCoroutines();
        if (engineSound.isPlaying)
        {
            engineSound.loop = false;
            engineSound.Stop();
        }
    }

    public void Clear()
    {
        StopSound();
        StartCoroutine(ClearSound());
        isActivated = true;
        isClear = true;
    }

    public void Restart()
    {
        step = 0;
        StopSound();
        transform.position = originPos;
        pushMode = false;
        isActivated = false;
    }
}
