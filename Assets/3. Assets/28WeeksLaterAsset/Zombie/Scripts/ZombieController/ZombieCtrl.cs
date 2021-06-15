using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using State = ZombieBehavior.ZombieState;

public class ZombieCtrl : MonoBehaviour
{
    // When Player detected -> detect
    // idle <-> patrol
    // Eco 

    public ZombieData zombieData;

    public Transform visionPoint;

    public ZombieBehavior behavior;
    
    private ZombieMovement _movement;

    public Transform Target { get; set; }
    public Vector3 TargetPos { get; private set; }
    
    public float life = 0f;

    public float HP;

    private LayerMask layerMask;
    public bool HasPath { get; private set; }
    
    public Transform hairPos;
    public Renderer renderer;

    private float time = 0f;

    void Start()
    {
        var player = GameManager.Instance.player.transform;
        if (player != null && behavior.CurrentState != State.TRIGGER) Target = player;
        Init();
        _movement = GetComponent<ZombieMovement>();
    }

    public void Init()
    {
        transform.tag = "Zombie";
        layerMask = 1 << 2;
        layerMask = ~layerMask;

        HP = zombieData.InitHp;
        behavior.SetRandomState();
    }

    void Update()
    {
        if (behavior.CurrentState == State.DIE) return;
        
        DetectPlayer();
        UpdateTargetPosition();
        UpdateState();
        UpdateRandomState();
        //KillUseless();
    }


    public void ChangeState(State state)
    {
        if (behavior.CurrentState == state) return;
        behavior.ChangeState(state);
    }
    
    private void DetectPlayer()
    {
        if (behavior.CurrentState == State.CHASING) return;
        
        var t_cols = Physics.OverlapSphere(transform.position, zombieData.ViewDistance, zombieData.PlayerLayer);
        
        if(t_cols.Length > 0) {
            var _targetTf = t_cols[0].transform;
            var _direction = (_targetTf.position - visionPoint.position).normalized;
            var _angle = Vector3.Angle(_direction, visionPoint.forward);
        
            if (_angle < zombieData.FieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(visionPoint.position, _direction, out hit, zombieData.ViewDistance, layerMask))
                {
                    if (hit.transform == Target)
                    {
                        Target = hit.transform;
                        StartChasing();
                    }
                }
            }
        }
    }

    private void StartChasing()
    {
        switch (behavior.CurrentState)
        {
            case State.ANGRY:
            case State.TALK:
                behavior.CommunicationNeed = (int) Random.Range(1f, 21f);
                behavior.animator.SetBool(zombieData.HashMatchComplete, false);
                break;
            case State.BITE:
                behavior.HungryNeeds = (int) Random.Range(0f, 21f);
                behavior.animator.SetBool(zombieData.HashBite, false);
                break;
        }

        behavior.agent.stoppingDistance = 0;
        behavior.animator.SetBool(zombieData.HashMove, true);
        ChangeState(State.CHASING);
    }
    
    
    private void UpdateTargetPosition()
    {
        var position = Target.position;
        TargetPos = new Vector3(position.x, transform.position.y, position.z);
    }

    private void UpdateState()
    {
        var distance = Vector3.Distance(TargetPos, transform.position);
        var triggerDist = Vector3.Distance(_movement.randomPos, transform.position);

        if (distance <= zombieData.AttackDist)
        {
            if(behavior.CurrentState == State.CHASING)
                ChangeState(State.ATTACK);
        }
        else{
            if (behavior.CurrentState == State.ATTACK)
                ChangeState(State.CHASING);
        }
        
        if(triggerDist <= zombieData.AttackDist * 2)
        {
            if (behavior.CurrentState == State.TRIGGER)
                ChangeState(State.IDLE);
        }
    }

    private void UpdateRandomState()
    {
        if ((time += Time.deltaTime) <= 1f) return;
        time = 0;
        if (behavior.CurrentState != State.IDLE) return;
        behavior.SetRandomState();
    }

    private void KillUseless()
    {
        life += Time.deltaTime;
        if (life >= 10 && Vector3.Distance(TargetPos, transform.position) >= 30.0f)
        {
            ReturnToPool();
        }
    }

    public void KillZombieNow()
    {
        ChangeState(State.DIE);
        ReturnToPool();
    }

    public void KillZombie()
    {
        StartCoroutine(ChangeToFood());
    }

    IEnumerator ChangeToFood()
    {
        transform.tag = "Food";
        ChangeState(State.DIE);
        yield return new WaitForSeconds(5f);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        ZombiePool.Instance.ReturnZombie(gameObject);
    }

    public bool CalculatePath()
    {
        return HasPath = NavMesh.CalculatePath(transform.position, TargetPos, NavMesh.AllAreas, new NavMeshPath());
    }

    public void DamageTaken()
    {
        if (behavior.CurrentState == State.CHASING &&
            behavior.CurrentState == State.ATTACK) return;
        Target = GameManager.Instance.player.transform;
        ChangeState(State.CHASING);
    }
    
    public void SetTrigger(Transform tr)
    {
        Target = tr;
        UpdateTargetPosition();
        ChangeState(State.TRIGGER);
    }
}
