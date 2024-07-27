using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject canvas;

    public Image dashImage;
    public Image healthImage;
    public Image gunImage;
    public Sprite[] dashSpriteArray; //manually filled array up with pictures of ui!
    public int gunSetUnlocked;
    public SpriteArray[] gunSpriteArray;
    public GameObject interactText;

    public GameObject pauseMenu;
    public bool pauseMenuIsOpen => pauseMenu.activeInHierarchy;
    //public rectTransform tr;
    void Start()
    {
        //using Image component instead of sprite renderer because it's for UI

        //initially makes dashes full (make sure to actually update w real value!)
        updateDashUI();
        //GameManager.inst.playerHealth.ApplyDamage(9);
        updateHealthUI();
        
    }

    void Update()
    {
        if (SceneManager.sceneCount == 1 && Input.GetButtonDown("Pause"))
        {
            if (!pauseMenuIsOpen) OpenPauseMenu();
            else ClosePauseMenu();
        }
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
                dashImage.sprite = dashSpriteArray[0];
                break;
            case 1:
                //1 dash
                dashImage.sprite = dashSpriteArray[1];
                break;
            case 2:
                //2 dashes
                dashImage.sprite = dashSpriteArray[2];
                break;
            case 3:
                //dashes full
                dashImage.sprite = dashSpriteArray[3];
                break;
        }
    }

    public void updateGunUI(int gunType) // 0 - pistol, 1 - shotgun, 2 - sniper
    {
        Debug.Log(gunSetUnlocked);
        // if gun has not been unlocked yet
        if (gunSetUnlocked == 0 && gunType != 0) {
            return;
        }
        if (gunSetUnlocked == 1 && gunType > 1) {
            return;
        }

        gunImage.sprite = gunSpriteArray[gunSetUnlocked].Imgs[gunType];
    }

    // Pause Menu:
    /// <summary>
    ///     Enables and disables all HUD elements in the Canvas
    /// </summary>
    /// <param name="value"></param>
    private void SetHUDActive(bool value)
    {
        foreach (Transform child in canvas.transform)
        {
            if (child.tag == "HUD")
            {
                child.gameObject.SetActive(value);
            }
        }
    }

    public void OpenPauseMenu()
    {
        SetHUDActive(false);

        Time.timeScale = 0; // freezes game

        pauseMenu.SetActive(true);
    }

    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);

        Time.timeScale = 1; // resumes game

        SetHUDActive(true);
    }

    public void GoToSettings()
    {
        SceneManager.LoadScene("Title Screen", LoadSceneMode.Additive);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Title Screen");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

// used to get 2D arrays in the inspector
[System.Serializable]
public struct SpriteArray
{
    [SerializeField] public Sprite[] Imgs;
}