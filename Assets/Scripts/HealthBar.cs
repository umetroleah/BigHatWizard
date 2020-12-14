using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    [SerializeField] private GameObject healthBarUI;


    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        SetHealth(maxHealth);
    }


    public void SetHealth(float health)
    {
        slider.value = health;
    }

    public void SetActive(bool active)
    {
        healthBarUI.SetActive(active);
    }
}
