using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellCtrl : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip shell;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(shell);
    }
}
