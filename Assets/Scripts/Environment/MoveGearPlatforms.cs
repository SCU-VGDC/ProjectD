using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGearPlatforms : MonoBehaviour
{
    public Transform start;
    public Transform end;

    private Transform platform;

    public float speed = 3f;

    private Vector3 target;

    void Start() 
    {
        platform = transform;

        platform.position = start.position;
        target = start.position;
    }

    void FixedUpdate() 
    {
        Vector3 currentPos = platform.position;
 
        if(currentPos != target) 
        {
            platform.position = Vector3.MoveTowards(platform.position, target, speed * Time.deltaTime);
        }
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
