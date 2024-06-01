using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoors : Interactable
{
    public Transform targetDoor;
    private int numChildren;
    private Transform[] paralaxBackgrounds;
    private Vector3[] paralaxToPlayerOffsets;
    private int cooldown = 1;
    private bool canTeleportAgain = true;
    
    void Start()
    {
        //find background paralax transform
        Transform BackGroundParallaxTrans = GameObject.Find("BackGroundParallax").GetComponent<Transform>();

        //get numebr of children of background paralax
        numChildren = BackGroundParallaxTrans.childCount;

        //set size of paralaxBackgrounds
        paralaxBackgrounds = new Transform[numChildren];

        //set size of paralaxToPlayerOffsets
        paralaxToPlayerOffsets = new Vector3[numChildren];

        //for every child of BackGroundParallaxTrans
        for(int i = 0; i < numChildren; i++)
        {
            //put all children of background paralax into paralaxBackgrounds
            paralaxBackgrounds[i] = BackGroundParallaxTrans.GetChild(i);

            //put the offset between each of background paralax and the player into paralaxToPlayerOffsets
            paralaxToPlayerOffsets[i] = GameManager.inst.player.transform.position - paralaxBackgrounds[i].position;
        }
    }
    
    public override void Activation() 
    {
        if (canTeleportAgain)
        {
            canTeleportAgain = false;
            targetDoor.GetComponent<InteractableDoors>().canTeleportAgain = false;
            GameManager.inst.player.transform.position = targetDoor.position;

            //for every child of BackGroundParallaxTrans
            for(int i = 1; i < numChildren; i++)
            {
                //move child to targetDoor's x position plus it's initial offset from the player's x position
                paralaxBackgrounds[i].position = new Vector3(targetDoor.position.x + paralaxToPlayerOffsets[i].x, paralaxBackgrounds[i].position.y, 0f);
            }

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
