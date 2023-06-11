using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Image HealthBarImage;
    public float CurrentHealth;
    private float MaxHealth;
    PlayerManager player;

    private void Start()
    {
        HealthBarImage = GetComponent<Image>();
        player = FindObjectOfType<PlayerManager>();
    }

    private void Update()
    {
        CurrentHealth = player.health;
        HealthBarImage.fillAmount = CurrentHealth / MaxHealth;
    }
}
