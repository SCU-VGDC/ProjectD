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

    string timerString;

    // Start is called before the first frame update
    void Start()
    {
        canCount = false;
        minutes = 0;
        seconds = 0;
        timer = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(canCount == true)
        {
            return;
        }

        timer++;
        if(timer >= 50)
        {
            seconds++;
            timer = 0;
        }

        if(seconds >= 60)
        {
            minutes++;
            seconds = 0;
        }
        if (minutes > 0)
        {
            timerString = minutes.ToString() + ":";
        }
        else
        {
            timerString = "";
        }
        if (seconds < 10)
        {
            timerString += "0" + seconds.ToString();
        }
        else
        {
            timerString += seconds.ToString();
        }
        if (timer < 10)
        {
            timerString += ":0" + timer.ToString();
        }
        else
        {
            timerString += ":" + timer.ToString();
        }
        textTimer.text = timerString;
    }

    void OnTriggerEnter2D(Collider2D pl)
    {
        if(pl.tag == "Player")
        {
            canCount = true;
        }
    }
}
