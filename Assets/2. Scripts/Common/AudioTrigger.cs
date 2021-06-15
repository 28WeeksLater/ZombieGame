using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] clip; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ClipOn(int i)
    {
        audioSource.clip = clip[i];
        audioSource.Play();
    }
    public void ClipOneShot(int i)
    {
        audioSource.PlayOneShot(clip[i]);
    }
}
