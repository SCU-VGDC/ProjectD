using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player_Movement_Namespace
{
    public class ui_dashes: MonoBehaviour
    {
        public GameObject player;
        public Player_Movement player_movement;
        public SpriteRenderer renderer; // change to cgange image
        public Sprite[] spriteArray;

        void Start()
        {
            renderer = gameObject.GetComponent<SpriteRenderer>();// change to image ;-;
            player = GameObject.FindWithTag("Player");
            player_movement = player.GetComponent<Player_Movement>();
            //0_DASH = Sprites.Load() <Sprite>("empty"); //maybe Resources instead of Sprites?
            //1_DASH = Sprites.Load() <Sprite>("1dash"); 
           // 2_DASH = Sprites.Load() <Sprite>("2dashes"); 
            //3_DASH = Sprites.Load() <Sprite>("3dashes");
        }
        
        private void Update() {
            showDashes();
        }
        void showDashes()
        {
            switch(player_movement.current_dashes)
            {
                case 0:
                    renderer.sprite = spriteArray[0];
                    //gameObject.GetComponent<Image>().sprite = 0_DASH;
                    break;
                case 1:
                    renderer.sprite = spriteArray[1];
                    //gameObject.GetComponent<Image>().sprite = 1_DASH;
                    break;
                case 2:
                    renderer.sprite = spriteArray[2];
                    //gameObject.GetComponent<Image>().sprite = 2_DASH;
                    break;
                case 3:
                    renderer.sprite = spriteArray[3];
                    //gameObject.GetComponent<Image>().sprite = 3_DASH;
                    break;
            }
        }
    }
}










