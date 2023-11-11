using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableEndDoor : Interactable
{
    public string nextScene;

    public override void Activation() 
    {
        Debug.Log("1");
        StartCoroutine(GameSceneManager.inst.StartFadeScreen(true, nextScene));
        Debug.Log("2");
    }
}
