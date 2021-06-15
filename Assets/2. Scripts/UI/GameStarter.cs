using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    private AudioSource _audioSource;
    public AudioClip openSfx;
    private bool isLoad;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void GameStart()
    {
        StartCoroutine(GameStarting());
    }

    IEnumerator GameStarting()
    {
        _audioSource.PlayOneShot(openSfx);
        yield return StartCoroutine(CameraManager.Instance.LiftIn());
        if(!isLoad)
        {
            SceneLoader.Instance.SceneChange();
            isLoad = true;
        }
    }
    
    public void GameExit()
    {
        _audioSource.PlayOneShot(openSfx);
        SceneLoader.Instance.ExitGame();
    }
}
