using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player_Movement_Namespace
{
    public class Player_Movement : MonoBehaviour
    {
        //basic movement vars:
        [HideInInspector] public float horizontal;
        public float speed = 0f;
        public float jump_power = 16f;
        [HideInInspector] public bool is_facing_right = true;

        //coyote jump vars:
        private float coyote_time = 0.2f;
        private float coyote_time_counter;

        //jump buffer vars:
        private float jump_buffer_time = 0.2f;
        private float jump_buffer_time_counter;

        //melee dash vars:
        public int maximum_dashes;
        public int current_dashes;
        private bool is_dashing;
        public float dash_power;
        public float dash_time;
        public float dash_recharge_time = 1f;
        [SerializeField] private float dash_recharge_time_counter;

        //wall slide vars:
        private bool is_wall_sliding;
        public float wall_slide_speed;

        //component and layer vars:
        public Rigidbody2D rb;
        public BoxCollider2D bc;
        [HideInInspector] public LayerMask plats_layer;
        [HideInInspector] public LayerMask semi_plats_layer;
        public TrailRenderer tr;
        public GameObject dash_damage_hitbox;
        public RaycastHit2D box_cast_hit;
        public Transform wall_check;

        //other objects
        public Player_Health player_health_obj;
        public GameObject isometric_diamond_obj;
        private SpriteRenderer isometric_diamond_sprite_rend;

        private void Start()
        {
            player_health_obj = GetComponent<Player_Health>();
            isometric_diamond_sprite_rend = isometric_diamond_obj.GetComponent<SpriteRenderer>();
            current_dashes = maximum_dashes;
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

            //*****Isometric Diamond color (Dash indicator)*****
            //if the player can dash...
            if(current_dashes > 0)
            {
                //make the isometric diamond red
                isometric_diamond_sprite_rend.color = Color.red;
            }
            else
            {
                //make the isometric diamond white
                isometric_diamond_sprite_rend.color = Color.white;
            }

            //*****Dashing*****
            //if dash button pressed...

            if(Input.GetButtonDown("Dash") && current_dashes > 0)

            {
                //dash
                StartCoroutine(Dash());

                //reset dash_recharge_rate
                dash_recharge_time_counter = 0f;
            }

            //if not dashing...
            if(!is_dashing)
            {
                //move with horizontal inputs
                rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

                //increment dash charge
                dash_recharge_time_counter += Time.deltaTime;
            }

            //if a dash should be recharged
            if(dash_recharge_time_counter >= dash_recharge_time && current_dashes < maximum_dashes)
            {
                //increment current_dashes
                current_dashes += 1;

                //reset dash_recharge_time_counter
                dash_recharge_time_counter = 0f;
            }

            Flip();
        }

        //returns true if the player is grounded, returns false if not
        private bool IsGrounded()
        {
            //box cast with plats_layer
            box_cast_hit = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, 0.1f, plats_layer);

            //if box cast with plats layer didn't hit something...
            if(box_cast_hit.collider == null)
            {
                //box cast with semi_plats_layer
                box_cast_hit = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, 0.1f, semi_plats_layer);
                
                //if box cast with semi_plats_layer hit something...
                if(box_cast_hit.collider == null)
                {
                    return false;
                }
                else //if box cast with semi_plats_layer didn't hit something...
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
            current_dashes -= 1;
            //can_dash = false;
            is_dashing = true;

            //disable gravity by storing original gravity then setting current gravity to zero
            float original_gravity = rb.gravityScale;
            rb.gravityScale = 0f;
            
            //zero rigidbody velocity
            rb.velocity = Vector2.zero;

            //*****dash*****
            //make player invulnerable for duration of dash
            player_health_obj.StartCoroutine("Become_Invulnerable_Dash");
            //normalize current dash_direction
            Player_Shooting.dash_direction.Normalize();
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

        }

        //debug visual for ground check
        void OnDrawGizmosSelected()
        {
            // Draw a semi-transparent red cube at the transform's position
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Vector2 true_center = bc.bounds.center + new Vector3(0, -0.6f);
            Gizmos.DrawCube(true_center, new Vector2(0.65f, 0.12f)); 
        }
    }
}
