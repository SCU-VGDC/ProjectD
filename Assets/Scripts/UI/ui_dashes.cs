using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Backend;

namespace Player_Movement_Namespace
{
    public class ui_dashes: MonoBehaviour
    {
        public GameObject player;
        public Image image_switcher;
        public Sprite[] spriteArray; //manually filled array up with pictures of ui
        private PersistentData pd;

        void Start()
        {
            //using Image component instead of sprite renderer because it's for UI
            image_switcher = gameObject.GetComponent<Image>();
            pd = PersistentDataManager.inst.persistentData;
        }
        
        private void Update() {
            showDashes();
        }
        void showDashes()
        {
            switch(GameManager.inst.playerMovement.currentDashes)
            {
                case 0:
                    //no dashes
                    image_switcher.GetComponent<Image>().sprite = spriteArray[0];
                    break;
                case 1:
                    //1 dash
                    image_switcher.GetComponent<Image>().sprite = spriteArray[1];
                    break;
                case 2:
                    //2 dashes
                    image_switcher.GetComponent<Image>().sprite = spriteArray[2];
                    break;
                case 3:
                    //dashes full
                    image_switcher.GetComponent<Image>().sprite = spriteArray[3];
                    break;
            }
        }
    }
}










