using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnLogs : MonoBehaviour
{
    public Transform[] logs;
    public GameObject logPrefab;
    private Vector3[] logPositions;
    private Quaternion[] logRotations;

    private void OnEnable()
    {
        logPositions = new Vector3[logs.Length];
        logRotations = new Quaternion[logs.Length];
        for (int i = 0; i < logs.Length; i++)
        {
            logPositions[i] = logs[i].position;
            logRotations[i] = logs[i].rotation;
        }
    }

    public void Respawn()
    {
        Debug.LogError("Logs");
        foreach (var log in logs)
        {
            if (log != null)
            {
                Destroy(log.gameObject);
            }
        }

        for(int i = 0; i < logs.Length; i++)
        {
            Instantiate(logPrefab, logPositions[i], logRotations[i], transform);
        }
    }
}
