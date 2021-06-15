using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{

    private const String soundRoot = "Melee";
    private const String impact = "/Impact";
    private const String swing = "/Swing";
    private const String body = "/BodyFlesh";
    private const String metal = "/Metal";
    private const String wood = "/Wood";
    private const String small = "/Small";
    private const String big = "/Big";
    

    private static readonly Dictionary<int, AudioClip[]> AudioClipsMap = new Dictionary<int, AudioClip[]>();
    public static SoundManager Instance { get; private set; }

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
    //impact/metal,wood/Big,Small
    //impact/body/Wood,Metal/Big,Small
    //swing/metal,wood/Big,Small

    void LoadSounds()
    {
        var sd = new SoundData();
        sd.SetDictionary(AudioClipsMap);
        
        sd.SetMaterial(SoundData.Type.Blunt)
        .SetSoundType(SoundData.Impact.Heavy)
        .SetSize(SoundData.Size.None)
        .SetTarget(SoundData.Target.Object)
        .AddToMap(Resources.LoadAll<AudioClip>(soundRoot + impact + wood + big));
        
        sd.SetSoundType(SoundData.Impact.Light)
        .SetTarget(SoundData.Target.Object)
        .AddToMap(Resources.LoadAll<AudioClip>(soundRoot + impact + wood + small));

        sd.SetSoundType(SoundData.Swing.Heavy)
        .SetSize(SoundData.Size.Big)
        .SetTarget(SoundData.Target.None)
        .AddToMap(Resources.LoadAll<AudioClip>(soundRoot + swing + wood + big));

        sd.SetSoundType(SoundData.Swing.Light)
        .SetSize(SoundData.Size.Small)
        .AddToMap(Resources.LoadAll<AudioClip>(soundRoot + swing + wood + small)); 
        
        sd.SetMaterial(SoundData.Type.Sword)
        .SetSoundType(SoundData.Impact.Heavy)
        .SetSize(SoundData.Size.None)
        .SetTarget(SoundData.Target.Object)
        .AddToMap(Resources.LoadAll<AudioClip>(soundRoot + impact + metal + big));
        
        sd.SetSoundType(SoundData.Impact.Light)
        .SetTarget(SoundData.Target.Object)
        .AddToMap(Resources.LoadAll<AudioClip>(soundRoot + impact + metal + small));
        
        sd.SetSoundType(SoundData.Swing.Heavy)
        .SetSize(SoundData.Size.Big)
        .SetTarget(SoundData.Target.None)
        .AddToMap(Resources.LoadAll<AudioClip>(soundRoot + swing + metal + big));

        sd.SetSoundType(SoundData.Swing.Light)
        .SetSize(SoundData.Size.Small)
        .AddToMap(Resources.LoadAll<AudioClip>(soundRoot + swing + metal + small)); 
        
        sd.SetMaterial(SoundData.Type.Sword)
        .SetSoundType(SoundData.Impact.Heavy)
        .SetSize(SoundData.Size.None)
        .SetTarget(SoundData.Target.Body)
        .AddToMap(Resources.LoadAll<AudioClip>(soundRoot + impact + body + metal + big));
        
        sd.SetSoundType(SoundData.Impact.Light)
        .AddToMap(Resources.LoadAll<AudioClip>(soundRoot + impact + body + metal + small));
        
        sd.SetMaterial(SoundData.Type.Blunt)
        .SetSoundType(SoundData.Impact.Heavy)
        .SetSize(SoundData.Size.None)
        .SetTarget(SoundData.Target.Body)
        .AddToMap(Resources.LoadAll<AudioClip>(soundRoot + impact + body + wood + big));
        
        sd.SetSoundType(SoundData.Impact.Light)
        .AddToMap(Resources.LoadAll<AudioClip>(soundRoot + impact + body + wood + small));
        
    }
    
    public AudioClip[] GetSounds(SoundData soundData)
    {
        return AudioClipsMap[soundData.Key];
    }

    public AudioClip GetRandomSound(SoundData soundData)
    {
        var clips = GetSounds(soundData);
        return clips[Random.Range(0,clips.Length)];
    }

    

}
