using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

using State = ZombieBehavior.ZombieState;

public class ZombieMovement : ZombieActionBase
{
    private NavMeshAgent _agent;
    public Vector3 randomPos;
    private float damping;
    private Quaternion rot = Quaternion.identity;

    protected override void OnEnable()
    {
        base.OnEnable();
        _agent = GetComponent<ZombieBehavior>().agent;
        damping = zombieData.Damping;
    }

    private void Start()
    {
        _agent.updateRotation = false;
        _agent.autoBraking = false;
        _agent.avoidancePriority = Random.Range(30, 50);
        _agent.stoppingDistance = zombieData.AttackDist - 0.15f;
    }
    
    private void Update()
    {
        if (behavior.CurrentState == State.DIE) return;
        Rotate();
    }

    protected override void WhenDie()
    {
        damping = 3.0f;
        _agent.ResetPath();
        _agent.isStopped = true;
    }

    protected override void WhenIdle()
    {
        damping = 3.0f;
        _agent.isStopped = true;
        //_agent.ResetPath();
    }

    protected override void WhenPatrol()
    {
        damping = 3.0f;
        StartCoroutine(WhenPatrolAfter());
    }

    IEnumerator WhenPatrolAfter()
    {
        var ws = new WaitForSeconds(0.3f);
        while (behavior.CurrentState == State.PATROL)
        {
            _agent.isStopped = false;
            _agent.speed = zombieData.PatrolSpeed;
            if (!_agent.hasPath || Vector3.Distance(_agent.pathEndPosition, transform.position) <= _agent.stoppingDistance)
            {
                if (behavior.CurrentState == State.PATROL)
                {
                    behavior.SetRandomState();
                }
                if (behavior.CurrentState == State.PATROL)
                {
                    _agent.SetDestination(GetRandomPoint(transform, zombieData.PatrolRadius));
                }
            }
            yield return ws;
        }
    }

    protected override void WhenChasing()
    {
        damping = 7.0f;
        _agent.isStopped = false;
        _agent.speed = zombieData.ChaseSpeed;
        StartCoroutine(WhenChasingAfter());
    }

    IEnumerator WhenChasingAfter()
    {
        var ws = new WaitForSeconds(0.3f);
        while (behavior.CurrentState == State.CHASING)
        {
            yield return ws;
            if (controller.CalculatePath())
            {
                if (Vector3.Distance(controller.TargetPos, randomPos) > zombieData.SpreadDist)
                {
                    GenerateRandomPosition(zombieData.SpreadDist);
                }
                var chaseTargetMode = Vector3.Distance(controller.TargetPos, transform.position) <= zombieData.SpreadDist;
                _agent.SetDestination(chaseTargetMode ? controller.TargetPos : randomPos);
            }
            else
            {
                if (_agent.FindClosestEdge(out var hit))
                {
                    _agent.SetDestination(hit.position);
                }
            }
            _agent.stoppingDistance = zombieData.AttackDist;
            _agent.autoBraking = true;
        }
    }

    protected override void WhenAttack()
    {
        damping = 7.0f;
        _agent.isStopped = true;
    }

    protected override void WhenTrigger()
    {
        damping = 3.0f;
        _agent.isStopped = false;
        _agent.speed = zombieData.ChaseSpeed;
        var chaseTargetMode = Vector3.Distance(controller.TargetPos, transform.position) <= zombieData.SpreadDist;
        _agent.SetDestination(chaseTargetMode ? controller.TargetPos : randomPos);
        
        if (_agent.destination.x == 0 && _agent.destination.z == 0)
        {
            _agent.SetDestination(new Vector3(controller.TargetPos.x, transform.position.y, controller.TargetPos.z));
        }
        
        if (Vector3.Distance(controller.TargetPos, randomPos) > zombieData.SpreadDist)
        {
            GenerateRandomPosition(zombieData.SpreadDist);
        }
    }

    protected override void WhenTalk()
    {
        damping = 3.0f;
        
        _agent.isStopped = false;
        _agent.updateRotation = true;

        _agent.speed = zombieData.PatrolSpeed;
        _agent.stoppingDistance = zombieData.TalkDist;
        _agent.SetDestination(controller.Target.position);

        StartCoroutine(WhenTalkAfter());
    }
    
    IEnumerator WhenTalkAfter()
    {
        while (!(Vector3.Distance(transform.position, controller.Target.position) <= zombieData.TalkDist))
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        _agent.isStopped = true;
        _agent.updateRotation = false;
        yield return new WaitForSeconds(ZombieAnimator.clipTimes["Talk"]);
        behavior.CommunicationNeed = (int) Random.Range(1f, 21f);
        controller.Target = null;
        _agent.stoppingDistance = 0;
        controller.ChangeState(State.IDLE);
    }

    protected override void WhenBite()
    {
        damping = 3.0f;
        _agent.isStopped = false;
        _agent.updateRotation = true;
        _agent.speed = zombieData.PatrolSpeed;
        _agent.stoppingDistance = zombieData.AttackDist;
        _agent.SetDestination(controller.Target.position);
        StartCoroutine(WhenBiteAfter());
    }

    IEnumerator WhenBiteAfter()
    {
        while (true) 
        {
            yield return new WaitForSeconds(0.5f);

            if ((Vector3.Distance(controller.Target.position, transform.position) <= zombieData.AttackDist))
            {
                _agent.isStopped = true;
                _agent.updateRotation = false;
                yield return new WaitForSeconds(ZombieAnimator.clipTimes["Bite"]);
                if (controller.Target.gameObject.activeInHierarchy)
                {
                    controller.Target.GetComponent<ZombieCtrl>().KillZombieNow();
                    _agent.stoppingDistance = 0;
                    _agent.speed = 0;
                    controller.ChangeState(State.IDLE);
                    break;
                }
            }
            else if (!controller.Target.gameObject.activeInHierarchy)
            {
                _agent.stoppingDistance = 0;
                _agent.speed = 0;
                controller.ChangeState(State.IDLE);
                break;
            }
        }
    }

    private void Rotate()
    {
        var state = behavior.CurrentState;
        var position = controller.Target.position;
        var position1 = transform.position;

        Vector3 desiredVelocity;
        rot = state switch
        {
            State.TALK => Quaternion.LookRotation((position - position1).normalized),
            State.BITE => Quaternion.LookRotation((position - position1).normalized),
            State.ATTACK => (controller.TargetPos - position1).normalized == Vector3.zero 
                ? rot :Quaternion.LookRotation((controller.TargetPos - position1).normalized),
            _ => (desiredVelocity = _agent.desiredVelocity) == Vector3.zero ? rot : Quaternion.LookRotation(desiredVelocity)
        };
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * damping);
    }
    
    private void GenerateRandomPosition(float spreadDist)
    {
        while (true)
        {
            randomPos = GetRandomPoint(controller.Target, spreadDist, true);
            var direction = (controller.TargetPos - randomPos).normalized;
            if (!Physics.Raycast(randomPos, direction, out var hit, Vector3.Distance(randomPos, controller.TargetPos))) return;
            if (hit.transform.gameObject == controller.Target.gameObject) return;
            
            if (spreadDist > 1.0f)
            {
                spreadDist -= 1;
                continue;
            }
            _agent.SetDestination(controller.Target.position);

            break;
        }
    }

    private static Vector3 GetRandomPoint(Transform point, float radius = 0, bool normalized = false)
    {
        return RandomPoint(point.position, radius, out var _point, normalized) ? _point : Vector3.zero;
    }
    
    private static bool RandomPoint(Vector3 center, float range, out Vector3 result, bool normalized = false)
    {
        for (var i = 0; i < 30; i++)
        {
            var randomPoint = (normalized ? center + Random.insideUnitSphere.normalized * range : center + Random.insideUnitSphere * range);
            if (!NavMesh.SamplePosition(randomPoint, out var hit, 1.0f, NavMesh.AllAreas)) continue;
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }

    protected override void WhenAngry()
    {
        _agent.isStopped = true;
    }
}
