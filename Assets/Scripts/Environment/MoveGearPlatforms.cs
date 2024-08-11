using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveGearPlatforms : MonoBehaviour
{
    public Transform start;
    public Transform end;

    private Transform platform;

    public float speed = 3f;

    private float timer;
    private float timerchange=1f;// this is the ammount of time to see how long before it moves back.

    private Vector3 target;

    void Start() 
    {
        platform = transform;

        platform.position = start.position;
        target = start.position;
        timer=0;//makes the timer start at zero initallty so it won't.
        
    }

    void FixedUpdate() 
    {
        Vector3 currentPos = platform.position;
        timer+=Time.deltaTime;
        //checks if timer is greatera than change and moves towards the start.
        if(timer>timerchange)
        {
            platform.position=Vector3.MoveTowards(platform.position,start.position,speed*Time.deltaTime);
        }
        else
        {
            platform.position=Vector3.MoveTowards(platform.position,end.position,speed*Time.deltaTime);
        }
        /*
        if(currentPos != target) 
        {
            platform.position = Vector3.MoveTowards(platform.position, target, speed * Time.deltaTime);
        }
        */
    }
    public void collided()//checks collider if it hits Check the event manager it should have the call for when it collides.
    {
        timer=0f;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        other.transform.SetParent(platform, true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        other.transform.SetParent(null);
    }

    public void ChangeMove()
    {
        if(target == start.position)
        {
            target = end.position;
            return;
        }

        if (target == end.position)
        {
            target = start.position;
            return;
        }
    }

    void OnDrawGizmosSelected()
    {
        #if UNITY_EDITOR
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(start.position, 1f);

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(end.position, 1f);
        #endif
    }
}
