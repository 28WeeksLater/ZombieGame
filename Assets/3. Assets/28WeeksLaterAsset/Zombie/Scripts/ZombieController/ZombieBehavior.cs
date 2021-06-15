using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ZombieBehavior : MonoBehaviour
{
    //Eco value Food Talk
    //Init state can be Idle or Patrol

    private ZombieCtrl controller;
    private ZombieData _zombieData;

    [SerializeField] private float hungryNeeds;
    public float HungryNeeds
    {
        get => hungryNeeds;
        set
        {
            hungryNeeds = value;
            animator.SetFloat(_zombieData.HashHungry, value);
        } 
    }

    [SerializeField] private float communicationNeeds;
    public float CommunicationNeed{
        get => communicationNeeds;
        set
        {
            communicationNeeds = value;
            animator.SetFloat(_zombieData.HashCommunication, value);
        } 
    }
    
    private bool communicationMatched = false;
    
    public enum ZombieState{IDLE, PATROL, TRIGGER, CHASING, ATTACK, DIE ,TALK, ANGRY, BITE, }

    public delegate void OnStateChanged(ZombieState state);

    private OnStateChanged onStateChanged;
    public ZombieState CurrentState
    {
        get => _currentState;
        private set => _currentState = value;
    }

    public ZombieState _currentState;

    
    public NavMeshAgent agent;
    public Animator animator;
    
    private bool nearCallReceived = false;

    private float time = 0f;

    private void Awake()
    {
        CurrentState = ZombieState.IDLE;
        controller = GetComponent<ZombieCtrl>();
        _zombieData = controller.zombieData;
    }

    private void OnEnable()
    {
        HungryNeeds = Random.Range(1f, 21f);
        CommunicationNeed = Random.Range(0f, 20f);
    }

    public void AddStateReceiver(OnStateChanged stateChanged)
    {
        onStateChanged += stateChanged;
    }

    public void ChangeState(ZombieState state)
    {
        CurrentState = state;
        switch (CurrentState)
        {
            case ZombieState.ATTACK:
            {
                if (controller.HasPath)
                {
                    CallNearZombie();
                }
                break;
            }
            case ZombieState.CHASING:
                CallNearZombie();
                break;
            case ZombieState.TALK:
                if (!communicationMatched)
                {
                    MatchZombie();
                }
                if (!communicationMatched)
                {
                    CommunicationNeed = 0f;
                    CurrentState = ZombieState.ANGRY;
                }
                break;
            case ZombieState.BITE:
                var detected = FindFood();
                if (!detected)
                {
                    HungryNeeds = (int) Random.Range(0f, 20f);
                    CurrentState = ZombieState.IDLE;
                    return;
                }
                break;
        }
        
        onStateChanged(CurrentState);
    }

    private void Update()
    {
        if (CurrentState == ZombieState.DIE) return;
        animator.SetFloat(_zombieData.HashSpeed, agent.velocity.magnitude);
        UpdateNeeds();
    }

    private void UpdateNeeds()
    {
        if ((time += Time.deltaTime) < 1f) return;
        var angry = (int)Random.Range(1f, 101f);
        if (CurrentState != ZombieState.IDLE && CurrentState != ZombieState.PATROL &&
            CurrentState != ZombieState.ANGRY) return;
        if (angry <= 1)
        {
            CommunicationNeed = 0f;
        }
        else if(CurrentState != ZombieState.TALK && CurrentState != ZombieState.ANGRY && CurrentState != ZombieState.BITE)
        {
            CommunicationNeed += (int) Random.Range(0f, 10f);
            HungryNeeds += (int) Random.Range(0f, 10f);
        }
        
        var maxState = 0 < CommunicationNeed && CommunicationNeed < HungryNeeds ? 1 : 0;
        switch (maxState)
        {
            case 0:
                if (CommunicationNeed > 75)
                {
                    controller.ChangeState(ZombieState.TALK);
                }
                else if (CommunicationNeed == 0)
                {
                    controller.ChangeState(ZombieState.ANGRY);
                }
                else
                {
                    SetRandomState();
                }
                break;
            case 1:
                controller.ChangeState(HungryNeeds > 75 ? ZombieState.BITE : ZombieState.IDLE);
                break;
        }

        time = 0;
    }

    public void SetRandomState()
    {
        if(CurrentState == ZombieState.TRIGGER) return;
        var state = Random.Range(0, 10) > 1 ? ZombieState.PATROL : ZombieState.IDLE;
        if (state != CurrentState) controller.ChangeState(state);
    }
    
    private void CallNearZombie()
    {
        if (nearCallReceived) return;
        nearCallReceived = true;
        foreach (var i in ZombieSpawner.Instance.zombies)
        {
            if (Vector3.Distance(i.transform.position, transform.position) > _zombieData.CallZombies) continue;
            var ctrl = i.GetComponent<ZombieCtrl>();
            ctrl.Target = controller.Target;
            ctrl.ChangeState(ZombieState.CHASING);
            ctrl.behavior.nearCallReceived = true;
        }
    }
    
    private void MatchZombie()
    {
        if (communicationMatched)
        {
            return;
        }
        foreach (var i in ZombieSpawner.Instance.zombies)
        {
            var opposite = i.GetComponent<ZombieCtrl>();
            if (Vector3.Distance(transform.position, opposite.transform.position) > _zombieData.ViewDistance)
            {
                continue;
            }

            if (!opposite.behavior.communicationMatched || i.transform.position.Equals(transform.position)) continue;
            
            opposite.Target = transform;
            opposite.ChangeState(ZombieState.TALK);
            opposite.behavior.CommunicationNeed = 75f;
            controller.Target = i.transform;
            communicationMatched = true;
            return;
        }
    }

    private bool FindFood()
    {
        var foods = GameObject.FindGameObjectsWithTag("Food");

        foreach (var i in foods)
        {
            if (!(Vector3.Distance(i.transform.position, transform.position) <= _zombieData.ViewDistance)) continue;
            controller.Target = i.transform;
            return true;
        }
        return false;
    }
    
}
