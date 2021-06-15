using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LeverEvent : EventHandler
{
    public Transform leftDoor;
    public Transform rightDoor;
    public float targetX;
    public UnityEvent leverDown;
    public float originalPosition;
    public float movement;
    public float delay;
    private WaitForSeconds ws;

    private void Start()
    {
        ws = new WaitForSeconds(delay);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localRotation.x >= targetX && !isClear)
        {
            leverDown.Invoke();
            isClear = true;
        }
    }

    IEnumerator DoorOpen()
    {
        var nowPosition = 0.0f;
        while (nowPosition < originalPosition)
        {
            leftDoor.Translate(Vector3.right * movement);
            rightDoor.Translate(Vector3.left * movement);
            nowPosition += movement;

            yield return ws;
        }
    }
    public void DoorOpenFunc()
    {
        StartCoroutine(DoorOpen());
    }
}
