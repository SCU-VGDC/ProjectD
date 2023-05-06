using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player_Movement_Namespace {
    public class DeahBox_S : MonoBehaviour
    {
        [SerializeField] private GameObject player;

        [SerializeField] private Player_Health player_health_obj;
        void Start(){
            player = GameObject.Find("Player");
            player_health_obj = player.GetComponent<Player_Health>();
        }

        void OnTriggerEnter2D(Collider2D Other) {
                
            if (Other.gameObject.layer == 7){
                player_health_obj.Die();
            }
        }


    }
}
