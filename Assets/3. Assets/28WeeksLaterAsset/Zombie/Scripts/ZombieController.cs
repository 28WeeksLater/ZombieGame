using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieController : MonoBehaviour
{
    public ZombieData zombieData;
    [SerializeField]
    private float life = 0f;

    private Transform target;
    private Vector3 targetPos;
    private Vector3 randomPos;

    private NavMeshAgent agent;
    [Header("Ray Position")]
    public Transform rayPos;
    
    private WaitForSeconds ws;

    public enum State { IDLE, PATROL, TRIGGER, CHASING, ATTACK, DIE };
    public enum Mode { Basic, Spread }
    public enum EcoState { NONE, TALK, ANGRY, BITE }

    [Header("Zombie State & Move Logic")]
    public State state = State.IDLE;
    public Mode mode = Mode.Basic;

    [Header("Zombie Eco State")]
    public EcoState ecoState = EcoState.NONE;

    public float HP = 0.0f;
    private bool isDie = false;
    public float speed
    {
        get { return agent.velocity.magnitude; }
    }

    private Dictionary<string, float> clipTimes = new Dictionary<string, float>();

    private Transform matchedZombie = null;
    private Transform findFood = null;

    private Vector3 forward = Vector3.zero;

    private Animator animator;
    private float damping;

    private bool notHavePath;
    private bool isCalling;
    private float Hp;

    public Transform hairPos;
    public Renderer renderer;

    private void Start()
    {
        // var player = GameObject.FindGameObjectWithTag("Player");
        // if (player != null && state != State.TRIGGER)
        //     target = player.transform;
        // agent = GetComponent<NavMeshAgent>();
        // animator = GetComponentInChildren<Animator>();
        // ws = new WaitForSeconds(0.3f);
        //
        // UpdateAnimClipTimes();
        //
        // agent.updateRotation = false;
        // agent.autoBraking = false;
        // agent.avoidancePriority = Random.Range(30, 50);
        //
        // HP = zombieData.InitHp;
        // damping = zombieData.Damping;
        // agent.stoppingDistance = zombieData.AttackDist - 0.1f;
        // animator.SetFloat(zombieData.HashCommunication, Random.Range(1f, 21f));
        // animator.SetFloat(zombieData.HashHungry, Random.Range(0f, 20f));
    }

    // void OnEnable()
    // {
    //     StartCoroutine(Action());
    //     StartCoroutine(RandomState());
    //     StartCoroutine(Eco());
    //     StartCoroutine(StateRoutine());
    //     StartCoroutine(RotateRoutine());
    // }

    void Update()
    {
        if (!isDie)
        {
            animator.SetFloat(zombieData.HashSpeed, speed);
            View();
            SetLocation();
        }
    }

    IEnumerator RotateRoutine()
    {
        while (!isDie)
        {
            yield return ws;
            damping = ((state != State.ATTACK && state != State.CHASING) ? 3.0f : 7.0f);

            var rotValue = (state != State.ATTACK ? agent.desiredVelocity : (targetPos - transform.position).normalized);
            var rot = Quaternion.LookRotation(rotValue);

            try
            {
                if (ecoState == EcoState.TALK && matchedZombie)
                {
                    rot = Quaternion.LookRotation((matchedZombie.position - transform.position).normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * damping);
                }
                else if (findFood && ecoState == EcoState.BITE &&
                    findFood.gameObject.activeInHierarchy)
                {
                    rot = Quaternion.LookRotation((findFood.position - transform.position).normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * damping);
                }
                else
                {
                    transform.rotation = ((state != State.IDLE) ?
                        Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * damping) :
                    Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * damping));
                }
            }
            catch (MissingReferenceException e)
            {
                animator.SetFloat(zombieData.HashHungry, (int)Random.Range(0f, 20f));
                findFood = null;
                agent.stoppingDistance = 0;
                animator.SetBool(zombieData.HashMove, false);
                agent.speed = 0;
                ecoState = EcoState.NONE;
            }
        }
    }

    IEnumerator StateRoutine()
    {
        while (!isDie)
        {
            yield return ws;
            if (Vector3.Distance(targetPos, transform.position) <= zombieData.AttackDist && state == State.CHASING)
                state = State.ATTACK;
            else if (Vector3.Distance(targetPos, transform.position) > zombieData.AttackDist && state == State.ATTACK)
                state = State.CHASING;
            else if (Vector3.Distance(targetPos, transform.position) <= zombieData.AttackDist && state == State.TRIGGER)
            {
                state = State.IDLE;
                StartCoroutine(RandomState());
            }
        }
    }

    private void NearZombieAttack()
    {
        foreach (var i in ZombieSpawner.Instance.zombies)
        {
            if (Vector3.Distance(i.transform.position, transform.position) <= zombieData.CallZombies)
            {
                i.GetComponent<ZombieController>().SetTarget(target);
                i.SendMessage("Detect", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void Detect()
    {
        if (state != State.ATTACK && state != State.CHASING)
        {
            state = State.CHASING;
        }

        if (ecoState > EcoState.NONE)
        {
            StopCoroutine(Eco());
            StartCoroutine(StartChasing());
        }
    }
    private void View()
    {
        Collider[] t_cols = Physics.OverlapSphere(transform.position, zombieData.ViewDistance, zombieData.PlayerLayer);

        if (t_cols.Length > 0)
        {
            Transform player = t_cols[0].transform;

            Vector3 direction = (player.position - transform.position).normalized;
            float angle = Vector3.Angle(direction, transform.forward);
            if (angle < zombieData.FieldOfViewAngle * 0.5f)
            { 
                RaycastHit hit;
                if (Physics.Raycast(rayPos.position, direction, out hit, zombieData.ViewDistance, zombieData.PlayerLayer))
                {
                    target = hit.transform;
                    Detect();
                }
            }
        }
    }

    private bool RandomPoint(Vector3 center, float range, out Vector3 result, bool normalized = false)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = (normalized ? center + Random.insideUnitSphere.normalized * range : center + Random.insideUnitSphere * range);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    private Vector3 SetRandomPoint(Transform point = null, float radius = 0, bool normalized = false)
    {
        if (RandomPoint(point.position, radius, out var _point, normalized))
        {
            return _point;
        }
        return Vector3.zero;
    }

    IEnumerator Action()
    {
        while (!isDie)
        {
            yield return ws;

            switch (state)
            {
                case State.DIE:
                    agent.isStopped = true;
                    agent.ResetPath();
                    ecoState = EcoState.NONE;
                    animator.SetBool(zombieData.HashMove, false);
                    isDie = true;
                    transform.tag = "Food";
                    //StopAllCoroutines();
                    yield return new WaitForSeconds(15.0f);
                    ResetZombieStat();
                    break;
                case State.IDLE:
                    if (ecoState != EcoState.NONE)
                    {
                        break;
                    }
                    agent.isStopped = true;
                    animator.SetBool(zombieData.HashMove, false);
                    break;
                case State.PATROL:
                    if (ecoState != EcoState.NONE)
                    {
                        break;
                    }
                    agent.isStopped = false;
                    animator.SetBool(zombieData.HashMove, true);
                    agent.speed = zombieData.PatrolSpeed;
                    if (!agent.hasPath || Vector3.Distance(agent.pathEndPosition, transform.position) <= agent.stoppingDistance)
                    {
                        agent.SetDestination(SetRandomPoint(transform, zombieData.PatrolRadius));
                    }
                    break;
                case State.CHASING:
                    if (animator.GetBool(zombieData.HashNotFoundTarget))
                    {
                        animator.SetTrigger(zombieData.HashFoundTarget);
                    }
                    agent.isStopped = false;
                    if (!isCalling)
                    {
                        NearZombieAttack();
                        isCalling = true;
                    }
                    animator.SetBool(zombieData.HashMove, true);
                    agent.speed = zombieData.ChaseSpeed;
                    bool chaseTargetMode;
                    if (NavMesh.CalculatePath(transform.position, targetPos, NavMesh.AllAreas, new NavMeshPath()))
                    {
                        notHavePath = false;
                        if (mode == Mode.Spread)
                        {
                            if (randomPos == null || Vector3.Distance(targetPos, randomPos) > zombieData.SpreadDist)
                            {
                                CheckTargetPosition(zombieData.SpreadDist);
                            }
                        }
                        chaseTargetMode = (mode == Mode.Basic ||
                            Vector3.Distance(targetPos, transform.position) <= zombieData.SpreadDist);
                        agent.SetDestination(chaseTargetMode ? targetPos : randomPos);
                    }
                    else
                    {
                        notHavePath = true;
                        NavMeshHit hit;
                        if (agent.FindClosestEdge(out hit))
                        {
                            agent.SetDestination(hit.position);
                        }
                    }
                    agent.stoppingDistance = zombieData.AttackDist;
                    agent.autoBraking = true;
                    break;
                case State.ATTACK:
                    agent.isStopped = true;
                    animator.SetBool(zombieData.HashMove, false);
                    if (notHavePath)
                    {
                        animator.SetTrigger(zombieData.HashNotFoundTarget);
                        yield return new WaitForSeconds(clipTimes["Scream"]);
                    }
                    else
                    {
                        animator.SetTrigger(zombieData.HashAttack);
                        yield return new WaitForSeconds(clipTimes["Attack"]);
                        NearZombieAttack();
                    }
                    break;
                case State.TRIGGER:
                    if (ecoState != EcoState.NONE)
                    {
                        break;
                    }
                    if (animator.GetBool(zombieData.HashNotFoundTarget))
                    {
                        animator.SetTrigger(zombieData.HashFoundTarget);
                    }
                    agent.isStopped = false;
                    agent.speed = zombieData.ChaseSpeed;
                    animator.SetBool(zombieData.HashMove, true);
                    if (mode == Mode.Spread)
                    {
                        if (randomPos == null || Vector3.Distance(targetPos, randomPos) > zombieData.SpreadDist)
                        {
                            CheckTargetPosition(zombieData.SpreadDist);
                        }
                    }
                    chaseTargetMode = (mode == Mode.Basic || Vector3.Distance(targetPos, transform.position) <= zombieData.SpreadDist);
                    agent.SetDestination(chaseTargetMode ? targetPos : randomPos);
                    if (agent.destination.x == 0 && agent.destination.z == 0)
                    {
                        agent.SetDestination(new Vector3(targetPos.x, transform.position.y, targetPos.z));
                    }
                    break;
            }
        }
    }

    IEnumerator RandomState()
    {
        while (state == State.IDLE || state == State.PATROL)
        {
            yield return ws;
            if (ecoState == EcoState.NONE && state != State.TRIGGER)
            {
                var dice = Random.Range(0, 10);

                switch (dice)
                {
                    case 1:
                    case 2:
                    case 3:
                        state = State.IDLE;
                        forward = transform.forward;
                        agent.ResetPath();
                        yield return new WaitForSeconds(clipTimes["Idle"]);
                        break;
                    default:
                        state = State.PATROL;
                        yield return new WaitUntil(() => Vector3.Distance(transform.position, agent.destination) <= (agent.stoppingDistance + 0.1f));
                        break;
                }
            }
        }
    }

    IEnumerator Eco()
    {
        while (state < State.TRIGGER)
        {
            yield return new WaitForSeconds(0.5f);

            life += Time.deltaTime;
            if (life >= 10 && Vector3.Distance(targetPos, transform.position) >= 10.0f)
            {
                ResetZombieStat();
            }

            int angry = (int)Random.Range(1f, 101f);
            if (angry <= 1 && ecoState == EcoState.NONE)
            {
                animator.SetFloat(zombieData.HashCommunication, 0f);
            }
            else if (ecoState == EcoState.NONE)
            {
                animator.SetFloat(zombieData.HashCommunication, animator.GetFloat(zombieData.HashCommunication) + (int)Random.Range(0f, 10f));
                animator.SetFloat(zombieData.HashHungry, animator.GetFloat(zombieData.HashHungry) + (int)Random.Range(0f, 10f));
            }

            int maxState = (animator.GetFloat(zombieData.HashCommunication) < animator.GetFloat(zombieData.HashHungry) &&
                animator.GetFloat(zombieData.HashCommunication) > 0 ? 1 : 0);
            switch (maxState)
            {
                case 0:
                    if (animator.GetFloat(zombieData.HashCommunication) > 75)
                    {
                        ecoState = EcoState.TALK;
                    }
                    else if (animator.GetFloat(zombieData.HashCommunication) == 0)
                    {
                        ecoState = EcoState.ANGRY;
                    }
                    else
                    {
                        ecoState = EcoState.NONE;
                    }
                    break;
                case 1:
                    if (animator.GetFloat(zombieData.HashHungry) > 75)
                    {
                        ecoState = EcoState.BITE;
                    }
                    else
                    {
                        ecoState = EcoState.NONE;
                    }
                    break;
            }

            switch (ecoState)
            {
                case EcoState.ANGRY:
                    agent.isStopped = true;
                    yield return new WaitForSeconds(clipTimes["Scream"]);
                    animator.SetFloat(zombieData.HashCommunication, (int)Random.Range(1f, 21f));
                    ecoState = EcoState.NONE;
                    break;
                case EcoState.TALK:
                    if (!matchedZombie)
                    {
                        MatchZombie();
                    }
                    if (!matchedZombie)
                    {
                        animator.SetFloat(zombieData.HashCommunication, 0f);
                        break;
                    }

                    agent.isStopped = false;
                    agent.updateRotation = true;
                    animator.SetBool(zombieData.HashMove, true);
                    agent.speed = zombieData.PatrolSpeed;
                    agent.stoppingDistance = zombieData.TalkDist;
                    agent.SetDestination(matchedZombie.position);
                    if (Vector3.Distance(transform.position, matchedZombie.position) <= zombieData.TalkDist)
                    {
                        agent.isStopped = true;
                        agent.updateRotation = false;
                        animator.SetBool(zombieData.HashMatchComplete, true);
                        yield return new WaitForSeconds(clipTimes["Talk"]);

                        animator.SetFloat(zombieData.HashCommunication, (int)Random.Range(1f, 21f));
                        animator.SetBool(zombieData.HashMatchComplete, false);
                        matchedZombie = null;
                        agent.stoppingDistance = 0;
                        if (state == State.IDLE)
                        {
                            animator.SetBool(zombieData.HashMove, false);
                        }
                        else
                        {
                            animator.SetBool(zombieData.HashMove, true);
                        }
                        ecoState = EcoState.NONE;
                    }
                    break;
                case EcoState.BITE:
                    if (findFood == null)
                    {
                        FindFood();
                    }
                    if (findFood == null)
                    {
                        animator.SetFloat(zombieData.HashHungry, (int)Random.Range(0f, 20f));
                        break;
                    }

                    agent.isStopped = false;
                    agent.updateRotation = true;
                    animator.SetBool(zombieData.HashMove, true);
                    agent.speed = zombieData.PatrolSpeed;
                    agent.stoppingDistance = zombieData.AttackDist;
                    agent.SetDestination(findFood.position);
                    if (Vector3.Distance(findFood.position, transform.position) <= zombieData.AttackDist)
                    {
                        agent.isStopped = true;
                        agent.updateRotation = false;
                        animator.SetBool(zombieData.HashBite, true);
                        yield return new WaitForSeconds(clipTimes["Bite"]);

                        animator.SetFloat(zombieData.HashHungry, (int)Random.Range(0f, 20f));
                        animator.SetBool(zombieData.HashBite, false);
                        if (findFood != null && findFood.gameObject.activeInHierarchy)
                        {
                            findFood.GetComponent<ZombieController>().ResetZombieStat();
                        }
                        findFood = null;
                        agent.stoppingDistance = 0;
                        animator.SetBool(zombieData.HashMove, false);
                        agent.speed = 0;
                        ecoState = EcoState.NONE;
                    }
                    else if (!findFood.gameObject.activeInHierarchy)
                    {
                        animator.SetFloat(zombieData.HashHungry, (int)Random.Range(0f, 20f));
                        findFood = null;
                        agent.stoppingDistance = 0;
                        animator.SetBool(zombieData.HashMove, false);
                        agent.speed = 0;
                        ecoState = EcoState.NONE;
                    }
                    break;
            }
        }
    }

    IEnumerator StartChasing()
    {
        switch (ecoState)
        {
            case EcoState.ANGRY:
            case EcoState.TALK:
                animator.SetFloat(zombieData.HashCommunication, (int)Random.Range(1f, 21f));
                animator.SetBool(zombieData.HashMatchComplete, false);
                matchedZombie = null;
                break;
            case EcoState.BITE:
                animator.SetTrigger(zombieData.HashStandUp);
                yield return new WaitForSeconds(clipTimes["StandUp"]);
                animator.SetFloat(zombieData.HashHungry, (int)Random.Range(0f, 20f));
                animator.SetBool(zombieData.HashBite, false);
                findFood = null;
                break;
        }

        agent.stoppingDistance = 0;
        if (state == State.IDLE)
        {
            animator.SetBool(zombieData.HashMove, false);
        }
        else
        {
            animator.SetBool(zombieData.HashMove, true);
        }
        ecoState = EcoState.NONE;
    }

    private void MatchZombie()
    {
        if (matchedZombie)
        {
            return;
        }
        foreach (var i in ZombieSpawner.Instance.zombies)
        {
            var matchZombie = i.GetComponent<ZombieController>();
            if (Vector3.Distance(transform.position, matchZombie.transform.position) > zombieData.ViewDistance)
            {
                continue;
            }
            if (matchZombie.matchedZombie &&
                matchZombie.ecoState == EcoState.NONE &&
                !i.transform.position.Equals(transform.position) &&
                matchZombie.findFood)
            {
                matchZombie.matchedZombie = transform;
                matchZombie.animator.SetFloat(zombieData.HashCommunication, 75);
                matchedZombie = i.transform;
                return;
            }
        }
    }

    private void FindFood()
    {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");

        foreach (var i in foods)
        {
            if (Vector3.Distance(i.transform.position, transform.position) <= zombieData.ViewDistance)
            {
                findFood = i.transform;
                return;
            }
        }
    }

    private void CheckTargetPosition(float spreadDist)
    {
        randomPos = SetRandomPoint(target, spreadDist, true);
        RaycastHit hit;
        Vector3 direction = (targetPos - randomPos).normalized;
        if (Physics.Raycast(randomPos, direction, out hit, Vector3.Distance(randomPos, targetPos)))
        {
            if (hit.transform.gameObject != target.gameObject)
            {
                if (spreadDist > 1.0f)
                {
                    CheckTargetPosition(spreadDist - 1);
                }
                else
                {
                    agent.SetDestination(target.position);
                }
            }
        }
    }
    
    public void ZombieDie()
    {
        StopCoroutine(RandomState());
        state = State.DIE;
    }

    private void SetLocation()
    {
        targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
    }

    public void SetTarget(Transform tr)
    {
        target = tr;
    }
    public void SetTrigger(Transform tr)
    {
        state = State.TRIGGER;
        target = tr;
    }

    private void UpdateAnimClipTimes()
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (!clipTimes.ContainsKey(clip.name))
            {
                if (clip.name.Equals("Attack"))
                {
                    clipTimes.Add(clip.name, clip.length / 2);
                    continue;
                }
                clipTimes.Add(clip.name, clip.length);
            }
        }
    }
    
    public void ChangeState()
    {
        if(state != State.CHASING ||
           state != State.ATTACK)
        {
            SetTarget(GameManager.Instance.player.transform);
            state = State.CHASING;
        }
    }

    public void ResetZombieStat()
    {
        StopAllCoroutines();
        ZombiePool.Instance.ReturnZombie(gameObject);
    }

    public void ReactiveZombie()
    {
        if (!target)
        {
            var player =  GameObject.FindGameObjectWithTag("Player");
            if (player != null && state != State.TRIGGER)
                target = player.transform;
        }
        if (!agent)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (!animator)
        {
            animator = GetComponentInChildren<Animator>();
        }
        
        ws = new WaitForSeconds(0.3f);

        if (clipTimes.Count == 0)
        {
            UpdateAnimClipTimes();
        }
        agent.updateRotation = false;
        agent.autoBraking = false;
        agent.avoidancePriority = Random.Range(30, 50);
        HP = zombieData.InitHp;
        damping = zombieData.Damping;
        agent.stoppingDistance = zombieData.AttackDist - 0.1f;
        animator.SetFloat(zombieData.HashCommunication, Random.Range(1f, 21f));
        animator.SetFloat(zombieData.HashHungry, Random.Range(0f, 20f));
        life = 0;
        state = State.IDLE;
        ecoState = EcoState.NONE;
        isDie = false;
        transform.tag = "Zombie";
        StartCoroutine(Action());
        StartCoroutine(RandomState());
        StartCoroutine(Eco());
        StartCoroutine(StateRoutine());
        StartCoroutine(RotateRoutine());
    }
}
