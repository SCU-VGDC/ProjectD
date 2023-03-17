using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player_Movement_Namespace {

    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        
        [SerializeField] private Collider2D playerCollider;
        [SerializeField] private GameObject thisCheckPoint;
        [SerializeField] private Collider2D checkpointCollider;
        [SerializeField] public Player_Movement playerMovement;
        void Start(){
            player = GameObject.Find("Player");
            playerCollider = GetComponent<Collider2D>();
            checkpointCollider = GetComponent<Collider2D>();
            playerMovement = player.GetComponent<Player_Movement>();
            //thisCheckPoint = GetComponent<Checkpoint>();


        }

        void OnTriggerEnter2D(Collider2D Other) {
            //players hitbox is hit updates checkpoint
            
            if (Other.gameObject.layer == 7){
                playerMovement.setCheckpoint(thisCheckPoint);
                Debug.Log("Checkpoint set to " + playerMovement.getCheckPoint());
            }
        }






   }
}