using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ZombieSpawner : MonoBehaviour
{
    public int MAX_ZOMBIE;
    public float zombieSpawnTime = 1f;
    public float spawnDistance = 15.0f;
    public GameObject player;
    public bool active = false;

    [HideInInspector] public GameObject[] zombies;
    public int count = 0;
    private static ZombieSpawner instance;
    private float time = 0;
    
    public static ZombieSpawner Instance
    {
        get
        {
            if (!instance)
            {
                return null;
            }
            else
                return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    
    private void Update()
    {
        if (!active) return;
        
        zombies = GameObject.FindGameObjectsWithTag("Zombie");
        count = zombies.Length;

        if (count < MAX_ZOMBIE && time > zombieSpawnTime)
        {
            Vector3 point = SetRandomPoint(player.transform, spawnDistance, true);
            if (Vector3.zero != point && (point.y-player.transform.position.y) <= 3)
            {
                var zombie = ZombiePool.Instance.GetZombie();
                zombie.transform.position = point;
                var damageSystem = zombie.GetComponent<DamageSystem>();
                damageSystem.isAlive = true;
                damageSystem.HP = 100;
                zombie.GetComponent<CopyMotion>().isActive = true;
                zombie.GetComponent<CopyMotion>().ReActivate();
                zombie.gameObject.SetActive(true);
                zombie.GetComponent<ZombieCtrl>().Init();
                time = 0;
            }
        }
        else
        {
            time += Time.deltaTime;
        }
    }
    
    private Vector3 SetRandomPoint(Transform point = null, float radius = 0, bool normalized = false)
    {
        Vector3 _point;

        if (RandomPoint(point.position, radius, out _point, normalized))
        {
            return _point;
        }
        return Vector3.zero;
    }
    private bool RandomPoint(Vector3 center, float range, out Vector3 result, bool normalized = false)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = (normalized ? center + Random.insideUnitSphere.normalized * range : center + Random.insideUnitSphere * range);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    public void CleanZombies()
    {
        var zom = GameObject.FindGameObjectsWithTag("Zombie");
        var dead = GameObject.FindGameObjectsWithTag("Food");
        foreach (var i in zom)
        {
            ZombiePool.Instance.ReturnZombie(i);
        }
        foreach (var i in dead)
        {
            ZombiePool.Instance.ReturnZombie(i);
        }
    }
    
    public void ActiveChange()
    {
        if (active)
            active = false;
        else
            active = true;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
