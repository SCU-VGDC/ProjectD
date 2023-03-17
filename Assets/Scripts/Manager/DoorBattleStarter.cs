using Player_Movement_Namespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBattleStarter : MonoBehaviour
{
    public float maxdist = 0.3f;
    private Player_Health pl;

    void Start()
    {
        pl = GameObject.Find("Player").GetComponent<Player_Health>();
    }

    void FixedUpdate()
    {
        float dist = Vector2.Distance(pl.transform.position, transform.position);
        if(dist < maxdist)
        {
            transform.parent.GetComponent<RoomManager>().Startbattle();
        }
    }

    void OnDrawGizmos()
    {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxdist); //drawing the player agro area
    }
}
