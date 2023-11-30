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
            EventManager.singleton.GetComponent<UIManager>().updateinteractUI(true);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (requiresActivation == false)
        {
            Deactivation();          
        }
        else
        {
            EventManager.singleton.LastInteractable = null;
            EventManager.singleton.GetComponent<UIManager>().updateinteractUI(false);
        }        
    }
}
