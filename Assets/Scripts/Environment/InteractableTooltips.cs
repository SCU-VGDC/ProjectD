using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTooltips : Interactable
{
    private GameObject textGameObject;
    public override void Activation() 
    {
        textGameObject.SetActive(true);
    }

    public override void Deactivation() 
    {
        textGameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
       textGameObject = gameObject.transform.GetChild(0).gameObject;
    }

}
