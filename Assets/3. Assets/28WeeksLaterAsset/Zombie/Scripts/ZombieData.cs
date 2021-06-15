using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Zombie Data", menuName = "Scriptable Object/Zombie Data", order = int.MaxValue)]
public class ZombieData : ScriptableObject
{
    [SerializeField]
    private float fieldOfViewAngle = 90.0f;
    public float FieldOfViewAngle { get { return fieldOfViewAngle; } }
    [SerializeField]
    private float viewDistance = 20.0f;
    public float ViewDistance { get { return viewDistance; } }
    [SerializeField]
    private LayerMask playerLayer;
    public LayerMask PlayerLayer { get { return playerLayer; } }
    [Header("Zombie Info")]
    [SerializeField]
    private float initHp = 100;
    public float InitHp { get { return initHp; } }
    [SerializeField]
    private float attackDist = 1.75f;
    public float AttackDist { get { return attackDist; } }
    [SerializeField]
    private float spreadDist = 2.5f;
    public float SpreadDist { get { return spreadDist; } }
    [SerializeField]
    private float talkDist = 2.5f;
    public float TalkDist { get { return talkDist; } }
    [SerializeField]
    private float patrolRadius = 15.0f;
    public float PatrolRadius { get { return patrolRadius; } }
    [SerializeField]
    private float callZombies = 7.0f;
    public float CallZombies { get { return callZombies; } }
    [SerializeField]
    private float patrolSpeed = 1.5f;
    public float PatrolSpeed { get { return patrolSpeed; } }
    [SerializeField]
    private float chaseSpeed = 5.0f;
    public float ChaseSpeed { get { return chaseSpeed; } }
    [SerializeField]
    private float damping = 2.0f;
    public float Damping { get { return damping; } }

    [Header("Animation Hash")]
    private int hashAttack = Animator.StringToHash("Attack");
    public int HashAttack { get { return hashAttack; } }
    private int hashSpeed = Animator.StringToHash("Speed");
    public int HashSpeed { get { return hashSpeed; } }
    private int hashMove = Animator.StringToHash("Move");
    public int HashMove { get { return hashMove; } }
    private readonly int hashDie = Animator.StringToHash("Die");
    public int HashDie { get { return hashDie; } }
    private int hashHungry = Animator.StringToHash("Hungry");
    public int HashHungry { get { return hashHungry; } }
    private int hashCommunication = Animator.StringToHash("Communication");
    public int HashCommunication { get { return hashCommunication; } }
    private int hashMatchComplete = Animator.StringToHash("MatchComplete");
    public int HashMatchComplete { get { return hashMatchComplete; } }
    private int hashBite = Animator.StringToHash("Bite");
    public int HashBite { get { return hashBite; } }
    private int hashStandUp = Animator.StringToHash("StandUp");
    public int HashStandUp { get { return hashStandUp; } }
    private int hashFoundTarget = Animator.StringToHash("FoundTarget");
    public int HashFoundTarget { get { return hashFoundTarget; } }
    private int hashNotFoundTarget = Animator.StringToHash("NotFoundTarget");
    public int HashNotFoundTarget { get { return hashNotFoundTarget; } }

}
