using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator animatorSource;

    public List<string> boolsAnimation = new List<string>();
    public List<string> triggerAnimation = new List<string>();

    public bool SetAnim(string name, bool value = false, Action callbackWhenIdle = null)
    {
        if (boolsAnimation.Contains(name))
        {
            //Debug.Log("test: boolsAnimation contains " + name);
            animatorSource.SetBool(name, value);

            if (callbackWhenIdle != null)
            {
                StartCoroutine(CallbackCoroutine(callbackWhenIdle));
            }

            return true;
        }

        if (triggerAnimation.Contains(name))
        {
            if(value==false)
            {
            animatorSource.SetTrigger(name);
            }
            else if(value==true)//this is so we can have a reset trigger function. To make sure it trigger stuff correcttly.
            {
                animatorSource.ResetTrigger(name);
            }

            if (callbackWhenIdle != null)
            {
                callbackWhenIdle?.Invoke();
            }

            return true;
        }

        Debug.LogWarning(gameObject + " requested missing animation " + name);
        return false;
    }

    private IEnumerator CallbackCoroutine(Action callback)
    {
        yield return new WaitUntil( delegate { return animatorSource.GetCurrentAnimatorStateInfo(0).IsName("Idle"); });

        callback?.Invoke();
    }
}