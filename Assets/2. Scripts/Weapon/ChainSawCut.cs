using System;
using System.Collections;
using System.Collections.Generic;
using MeshCutter;
using UnityEngine;

public class ChainSawCut : MonoBehaviour
{
    public Cutter cutter;
    public AudioSource sound;
    public AudioClip cutClip;
    public AudioClip loopClip;
    public bool active = false;

    private void OnCollisionEnter(Collision other)
    {
        if (active)
        {
            if (other.transform.tag.Equals("CutTarget"))
            {
                cutter.Cut(other.gameObject.GetComponent<CutterTarget>(), transform.position,
                    -transform.right);
            }

            if (other.transform.tag.Equals("CutTarget") || other.transform.tag.Equals("Zombie"))
            {
                PlaySound(cutClip);
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (active && other.transform.tag.Equals("CutTarget"))
        {
            PlaySound(loopClip);
        }
    }

    public void Active()
    {
        active = true;
    }

    public void Inactive()
    {
        active = false;
    }

    private void PlaySound(AudioClip clip)
    {
        sound.Stop();
        sound.loop = true;
        sound.clip = clip;
        sound.Play();
    }
}