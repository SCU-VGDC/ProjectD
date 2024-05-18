using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConveyorScript : MonoBehaviour
{
    // Positive is Right, Negative is Left
    [SerializeField] float ConvayerSpeed = 0f;

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7 || collision.gameObject.layer == 9)
        {

            Rigidbody2D other = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector3 force = new Vector3(other.velocity.x + ConvayerSpeed, other.velocity.y);

            other.AddForce(force);
        }
    }
}
