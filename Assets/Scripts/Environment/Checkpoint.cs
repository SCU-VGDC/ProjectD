using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

public class Checkpoint : Interactable
{
    public int CheckPointId;
    public bool LastActivated;

    public override void Activation()
    {
        if(LastActivated)
        {
            return;
        }

        foreach (Checkpoint cp in FindObjectsOfType<Checkpoint>())
        {
            cp.LastActivated = false;
        }

        LastActivated = true;

        EventManager.singleton.AddEvent(new newCheckPointmsg(gameObject));
    }
}