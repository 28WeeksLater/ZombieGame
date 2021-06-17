using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    private PlayerCtrl playerCtrl;
    public Transform[] savePoints;
    private int maxIndex = 0;
    private int currIndex = 0;
    public bool isDie;

    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                return null;

            else return instance;
        }
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
        playerCtrl = player.GetComponent<PlayerCtrl>();
        maxIndex = savePoints.Length;

        MoveToSavePoint();
    }

    private void Update()
    {
        isDie = playerCtrl.isDie;
        if (playerCtrl.HP <= 0)
        {
            PlayerDie();
        }
    }
    
    public void MoveToSavePoint()
    {
        player.transform.position = savePoints[currIndex].position;
        player.transform.rotation = savePoints[currIndex].rotation;
    }

    public void UpdateSavePoint()
    {
        if(currIndex < maxIndex - 1) 
            currIndex++;
    }

    public void SceneChange()
    {
        SceneLoader.Instance.SceneChange();
        Destroy(EventManager.Instance.gameObject);
        Destroy(ZombieSpawner.Instance.gameObject);
        Destroy(TextManager.Instance.gameObject);
        Destroy(gameObject);
    }
    
    public void PlayerDie()
    {
        StartCoroutine(SavingPlayer());
    }

    private IEnumerator SavingPlayer()
    {
        playerCtrl.Die();
        ZombieSpawner.Instance.active = false;
        CameraManager.Instance.Die();
        MoveToSavePoint();
        playerCtrl.ResetState();
        yield return new WaitForSeconds(3.0f);
        ZombieSpawner.Instance.CleanZombies();
        CameraManager.Instance.Restart();
        CameraManager.Instance.ResetVolume();
        yield return new WaitForSeconds(4.0f);
        ZombieSpawner.Instance.active = true;
        EventManager.Instance.EventRestart();
    }
}
