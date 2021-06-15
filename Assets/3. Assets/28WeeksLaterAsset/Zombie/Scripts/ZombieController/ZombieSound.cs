using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSound : ZombieActionBase
{
    public enum Mode { Once, Loop }

    private bool nowIdlePatrol = false;

    [HideInInspector] public GameObject audioTarget;
    [HideInInspector] public AudioSource audioSource;

    private void Awake()
    {
        if (audioTarget == null)
        {
            audioTarget = GameObject.Find("Zombie_SoundTarget");
            if (audioTarget == null)
            {
                Debug.LogError("Zombie_SoundTarget is missing in " + name);
            }
        }
        audioSource = audioTarget.GetComponent<AudioSource>();
    }

    protected override void WhenDie() 
    {
        var sound = new ZombieSoundData()
            .SetState(ZombieSoundData.State.Die);
        PlaySound(sound, Mode.Once);
    }
    protected override void WhenIdle()
    {
        if(nowIdlePatrol) return;
        var sound = new ZombieSoundData()
            .SetState(ZombieSoundData.State.Idle);
        PlaySound(sound, Mode.Loop);
        nowIdlePatrol = true;
    }
    
    //WhenPatrol()
    protected override void WhenChasing() 
    {
        var sound = new ZombieSoundData()
            .SetState(ZombieSoundData.State.Chasing);
        PlaySound(sound, Mode.Loop);

    }
    protected override void WhenAttack() 
    {
        var sound = new ZombieSoundData()
            .SetState(ZombieSoundData.State.Attack);
        PlaySound(sound, Mode.Loop);
    }
    protected override void WhenTrigger() 
    {
        var sound = new ZombieSoundData()
            .SetState(ZombieSoundData.State.Trigger);
        PlaySound(sound, Mode.Loop);
    }
    protected override void WhenTalk()
    {
        var sound = new ZombieSoundData()
            .SetState(ZombieSoundData.State.Talk);
        PlaySound(sound, Mode.Once);
        nowIdlePatrol = false;
    }
    protected override void WhenAngry()
    {
        var sound = new ZombieSoundData()
            .SetState(ZombieSoundData.State.Angry);
        PlaySound(sound, Mode.Once);
        nowIdlePatrol = false;
    }
    protected override void WhenBite()
    {
        StartCoroutine(BiteSound());
        nowIdlePatrol = false;
    }

    private void PlaySound(ZombieSoundData data, Mode mode)
    {
        var audioClip = ZombieSoundManager.Instance
            .GetRandomSound(data);

        if (audioSource.isPlaying)
            audioSource.Stop();
        
        if (mode == Mode.Once)
        {
            audioSource.loop = false;
            audioSource.PlayOneShot(audioClip);
        }
        else
        {
            audioSource.loop = true;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    IEnumerator BiteSound()
    {
        while (behavior.CurrentState == ZombieBehavior.ZombieState.BITE)
        {
            yield return new WaitForSeconds(0.5f);
            
            if ((Vector3.Distance(controller.Target.position, transform.position) <= zombieData.AttackDist))
            {
                var sound = new ZombieSoundData()
                    .SetState(ZombieSoundData.State.Bite);
                PlaySound(sound, Mode.Loop);
                break;
            }
        }
    }
}
