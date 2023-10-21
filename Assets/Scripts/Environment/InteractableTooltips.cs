using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTooltips : Interactable
{
    private GameObject textGameObject;
    public override void Activation() {
        Debug.Log("Activation");
        textGameObject.SetActive(true);
    }
    public override void Deactivation() {
        Debug.Log("Deactivation");
        textGameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
       textGameObject = gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
