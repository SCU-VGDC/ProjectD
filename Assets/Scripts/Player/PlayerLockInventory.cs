using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLockInventory : MonoBehaviour
{
    //list of keys player has obtained
    public HashSet<string> playerKeys;

    // Start is called before the first frame update
    void Start()
    {
        //create a set
        playerKeys = new HashSet<string>();
    }

    void Update()
    {
        Debug.Log(playerKeys);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //if other collider was a key...
        if(collider.gameObject.GetComponent<KeyBehavior>())
        {
            // Debug.Log("Key Touched");
            // Debug.Log(col.gameObject.GetComponent<KeyBehavior>().keyType);

            //add key getKey event
            EventManager.singleton.AddEvent(new addKey(gameObject, collider));
            
            // Debug.Log(playerKeys.Contains("bronze"));
            // Debug.Log(playerKeys.Contains("silver"));
            // Debug.Log(playerKeys.Contains("gold"));
        }
    }
}
