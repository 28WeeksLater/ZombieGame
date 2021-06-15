using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsPaperReadEvent : EventHandler
{
    private bool[] isRead = new bool[2];
    private void OnTriggerEnter(Collider other)
    {
        if (!isClear && other.transform.CompareTag("Player") && isActivated)
        {
            isClear = true;
            _event.Invoke();
        }
    }

    public void Active(int num)
    {
        isRead[num] = true;
        for (int i = 0; i < isRead.Length; i++)
        {
            if (!isRead[i])
            {
                isActivated = false;
                return;
            }
        }
        isActivated = true;
    }
}
