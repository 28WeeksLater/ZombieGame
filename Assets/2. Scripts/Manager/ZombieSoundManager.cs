using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieSoundManager : MonoBehaviour
{
    private const String attack = "Attack";
    private const String chasing = "Chasing";
    private const String die = "Die";
    private const String eating = "Eating";
    private const String idlePatrol = "IdlePatrol";
    private const String talk = "Talk";

    private static readonly Dictionary<int, AudioClip[]> AudioClipsMap = new Dictionary<int, AudioClip[]>();
    public static ZombieSoundManager Instance { get; private set; }

    void Start()
    {
        if (Instance == null)
        {
            LoadSounds();
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void LoadSounds()
    {
        var sd = new ZombieSoundData();
        sd.SetDictionary(AudioClipsMap);

        sd.SetState(ZombieSoundData.State.Idle)
         .AddToMap(Resources.LoadAll<AudioClip>(idlePatrol));
        sd.SetState(ZombieSoundData.State.Patrol)
         .AddToMap(Resources.LoadAll<AudioClip>(idlePatrol));

        sd.SetState(ZombieSoundData.State.Trigger)
         .AddToMap(Resources.LoadAll<AudioClip>(chasing));
        sd.SetState(ZombieSoundData.State.Chasing)
         .AddToMap(Resources.LoadAll<AudioClip>(chasing));

        sd.SetState(ZombieSoundData.State.Attack)
         .AddToMap(Resources.LoadAll<AudioClip>(attack));

        sd.SetState(ZombieSoundData.State.Die)
         .AddToMap(Resources.LoadAll<AudioClip>(die));

        sd.SetState(ZombieSoundData.State.Talk)
         .AddToMap(Resources.LoadAll<AudioClip>(talk));
        sd.SetState(ZombieSoundData.State.Angry)
         .AddToMap(Resources.LoadAll<AudioClip>(talk));

        sd.SetState(ZombieSoundData.State.Bite)
         .AddToMap(Resources.LoadAll<AudioClip>(eating));

    }
    public AudioClip[] GetSounds(ZombieSoundData zombieSoundData)
    {
        return AudioClipsMap[zombieSoundData.Key];
    }

    public AudioClip GetRandomSound(ZombieSoundData zombieSoundData)
    {
        var clips = GetSounds(zombieSoundData);
        return clips[Random.Range(0,clips.Length)];
    }
}
