using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePool : MonoBehaviour
{
    public int poolSize;
    
    public GameObject zombiePrefab;
    public Material[] skins;
    public GameObject[] hairs;
    public ZombieData[] stats;

    private static ZombiePool instance;
    public static ZombiePool Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            else
                return instance;
        }
    }
    
    private Queue<GameObject> pool;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        pool = new Queue<GameObject>();
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < poolSize; i++)
        {
            pool.Enqueue(CreateZombie());    
        }
    }

    private GameObject CreateZombie()
    {
        var zombie = Instantiate(zombiePrefab, transform.position, transform.rotation);
        zombie.transform.SetParent(transform);
        var controller = zombie.GetComponent<ZombieCtrl>();
        controller.zombieData = stats[Random.Range(0, stats.Length)];
        Instantiate(hairs[Random.Range(0, hairs.Length)], controller.hairPos);
        controller.renderer.material = skins[Random.Range(0, skins.Length)];
        zombie.SetActive(false);
        return zombie;
    }

    public GameObject GetZombie()
    {
        var zombie = pool.Count > 0 ? pool.Dequeue() : CreateZombie();
        return zombie;
    }

    public void ReturnZombie(GameObject zombie)
    {
        zombie.SetActive(false);
        zombie.transform.position = transform.position;
        pool.Enqueue(zombie);
    }
}
