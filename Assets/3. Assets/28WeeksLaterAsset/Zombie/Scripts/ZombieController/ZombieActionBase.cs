using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public abstract class ZombieActionBase : MonoBehaviour
{
    
    protected ZombieCtrl controller;
    protected ZombieBehavior behavior;
    protected ZombieData zombieData;

    protected virtual void OnEnable()
    {
        controller = GetComponent<ZombieCtrl>();
        zombieData = controller.zombieData;
        behavior = GetComponent<ZombieBehavior>(); 
        behavior.AddStateReceiver(StateChanged);
    }

    private void StateChanged(ZombieBehavior.ZombieState state)
    {
        switch (state)
        {
            case ZombieBehavior.ZombieState.DIE:
                WhenDie();
                break;
            
            case ZombieBehavior.ZombieState.IDLE:
                WhenIdle();
                break;
            
            case ZombieBehavior.ZombieState.PATROL:
                WhenPatrol();
                break;
            
            case ZombieBehavior.ZombieState.CHASING:
                WhenChasing();
                break;
            
            case ZombieBehavior.ZombieState.ATTACK:
                WhenAttack();
                break;
            
            case ZombieBehavior.ZombieState.TRIGGER:
                WhenTrigger();
                break;

            case ZombieBehavior.ZombieState.TALK:
                WhenTalk();
                break;
            
            case ZombieBehavior.ZombieState.ANGRY:
                WhenAngry();
                break;
            
            case ZombieBehavior.ZombieState.BITE:
                WhenBite();
                break;
        }
    }

    protected virtual void WhenDie() {}
    protected virtual void WhenIdle() {}
    protected virtual void WhenPatrol() {}
    protected virtual void WhenChasing() {}
    protected virtual void WhenAttack() {}
    protected virtual void WhenTrigger() {}
    protected virtual void WhenTalk() {}
    protected virtual void WhenAngry() {}
    protected virtual void WhenBite() {}
    
    
    
}
