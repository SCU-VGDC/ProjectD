using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTip : MonoBehaviour
{
    private GameObject textGameObject;
    void Start()
    {
        textGameObject = gameObject.transform.GetChild(0).gameObject;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        // game object is the player
        if (other.gameObject.layer == 7)
        {
            textGameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // game object is the player
        if (other.gameObject.layer == 7)
        {
            textGameObject.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
