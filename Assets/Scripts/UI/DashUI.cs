using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{
    public Image image_switcher;
    public Sprite[] spriteArray; //manually filled array up with pictures of ui!

    void Start()
    {
        //using Image component instead of sprite renderer because it's for UI
        image_switcher = gameObject.GetComponent<Image>();

        //initially makes dashes full (make sure to actually update w real value!)
        updateDashUI(3);
    }

    void Update()
    {

    }

    public void updateDashUI(int numDashes)
    {
        switch(numDashes)
        {
            case 0:
                //no dashes
                image_switcher.sprite = spriteArray[0];
                break;
            case 1:
                //1 dash
                image_switcher.sprite = spriteArray[1];
                break;
            case 2:
                //2 dashes
                image_switcher.sprite = spriteArray[2];
                break;
            case 3:
                //dashes full
                image_switcher.sprite = spriteArray[3];
                break;
        }
    }
}
