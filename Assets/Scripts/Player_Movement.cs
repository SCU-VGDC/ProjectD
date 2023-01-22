using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player_Movement_Namespace
{
    public class Player_Movement : MonoBehaviour
    {
        //basic movement vars:
        public float horizontal;
        public float speed = 0f;
        public float jump_power = 16f;
        public bool is_facing_right = true;

        //coyote jump vars:
        private float coyote_time = 0.2f;
        private float coyote_time_counter;

        //jump buffer vars:
        private float jump_buffer_time = 0.2f;
        private float jump_buffer_time_counter;

        //melee dash vars:
        private bool can_dash = true;
        private bool is_dashing;
        public float dash_power;
        public float dash_time = 0.2f;
        public float dash_cooldown = 1f;
        public float dash_distance;

        //wall slide vars:
        private bool is_wall_sliding;
        public float wall_slide_speed;

        //component and layer vars:
        [SerializeField] public Rigidbody2D rb;
        [SerializeField] public BoxCollider2D bc;
        [SerializeField] public LayerMask plats_layer;
        [SerializeField] public LayerMask semi_plats_layer;
        [SerializeField] public TrailRenderer tr;
        [SerializeField] public GameObject dash_damage_hitbox;
        public RaycastHit2D box_cast_hit;
        [SerializeField] public Transform wall_check;

        //other objects
        public Player_Health player_health_obj;

        private void Start()
        {
            player_health_obj = GetComponent<Player_Health>();
        }

        private void Update()
        {
            //prevent player from moving or jumping while dashing
            if(is_dashing)
            {
                return;
            }

            //*****Left and Right movement*****:
            //get horizontal input
            horizontal = Input.GetAxisRaw("Horizontal");

            //*****coyote jump time set up*****:
            if (IsGrounded())
            {
                //reset coyote_time_counter
                coyote_time_counter = coyote_time;
            }
            else
            {
                //count down coyote_time_counter
                coyote_time_counter -= Time.deltaTime;
            }

            //*****jump buffer set up*****
            if(Input.GetButtonDown("Jump"))
            {
                //reset jump_buffer_time_counter
                jump_buffer_time_counter = jump_buffer_time;
            }
            else
            {
                //count down jump_buffer_time_counter
                jump_buffer_time_counter -= Time.deltaTime;
            }

            //*****jumping*****:
            //if coyote_time_counter > 0 and jump button is pressed...
            if(coyote_time_counter > 0 && jump_buffer_time_counter > 0)
            {
                //make character jump
                rb.velocity = new Vector2(rb.velocity.x, jump_power);
                //set jump_buffer_time_counter to 0
                jump_buffer_time_counter = 0f;
            }

            //if jump button is unpressed and the character is still rising...
            if(Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            {
                //make character fall faster (due to variable jump height)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                //set coyote_time_counter to 0 (prevents double jumping due to spam jumping)
                coyote_time_counter = 0f;
            }

            //*****Dashing*****
            //if dash button pressed...
            if(Input.GetButtonDown("Dash") && can_dash)
            {
                //dash
                StartCoroutine(Dash());
            }

            //if not dashing...
            if(!is_dashing)
            {
                //move with horizontal inputs
                rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            }

            Flip();
        }

        //returns true if the player is grounded, returns false if not
        private bool IsGrounded()
        {
            //box cast with plats_layer
            //OLD CODE: 
            box_cast_hit = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, 0.1f, plats_layer);
            //box_cast_hit = Physics2D.BoxCast(bc.bounds.center, new Vector2(0.65f, 0.12f), 0f, Vector2.down, 0.6f, plats_layer);

            //if box cast with plats layer didn't hit something...
            if(box_cast_hit.collider == null)
            {
                //box cast with semi_plats_layer
                //OLD CODE: 
                box_cast_hit = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, 0.1f, semi_plats_layer);
                //box_cast_hit = Physics2D.BoxCast(bc.bounds.center, new Vector2(0.65f, 0.12f), 0f, Vector2.down, 0.6f, semi_plats_layer);
                
                //if box cast with semi_plats_layer hit something...
                if(box_cast_hit.collider == null)
                {
                    return false;
                }
                else //if box cast with semi_plats_layer 
                {
                    return true;
                }
            }
            else //if box cast with plats_layer didn't hit something...
            {
                return true;
            }
        }

        //flip player depending on direction
        private void Flip()
        {
            if((is_facing_right && horizontal < 0f) || (!is_facing_right && horizontal > 0f))
            {
                is_facing_right = !is_facing_right;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }

        //corotine that handles dashing
        private IEnumerator Dash()
        {
            can_dash = false;
            is_dashing = true;

            //disable gravity by storing original gravity then setting current gravity to zero
            float original_gravity = rb.gravityScale;
            rb.gravityScale = 0f;
            
            //zero rigidbody velocity
            rb.velocity = Vector2.zero;

            //*****dash*****
            //make player invulnerable for duration of dash
            player_health_obj.StartCoroutine("Become_Invulnerable_Dash");
            //launch player in direction of mouse
            //rb.velocity = new Vector2(Player_Shooting.dash_direction.x, Player_Shooting.dash_direction.y).normalized * dash_power;
            rb.AddForce(new Vector2(Player_Shooting.dash_direction.x * dash_distance, Player_Shooting.dash_direction.y * dash_distance), ForceMode2D.Impulse);
            //turn on trail renderer
            tr.emitting = true;
            //enable dash damage hitbox
            dash_damage_hitbox.SetActive(true);

            //wait for dashtime
            yield return new WaitForSeconds(dash_time);

            //*****end dash*****
            //turn off trail renderer
            tr.emitting = false;
            //set gravity to original gravity
            rb.gravityScale = original_gravity;
            //disable dash damage hitbox
            dash_damage_hitbox.SetActive(false);
            //set is_dashing to false
            is_dashing = false;
            //******TEST*******
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

            //cooldown
            yield return new WaitForSeconds(dash_cooldown);
            Debug.Log("Dash recharged!");
            can_dash = true;
        }

        //debug visual for ground check
        void OnDrawGizmosSelected()
        {
            // Draw a semitransparent red cube at the transform's position
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Vector2 true_center = bc.bounds.center + new Vector3(0, -0.6f);
            //Gizmos.DrawCube(true_center, bc.bounds.size);
            Gizmos.DrawCube(true_center, new Vector2(0.65f, 0.12f)); 
        }
    }
}
