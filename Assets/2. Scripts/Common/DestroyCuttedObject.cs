using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCuttedObject : MonoBehaviour
{
    public float destroyTime = 3f;

    private float time = 0;
    
    void Update()
    {
        if (time > destroyTime)
        {
            Destroy(gameObject);
        }
        else
        {
            time += Time.deltaTime;
        }
    }
}
