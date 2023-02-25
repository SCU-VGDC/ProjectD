using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player_Movement_Namespace
{
    public class Player_Movement : MonoBehaviour
    {
        //basic movement vars:
        [HideInInspector] public float horizontal;
        private float vertical;//vertical 
        public float speed;
        public float jump_power;
        [HideInInspector] public bool is_facing_right = true;

        //coyote jump vars:
        private float coyote_time = 0.2f;
        [SerializeField] private float coyote_time_counter;

        //jump buffer vars:
        private float jump_buffer_time = 0.2f;
        private float jump_buffer_time_counter;

        //melee dash vars:
        public int maximum_dashes;
        public int current_dashes;
        private bool is_dashing;
        public float dash_power;
        public float dash_distance;
        public float dash_time;
        public float dash_recharge_time = 1f;
        [SerializeField] private float dash_recharge_time_counter;
        
        //wall slide vars:
        private bool is_wall_sliding;
        private float wall_slide_speed=2f;
         [SerializeField] private Transform wallCheck;
         private bool iswalljumping;
         private float wallJumpingDirection;
         private float walljumpingtime = 0.2f;
         [SerializeField] private float walljumpingcounter;
         private float walljumpingduration = 0.35f; // might have to change to be bigger or smaller depend on length between walls jumping walls
         [SerializeField] private Vector2 wallJumpingPower=new Vector2(40f,15f);//og value 8 and 16 
         /// <summary> 0 means no wall, 1 means right wall, and 2 means left wall </summary>
         [SerializeField] private int lastWallTouched = 0;
         private bool hasWallJumped = false;
        //component and layer vars:
        public Rigidbody2D rb;
        public BoxCollider2D bc;
        public LayerMask plats_layer;
        public LayerMask semi_plats_layer;
        public TrailRenderer tr;
        public GameObject dash_damage_hitbox;
        public RaycastHit2D box_cast_hit;
        //public Transform wall_check;
        

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
            //this is for fast fall while pressing down
            vertical=Input.GetAxisRaw("Vertical");
            
            if(vertical<0 &&!IsGrounded())
                {
                    rb.gravityScale=100;
                }
            //*****coyote jump time set up*****:
            if (IsGrounded())
            {
                //reset coyote_time_counter
                coyote_time_counter = coyote_time;
                rb.gravityScale=6;
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
            if(coyote_time_counter > 0f && jump_buffer_time_counter > 0)
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

            //if not dashing or wall jumping...
            if(!is_dashing && !is_wall_sliding && !iswalljumping)
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

            //Flip();

            // reset lastWallTouched if grounded
            if (IsGrounded()) 
            {
                lastWallTouched = 0;
                hasWallJumped = false;
            }

            WallSlide();
            WallJump();
            if(!iswalljumping)
            {
                Flip();
            }
        }
    private bool IsWalled()
    {
        // Physics2D.CircleCast look into and do the cast to only the platforms layer
        Collider2D[] collidedWith =  Physics2D.OverlapCircleAll(wallCheck.position, 0.2f);

        foreach (Collider2D col in collidedWith)
        {
            if (col.tag != "Player")
            {
                return true;
            }
        }

        return false;
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            is_wall_sliding=true;
            rb.velocity=new Vector2(rb.velocity.x,Mathf.Clamp(rb.velocity.y,-wall_slide_speed,float.MaxValue));
        }
        else
        {
            is_wall_sliding=false;
        }
    }
    private void WallJump()
    {
        if(is_wall_sliding)
        {   
            iswalljumping=false;
            wallJumpingDirection=-transform.localScale.x;
            walljumpingcounter=walljumpingtime;

            if (Mathf.Sign(wallJumpingDirection) == 1)
            {
                if (lastWallTouched == 1) 
                {
                    hasWallJumped = false;
                }
                lastWallTouched = 2;
            } 
            else // wallJumpingDirection is negative 
            {
                if (lastWallTouched == 2) 
                {
                    hasWallJumped = false;
                }
                lastWallTouched = 1;
            }

            CancelInvoke(nameof(StopWalljumping));
        }
        else
        {
            walljumpingcounter -= Time.deltaTime;
        }
        
        if(Input.GetButtonDown("Jump") && walljumpingcounter > 0f && CanWallJump())
        {
            iswalljumping=true;
            hasWallJumped = true;
            rb.velocity=new Vector2(wallJumpingDirection*wallJumpingPower.x, wallJumpingPower.y);
            walljumpingcounter=0f;

            if(transform.localScale.x != wallJumpingDirection)
            {
                is_facing_right=!is_facing_right;
                Vector3 localScale=transform.localScale;
                localScale.x*=-1f;
                transform.localScale=localScale;
            }

            Invoke(nameof(StopWalljumping),walljumpingduration);
        }

        
    }

    private void StopWalljumping()
    {
        iswalljumping=false;
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

        private bool CanWallJump()
        {
            if (lastWallTouched == 0) // if you touched the ground last
            {
                return true;
            }
            else if (lastWallTouched == 1 && !hasWallJumped) // if right wall was touched and hasn't jumped yet
            {
               return true; 
            }
            else if (lastWallTouched == 2 && !hasWallJumped) // if left wall was touched and hasn't jumped yet
            {
                return true;
            }

            return false;
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
            //Gizmos.DrawCube(true_center, new Vector2(0.65f, 0.12f)); 

            // Debugging wall jump circle used to detect if touching a wall
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(wallCheck.position, 0.2f);
        }
    }
    
}
