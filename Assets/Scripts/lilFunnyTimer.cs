using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class lilFunnyTimer : MonoBehaviour
{
    long timer;
    public TMP_Text textTimer;

    int seconds, minutes;
    bool canCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(canCount == true)
        {
            return;
        }

        timer++;
        if(timer > 50)
        {
            seconds++;
            timer = 0;
        }

        if(seconds > 60)
        {
            minutes++;
            seconds = 0;
        }

        textTimer.text = minutes.ToString() + ":" + seconds.ToString() + "." + timer.ToString();
    }

    void OnTriggerEnter2D(Collider2D pl)
    {
        if(pl.tag == "Player")
        {
            canCount = true;
        }
    }
}
