using Player_Movement_Namespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBattleStarter : MonoBehaviour
{
    public float maxdist = 0.3f;
    private GameObject pl;

    void Start()
    {
        pl = GameManager.inst.player; //gets player
    }

    void FixedUpdate()
    {
        float dist = Vector2.Distance(pl.transform.position, transform.position);
        if(dist < maxdist)
        {
            transform.parent.GetComponent<RoomManager>().Startbattle(); //starts battle if player is inside
        }
    }

    void OnDrawGizmos()
    {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxdist); //drawing the player agro area
    }
}
