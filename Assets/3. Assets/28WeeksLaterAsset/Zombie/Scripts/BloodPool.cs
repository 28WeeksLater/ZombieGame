using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodPool : MonoBehaviour
{
    public int poolSize;

    public BloodData bloodData;
    
    private static BloodPool instance;
    public static BloodPool Instance
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
    
    //index 0:BloodParticle, index 1: BloodAttach
    private Queue<GameObject[]> pool;
    
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

        pool = new Queue<GameObject[]>();
        Init();
    }
    private void Init()
    {
        for (int i = 0; i < poolSize; i++)
        {
            pool.Enqueue(CreateBlood());    
        }
    }

    private GameObject[] CreateBlood()
    {
        var particle = Instantiate(bloodData.BloodParticles[Random.Range(0,bloodData.BloodParticles.Length)]);
        var attach = Instantiate(bloodData.BloodAttach);
        particle.transform.SetParent(transform);
        attach.transform.SetParent(transform);
        particle.SetActive(false);
        attach.SetActive(false);
        GameObject[] blood = {particle, attach};
        return blood;
    }

    public GameObject[] GetBlood()
    {
        return pool.Count > 0 ? pool.Dequeue() : CreateBlood();
    }

    public void ReturnBlood(GameObject[] blood)
    {
        if (blood.Length != 2)
        {
            return;
        }
        blood[0].SetActive(false);
        blood[1].SetActive(false);
        blood[1].transform.SetParent(transform);
        pool.Enqueue(blood);
    }
}
