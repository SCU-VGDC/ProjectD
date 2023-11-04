using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool requiresActivation;

    public abstract void Activation();

    public virtual void Deactivation() { }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (requiresActivation == false)
        {
            if (collision.gameObject.layer == 7) //player layer
            {
                Activation();
            }
        }
        else
        {
            EventManager.singleton.LastInteractable = this;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (requiresActivation == false)
        {
            Deactivation();
            EventManager.singleton.LastInteractable = null;
        }
    }
}
