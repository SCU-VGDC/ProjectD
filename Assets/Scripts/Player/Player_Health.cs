using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

namespace Player_Movement_Namespace
{
    public class Player_Health : MonoBehaviour
    {
        [Header("Health and Is Invulnerable")]
        public bool is_vul = true;
        [SerializeField] float invul_time;
        public float heal_radius = 0.5f;

        [Header("Sprite Renderer")]
        [SerializeField] public SpriteRenderer sr;

        [Header("Color")]
        Color sr_color;

        [Header("Other Objects")]
        [SerializeField] private Player_Movement player_movement_obj;
        [SerializeField] private Player_Shooting player_shooting_obj;
        private PersistentData pd;

        void Start()
        {
            pd = PersistentDataManager.inst.persistentData;

            //set sr to sprite renderer
            sr = GetComponent<SpriteRenderer>();
            //store color in sr_color
            sr_color = sr.material.color;
            //get an object for player movement
            player_movement_obj = GameManager.inst.playerMovement;
            player_shooting_obj = GameManager.inst.playerShooting;
        }

        void OnTriggerStay2D(Collider2D hit_info)
        {
            //Debug.Log(hit_info.gameObject.layer.ToString());
            //if player collided with an enemy (layer 9) or an enemy bullet (layer 12)...
            if((hit_info.gameObject.layer == 9 || hit_info.gameObject.layer == 12) && is_vul == true && pd.PlayerCurrentState == "alive")
            {
                //take damage
                pd.AddPlayerHealth(-1);

                //become invulnerable
                StartCoroutine("Become_Invulnerable_Damage");

                //if health is less than or equal to zero, destroy player
                if(pd.PlayerHealth <= 0)
                {
                    Die();
                }
            
            }
            else if(hit_info.gameObject.layer==15)
            {
                Die();
            }
            
        }

        //Co-routine for becoming invulnerable after taking damage
        IEnumerator Become_Invulnerable_Damage()
        {
            is_vul = false;

            sr_color.a = 0.5f;
            sr.material.color = sr_color;

            yield return new WaitForSeconds(invul_time);

            is_vul = true;

            sr_color.a = 1f;
            sr.material.color = sr_color;
        }

        //Co-routine for becoming invulnerable when dashing
        IEnumerator Become_Invulnerable_Dash()
        {
            is_vul = false;

            sr_color.a = 0.5f;
            sr.material.color = sr_color;

            yield return new WaitForSeconds(player_movement_obj.dash_time);

            is_vul = true;

            sr_color.a = 1f;
            sr.material.color = sr_color;
        }


        // Run this when the player dies 
        // Stops movement and shooting
        // Respawn is in Player Movement
        public void Die()
        {
            pd.PlayerCurrentState = "dead";
            player_shooting_obj.setCanShoot(false);            
            Debug.Log("LMAO you died XD");
            Debug.Log("Press enter to Respawn");
        }

        public void OnDrawGizmos()
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, heal_radius);
#endif
        }
    }
}
