using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGearPlatforms : MonoBehaviour
{
    public GameObject player;
    public GameObject gear;
    public GameObject start;
    public GameObject end;

    private GameObject platform;

    public float speed = 3f;

    // testing stuff
    public bool trigger = false;
    public bool triggerback = false;

    private Vector3 target;

    // Start is called before the first frame update
    void Start() {
        platform = this.gameObject;

        platform.transform.position = start.transform.position;
        target = start.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 currentPos = platform.transform.position;

        // ----- Temporary Debug Stuff -----
        if(trigger) {
            Move();
            trigger = false;
        }

        if(triggerback) {
            MoveBack();
            triggerback = false;
        }
        // ---------------------------------
 
        if(currentPos != target) {
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, target, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        // Debug.Log("TriggerEnter" + other.gameObject);

        player.transform.SetParent(platform.transform, true);
    }

    private void OnTriggerExit2D(Collider2D other){
        // Debug.Log("TriggerExit" + other.gameObject);
        
        player.transform.SetParent(null);
    }

    void Move(){
        target = end.transform.position;
    }

    void MoveBack(){
        target = start.transform.position;
    }
}
