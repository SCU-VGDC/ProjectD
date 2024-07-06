using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLockInventory : MonoBehaviour
{
    //list of keys player has obtained
    public List<string> playerKeys;

    void Awake()
    {
        //create a set
        playerKeys = new List<string>();
    }

    // void Update()
    // {
    //     Debug.Log("Player keys are: Bronze = " + playerKeys.Contains("Bronze Key") + ". Silver = " + playerKeys.Contains("Silver Key") + ". Brass = " + playerKeys.Contains("Brass Key"));
    // }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //if other collider was a key...
        if(collider.gameObject.GetComponent<KeyBehavior>())
        {
            // Debug.Log("Key Touched");
            // Debug.Log(col.gameObject.GetComponent<KeyBehavior>().PrefabName);

            //add key getKey event
            EventManager.singleton.AddEvent(new addKey(gameObject, collider));
            
            // Debug.Log(playerKeys.Contains("bronze"));
            // Debug.Log(playerKeys.Contains("silver"));
            // Debug.Log(playerKeys.Contains("gold"));
        }
    }
}
