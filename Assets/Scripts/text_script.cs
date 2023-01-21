using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player_Movement_Namespace
{
    public class Player_Dash_Behavior : MonoBehaviour
    {
        [SerializeField] Rigidbody2D rb;
        [SerializeField] float dash_power;
        [SerializeField] float speed;
        [SerializeField] float horizontal;
        private bool is_dashing;

        void Update()
        {
            horizontal = Input.GetAxisRaw("Horizontal");

            if(Input.GetButtonDown("Dash"))
            {
                Debug.Log("Slice! C");
                //rb.velocity = new Vector2(transform.localScale.x + 100f, 100f);
                StartCoroutine(Dash_2());
                //rb.AddForce(new Vector3(rb.velocity.x + Player_Shooting.dash_direction.x, rb.velocity.y + Player_Shooting.dash_direction.y).normalized * dash_power);
                Debug.Log("Slice! D");
            }

            if(!is_dashing)
                rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }

        IEnumerator Dash_2()
        {
            float original_gravity = rb.gravityScale;
            rb.gravityScale = 0f;
            
            //zero rigidbody velocity
            rb.velocity = Vector2.zero;

            //dash
            is_dashing = true;
            rb.velocity = new Vector2(Player_Shooting.dash_direction.x, Player_Shooting.dash_direction.y).normalized * dash_power;
            yield return new WaitForSeconds(0.2f);
            is_dashing = false;
            Debug.Log(Player_Shooting.dash_direction);
            rb.gravityScale = original_gravity;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
