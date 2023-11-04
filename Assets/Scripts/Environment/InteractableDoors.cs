using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoors : Interactable
{
    public Transform targetDoor;
    public override void Activation() 
    {
        GameManager.inst.player.transform.position=targetDoor.position;
    }
    
}
