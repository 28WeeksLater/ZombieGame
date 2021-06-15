using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundData
{
    public enum Type
    {
        None, Blunt, Sword
    }
    
    public enum Impact
    {
        Light,Heavy
    }
        
    public enum Swing
    {
        Light, Heavy
    }

    public enum Size
    {
        None, Small, Big
    }

    public enum Target
    {
        None, Object, Body
    }

    public int Key => _material + _soundType + _size + _target;
    private const int ImpactType = 00;
    private const int SwingType = 10;
    private int _material; 
    private int _soundType; 
    private int _size;
    private int _target;

    private Dictionary<int, AudioClip[]> dictionary;

    public void SetDictionary(Dictionary<int, AudioClip[]> dictionary)
    {
        this.dictionary = dictionary;
    }

    public SoundData()
    {
        
    }
        
    public SoundData SetMaterial(Type type)
    {
        _material = Convert.ToInt32(type) * 10000;
        return this;
    }
        
    public SoundData SetSoundType(Impact type)
    {
        _soundType = (ImpactType + Convert.ToInt32(type)) * 100;
        return this;
    }
        
    public SoundData SetSoundType(Swing type)
    {
        _soundType = (SwingType + Convert.ToInt32(type)) * 100;
        return this;
    }
        
    public SoundData SetSize(Size type)
    {
        _size = Convert.ToInt32(type) * 10;
        return this;
    }
            
    public SoundData SetTarget(Target type)
    {
        _target = Convert.ToInt32(type);
        return this;
    }

    public void Reset()
    {
        _material = 0;
        _soundType = 0;
        _size = 0;
        _target = 0;
    }

    public void AddToMap(AudioClip[] data)
    {
        dictionary.Add(Key,data);
    }
        
}