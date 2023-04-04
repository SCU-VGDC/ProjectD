using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

namespace Player_Movement_Namespace {

    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        
        [SerializeField] private Collider2D playerCollider;
        [SerializeField] private GameObject thisCheckPoint;
        [SerializeField] private Collider2D checkpointCollider;
        [SerializeField] public Player_Movement playerMovement;

        [Header("Game Manager")]
        private PersistentData pd;
        void Start(){
            pd = GameObject.Find("Persistent Data Manager").GetComponent<PersistentDataManager>().persistentData;

            playerCollider = GetComponent<Collider2D>();
            checkpointCollider = GetComponent<Collider2D>();
            playerMovement = player.GetComponent<Player_Movement>();
        }

        void OnTriggerEnter2D(Collider2D Other) {
            //players hitbox is hit updates checkpoint
            
            if (Other.gameObject.layer == 7){
                pd.PlayerCurrentCheckpoint = thisCheckPoint;
            }
        }






   }
}