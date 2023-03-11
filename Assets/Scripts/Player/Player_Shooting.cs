using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player_Movement_Namespace
{
    public class Player_Shooting : MonoBehaviour
    {
        [Header("Number Variables")]
        public float fire_rate_time;
        [HideInInspector] public float fire_rate_time_counter;
        public static Vector3 dash_direction;
        private float bullet_range = 100f;

        [Header("Damage Variables")]
        public Enemy_Health enemy_health;
        public int player_bullet_damage;

        [Header("GameObjects")]
        public GameObject rotate_point;
        public Transform fire_point;
        public GameObject bullet_trail_prefab; 
        private Camera main_camera;
        private Vector3 mouse_pos;

        [Header("Death Variables")]
        public bool canShoot;

        void Start()
        {
            main_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            canShoot = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!canShoot){
                return;
            }
            //*****Aiming:*****
            mouse_pos = main_camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 rotation = mouse_pos - rotate_point.transform.position;
            float rotation_z = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            rotate_point.transform.rotation = Quaternion.Euler(0, 0, rotation_z);

            //*****Dash direction calculation*****
            dash_direction = (Vector2)(mouse_pos - transform.position);
            //permanently normalize the vector
            dash_direction.Normalize();

            //*****Shooting:*****
            if(Input.GetButtonDown("Shoot") && (fire_rate_time_counter >= fire_rate_time))
            {
                //Shoot
                //NOTE: bitwise operators are used to create a layermask for layers 6 (platforms) and layer 9 (enemies)
                RaycastHit2D hit = Physics2D.Raycast((Vector2)fire_point.position, dash_direction, bullet_range, (1<<6) | (1<<9)); 
                GameObject trail = Instantiate(bullet_trail_prefab, fire_point.position, transform.rotation);
                Bullet_Trail_Behavior trailScript = trail.GetComponent<Bullet_Trail_Behavior>();

                //if the raycast hit something,...
                if(hit.collider != null)
                {
                    //change the end point of the trail
                    trailScript.SetTargetPosition((Vector3)hit.point);

                    //if the hit collider was of an enemy,...
                    //NOTE: enemy layer is layer 9
                    if(hit.transform.gameObject.layer == 9) 
                    {
                        //Get enemy health component of enemy
                        enemy_health = hit.transform.gameObject.GetComponent<Enemy_Health>();
                        
                        //if the enemy has a health component,...
                        if(enemy_health != null)
                        {
                            //Make the enemy take damage
                            enemy_health.Take_Damage(player_bullet_damage);
                        }
                    }
                    //Debug.Log("Player hit something! It was: " + hit.collider.name);
                }
                else //if the raycast did not hit something,...
                {
                    //Make the end point of the trail the maximum range of bullet
                    Vector3 endPosition = fire_point.position + dash_direction * bullet_range;
                    trailScript.SetTargetPosition(endPosition);
                }

                //reset fire_rate_time_counter
                fire_rate_time_counter = 0f;
            }

            //increment fire_rate_time_counter
            fire_rate_time_counter += Time.deltaTime;
        }

        public void setCanShoot(bool other){
            canShoot = other;
        }

    }
}