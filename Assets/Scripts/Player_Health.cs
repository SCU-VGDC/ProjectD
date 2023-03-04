using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player_Movement_Namespace
{
    public class Player_Health : MonoBehaviour
    {
        //health and is invulnerable vars
        public int health;
        public bool is_vul = true;
        [SerializeField] float invul_time;

        //sprite renderer
        [SerializeField] public SpriteRenderer sr;

        //color
        Color savedColor;

        //other objects
        private Player_Movement player_movement_obj;

        void Start()
        {
            //set sr to sprite rendere
            sr = GetComponent<SpriteRenderer>();
            //store color in sr_color
            savedColor = sr.color;
            //get an object for player movement
            player_movement_obj = GetComponent<Player_Movement>();
        }

        void OnTriggerStay2D(Collider2D hit_info)
        {
            //Debug.Log(hit_info.gameObject.layer.ToString());
            //if player collided with an enemy (layer 9) or an enemy bullet (layer 12)...
            if((hit_info.gameObject.layer == 9 || hit_info.gameObject.layer == 12) && is_vul == true)
            {
                //take damage
                health -= 1;

                //become invulnerable
                StartCoroutine("Become_Invulnerable_Damage");

                //if health is less than or equal to zero, destroy player
                if(health <= 0)
                {
                    Die();
                }
            }
            
        }
        //public member function to call getdamage from melle attack
        public void GetDamage(int damage)
        {
            if (is_vul)
            {
                health = health - damage;
                //become invulnerable
                StartCoroutine("Become_Invulnerable_Damage");

                //if health is less than or equal to zero, destroy player
                if (health <= 0)
                {
                    Die();
                }
            }
        }

        //Co-routine for becoming invulnerable after taking damage
        IEnumerator Become_Invulnerable_Damage()
        {
            is_vul = false;         

            sr.color = new Color(1,0,0,0.5f) ;

            yield return new WaitForSeconds(invul_time);

            is_vul = true;

            sr.color = savedColor;
        }

        //Co-routine for becoming invulnerable when dashing
        IEnumerator Become_Invulnerable_Dash()
        {
            is_vul = false;

            sr.color = new Color(1, 0, 0, 0.5f);

            yield return new WaitForSeconds(player_movement_obj.dash_time);

            is_vul = true;

            sr.color = savedColor;
        }

        //Function to destroy player gameobject
        //PLEASE NOTE: This is not how player death should be handled in a working build.
        //Instead, we should have a state that holds when the player is dead and what to
        //do from there. But for now, this code will suffice.
        public void Die()
        {
            Debug.Log("Player has died!!!");
            Destroy(gameObject);
        }
    }
}
