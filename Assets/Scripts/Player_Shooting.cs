using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player_Movement_Namespace
{
    public class Player_Shooting : MonoBehaviour
    {
        //number vars
        public float player_bullet_speed;
        public float fire_rate_time;
        [HideInInspector] public float fire_rate_time_counter;
        public static Vector3 dash_direction;

        //Gameobject vars
        public GameObject rotate_point;
        public Transform fire_point;
        public GameObject player_bullet_prefab;
        private Camera main_camera;
        private Vector3 mouse_pos;

        void Start()
        {
            main_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            //*****Aiming:*****
            mouse_pos = main_camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 rotation = mouse_pos - rotate_point.transform.position;
            float rotation_z = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            rotate_point.transform.rotation = Quaternion.Euler(0, 0, rotation_z);

            //*****Dash direction calculation*****
            dash_direction = (Vector2)(mouse_pos - transform.position);

            //*****Shooting:*****
            if(Input.GetButtonDown("Shoot") && (fire_rate_time_counter >= fire_rate_time))
            {
                //Debug.Log("The Player fired a bullet");
                //create a bullet
                Instantiate(player_bullet_prefab, fire_point.position, Quaternion.identity);

                //reset fire_rate_time_counter
                fire_rate_time_counter = 0f;
            }

            //increment fire_rate_time_counter
            fire_rate_time_counter += Time.deltaTime;
        }
    }
}