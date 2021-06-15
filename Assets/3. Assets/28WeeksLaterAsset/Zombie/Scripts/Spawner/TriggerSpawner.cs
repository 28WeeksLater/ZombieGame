using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TriggerSpawner : MonoBehaviour
{
    public int MAX_ZOMBIE;
    public float zombieSpawnTime = 1f;
    public Transform[] triggerTarget;
    public List<GameObject> zombies;
    public Transform[] spawnPoints;
    private EventHandler eventHandler;
    private bool active = false;

    private int count = 0;
    private float time = 0;

    public void Awake()
    {
        eventHandler = GetComponent<EventHandler>();
        zombies = new List<GameObject>(MAX_ZOMBIE);
    }

    private void Update()
    {
        if (GameManager.Instance)
        {
            if(GameManager.Instance.isDie)
                StopAllCoroutines();
        }

        if (!active) return;
        
        if (count < MAX_ZOMBIE && time > zombieSpawnTime)
        {
            var zombie = ZombiePool.Instance.GetZombie();
            zombie.transform.position = spawnPoints[count % spawnPoints.Length].position;
            zombie.transform.rotation = spawnPoints[count % spawnPoints.Length].rotation;
            var damageSystem = zombie.GetComponent<DamageSystem>();
            damageSystem.isAlive = true;
            damageSystem.HP = 100;
            zombie.SetActive(true);
            var controller = zombie.GetComponent<ZombieCtrl>();
            controller.Init();
            controller.SetTrigger(triggerTarget[Random.Range(0, triggerTarget.Length)]);
            zombies.Add(zombie);

            count++;
            time = 0;
        }
        else
        {
            time += Time.deltaTime;
            if(count == MAX_ZOMBIE)
                StartCoroutine(ZombieCheck());
        }
    }
    
    public void Spawn()
    {
        ZombieSpawner.Instance.active = false;
        active = true;
        count = 0;
    }

    IEnumerator ZombieCheck()
    {
        while(zombies.Count > 0)
        {
            foreach(var i in zombies)
            {
                try
                {
                    if (i.GetComponent<ZombieBehavior>().CurrentState == ZombieBehavior.ZombieState.DIE)
                    {
                        zombies.Remove(i);
                    }
                }
                catch (Exception e)
                {
                    zombies.Clear();
                    throw;
                }
            }
            yield return new WaitForSeconds(0.3f);
        }
        if (!GameManager.Instance.isDie && zombies.Count <= 0) {
            ZombieSpawner.Instance.active = true;
            eventHandler.isClear = true;
            active = false;
        }
    }
}
