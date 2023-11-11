using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoors : Interactable
{
    public Transform targetDoor;
    private int cooldown = 1;
    private bool canTeleportAgain = true;
    public override void Activation() 
    {
        if (canTeleportAgain)
        {
            canTeleportAgain = false;
            targetDoor.GetComponent<InteractableDoors>().canTeleportAgain = false;
            GameManager.inst.player.transform.position = targetDoor.position;        
            StartCoroutine(DoorCool());
            StartCoroutine(targetDoor.GetComponent<InteractableDoors>().DoorCool());
        }
    }

    public IEnumerator DoorCool()
    {   
        yield return new WaitForSeconds(cooldown);
        canTeleportAgain = true;
    }
    
}
