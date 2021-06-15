using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventHandler : MonoBehaviour
{
    public bool isClear;
    public bool isActivated;

    public UnityEvent _event;
    public UnityEvent _restart;

    public virtual void TriggerStart() { }
}
