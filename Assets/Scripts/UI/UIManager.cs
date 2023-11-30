using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Pool;

public class UIManager : MonoBehaviour
{
    public Image dashImage;
    public Image healthImage;
    public Sprite[] spriteArray; //manually filled array up with pictures of ui!
    public GameObject interactText;
    //public rectTransform tr;
    void Start()
    {
        //using Image component instead of sprite renderer because it's for UI

        //initially makes dashes full (make sure to actually update w real value!)
        updateDashUI();
        //GameManager.inst.playerHealth.ApplyDamage(9);
        updateHealthUI();
        
    }

    public void updateinteractUI(bool how)
    {
        interactText.SetActive(how);
    }
    
    public void updateHealthUI()
    {
        int health = GameManager.inst.playerHealth.currentHealth;
        int healthWidth = health*20; 
        //THIS COEFFICIENT IS DEPENDENT ON HOW MANY BITS OF DAMAGE YOU CAN TAKE!
        //good health-full width is 200,so 10(max-health)*20= width;
        //Debug.Log(healthWidth);
        //healthImage.sprite.rect.width = health;
        int healthHeight = 35;
        healthImage.rectTransform.sizeDelta = new Vector2(healthWidth,healthHeight);
        //tr.sizeDelta = new Vector2(healthImage.sprite.rect.healthWidth, healthImage.sprite.rect.healthHeight);     
    }

    public void updateDashUI()
    {
        int numDashes = GameManager.inst.playerMovement.dashesRemaining;
        switch(numDashes)
        {
            case 0:
                //no dashes
                dashImage.sprite = spriteArray[0];
                break;
            case 1:
                //1 dash
                dashImage.sprite = spriteArray[1];
                break;
            case 2:
                //2 dashes
                dashImage.sprite = spriteArray[2];
                break;
            case 3:
                //dashes full
                dashImage.sprite = spriteArray[3];
                break;
        }
    }
}
