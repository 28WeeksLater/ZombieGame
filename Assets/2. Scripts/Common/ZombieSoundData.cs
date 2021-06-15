using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSoundData
{
    public enum State
    {
        Idle, Patrol, Trigger, Chasing, Attack, Die, Talk, Angry, Bite
    }

    public int Key => _state;
    private int _state;
    
    private Dictionary<int, AudioClip[]> dictionary;
    public void SetDictionary(Dictionary<int, AudioClip[]> dictionary)
    {
        this.dictionary = dictionary;
    }
    public ZombieSoundData()
    {
        
    }
        
    public ZombieSoundData SetState(State state)
    {
        _state = Convert.ToInt32(state);
        return this;
    }
    public void Reset()
    {
        _state = 0;
    }

    public void AddToMap(AudioClip[] data)
    {
        dictionary.Add(Key,data);
    }
    
}
