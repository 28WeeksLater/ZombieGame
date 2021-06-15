using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimator : ZombieActionBase
{
    
    private Animator _animator;
    
    public static Dictionary<string, float> clipTimes;
    private bool AttackFinished = true;
    
    private static readonly int ZombieAttackSpeed = Animator.StringToHash("ZombieAttackSpeed");

    protected override void OnEnable()
    {
        base.OnEnable();
        _animator = GetComponent<ZombieBehavior>().animator;
        UpdateAnimClipTimes();
    }
    
    private void UpdateAnimClipTimes()
    {
        clipTimes = new Dictionary<string, float>();
        
        var clips = _animator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            if (clipTimes.ContainsKey(clip.name)) continue;
            if (clip.name.Equals("Attack"))
            {
                clipTimes.Add(clip.name, clip.length/_animator.GetFloat(ZombieAttackSpeed));
                continue;
            }
            clipTimes.Add(clip.name, clip.length);
        }
    }

    protected override void WhenDie()
    {
        _animator.SetBool(zombieData.HashMove, false);
    }

    protected override void WhenIdle()
    {
        _animator.SetBool(zombieData.HashMove, false);
    }

    protected override void WhenPatrol()
    {
        _animator.SetBool(zombieData.HashMove, true);
    }

    protected override void WhenChasing()
    {
        if (_animator.GetBool(zombieData.HashNotFoundTarget))
        {
            _animator.SetTrigger(zombieData.HashFoundTarget);
        }
        
        _animator.SetBool(zombieData.HashMove, true);
        
    }

    protected override void WhenAttack()
    {
        if(!AttackFinished) return;
        StartCoroutine(WhenAttackAfter());
    }

    IEnumerator WhenAttackAfter()
    {
        AttackFinished = false;
        while (behavior.CurrentState == ZombieBehavior.ZombieState.ATTACK)
        {
            _animator.SetBool(zombieData.HashMove, false);
            _animator.SetTrigger(controller.HasPath ? zombieData.HashAttack : zombieData.HashNotFoundTarget);
            yield return new WaitForSeconds(clipTimes["Attack"]);
        }
        AttackFinished = true;
    }

    protected override void WhenTrigger()
    {
        if (_animator.GetBool(zombieData.HashNotFoundTarget))
        {
            _animator.SetTrigger(zombieData.HashFoundTarget);
        }
        _animator.SetBool(zombieData.HashMove, true);
        
    }

    protected override void WhenAngry()
    {
        StartCoroutine(WhenAngryAfter());
    }

    IEnumerator WhenAngryAfter()
    {
        yield return new WaitForSeconds(clipTimes["Scream"]);
        behavior.CommunicationNeed = Random.Range(1f, 20f);
        //behavior.SetRandomState();
    }

    protected override void WhenTalk()
    {
        _animator.SetBool(zombieData.HashMove, true);
        if (!(Vector3.Distance(transform.position, controller.Target.position) <= zombieData.TalkDist)) return;
        _animator.SetBool(zombieData.HashMatchComplete, true);
        StartCoroutine(WhenTalkAfter());
    }

    IEnumerator WhenTalkAfter()
    {
        yield return new WaitForSeconds(clipTimes["Talk"]);
        _animator.SetBool(zombieData.HashMatchComplete, false);
        _animator.SetBool(zombieData.HashMove, false);
    }

    protected override void WhenBite()
    {
        _animator.SetBool(zombieData.HashMove, true);
        StartCoroutine(WhenBiteAfter());
    }
    
    IEnumerator WhenBiteAfter()
    {
        while (true) 
        {
            yield return new WaitForSeconds(0.5f);

            if ((Vector3.Distance(controller.Target.position, transform.position) <= zombieData.AttackDist))
            {
                _animator.SetBool(zombieData.HashBite, true);
                yield return new WaitForSeconds(clipTimes["Bite"]);

                behavior.HungryNeeds = (int) Random.Range(0f, 20f);
                _animator.SetBool(zombieData.HashBite, false);
                
                if (controller.Target.gameObject.activeInHierarchy)
                {
                    break;
                }
                _animator.SetBool(zombieData.HashMove, false);
            }
            else if (!controller.Target.gameObject.activeInHierarchy)
            {
                behavior.HungryNeeds = (int) Random.Range(0f, 20f);
                _animator.SetBool(zombieData.HashMove, false);
                break;
            }
        }
    }
    
}
