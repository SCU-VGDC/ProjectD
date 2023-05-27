using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Backend;

public class HealthBar : MonoBehaviour
{
    public slider slider;

    public void setMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void setHealth(int health)
    {
        slider.value = health;
    }
}
