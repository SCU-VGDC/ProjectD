using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Element : MonoBehaviour
{
    // Start is called before the first frame update
    public float interactable_range: 2.0f;
    public LayerMask interactable_layer;

    // Update is called once per frame
    void Update()
    {
        //If J (can be changed later) is pressed, check for interactable element.
        if(Input.GetKeyDown(KeyCode.j)){
            interactable_elt_check();
        }
    }

    //To check if any object is interactable nearby
    private void interactable_elt_check() 
    {
        //If no objects that's in the interactable_layer is within the range of the player, return NULL.
        Collider2D interactableObject = Physics2D.OverlapCircle(transform.position, interactable_range, interactable_layer);

        if (interactableObject != null)
        {
            // Send a message to the interactable element.
            // This will call the "OnInteract" method on the interactable object's script
            interactableObject.SendMessage("OnInteract", SendMessageOptions.DontRequireReceiver);
        }
    }
}
