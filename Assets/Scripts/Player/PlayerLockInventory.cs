using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLockInventory : MonoBehaviour
{
    //list of keys player has obtained
    public Dictionary<string, bool> playerKeys;

    // Start is called before the first frame update
    // void Start()
    // {
    //     //make playerKeys an array of size 3
    //     playerKeys = new Dictionary<string, bool>();
    // }

    void OnTriggerEnter2D(Collider2D col)
    {
        //if other collider was a key...
        if(col.gameObject.GetComponent<KeyBehavior>())
        {
            Debug.Log("Key Touched");
            Debug.Log(col.gameObject.GetComponent<KeyBehavior>().keyType);
            //add key to inventory
            playerKeys.Add(col.gameObject.GetComponent<KeyBehavior>().keyType, true);
            Debug.Log(playerKeys.ContainsKey("bronze"));
            Debug.Log(playerKeys.ContainsKey("silver"));
            Debug.Log(playerKeys.ContainsKey("gold"));

            //destroy key
            Destroy(col.gameObject);
        }
    }
}
