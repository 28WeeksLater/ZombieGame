using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EndingCredit : MonoBehaviour
{
    private Animator _animator;
    private readonly int hashScroll = Animator.StringToHash("IsScrolling");
    private float animTime = 0;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        AnimationTime();
        StartCoroutine(Scrolling());
    }

    IEnumerator Scrolling()
    {
        yield return new WaitForSeconds(5.0f);
        _animator.SetTrigger(hashScroll);
        yield return new WaitForSeconds(animTime + 7.0f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    private void AnimationTime()
    {
        var name = "Scrolling";
        var runtimeAnim = _animator.runtimeAnimatorController;

        foreach (var i in runtimeAnim.animationClips)
        {
            if (i.name == name)
            {
                animTime = i.length;
            }
        }
    }
    
}
