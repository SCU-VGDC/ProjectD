using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    // Positive is Right, Negative is Left
    [SerializeField] float ConvayerSpeed = 0f;
    // Update is called once per frame
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7 || collision.gameObject.layer == 9) ;
        {
            Debug.Log("Touching " + collision);
            Rigidbody2D other = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector3 force = new Vector3(other.velocity.x + ConvayerSpeed, other.velocity.y);

            other.AddForce(force);
        }
    }
}
