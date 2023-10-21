using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator animatorSource;

    public List<string> boolsAnimation = new List<string>();
    public List<string> triggerAnimation = new List<string>();

    public void SetAnim(string name, bool value = false)
    {
        if (boolsAnimation.Contains(name))
        {
            animatorSource.SetBool(name, value);
            return;
        }

        if (triggerAnimation.Contains(name))
        {
            animatorSource.SetTrigger(name);
            return;
        }

        Debug.LogWarning(gameObject + " requested missing animation " + name);
    }
}