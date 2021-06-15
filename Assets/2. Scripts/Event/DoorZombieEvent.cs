using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorZombieEvent : DoorEvent
{
    private TriggerSpawner trigger;

    protected override void Start()
    {
        base.Start();
        trigger = GetComponent<TriggerSpawner>();
    }
    public override void TriggerStart()
    {
        base.TriggerStart();
        trigger.Spawn();
    }
}
