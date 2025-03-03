using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class EnemyHealthManager : MonoBehaviour {
    public Image mainHealthBar;
    public Image sideHealthBar;

    public float maxHealth; // Set max health dynamically
    public float healthAmount;
    private float targetHealth; // Target health value for smooth animation
    public float sideBarSpeed = 2f; // Speed of the side bar animation

    public GameObject enemy;

    void Start()
    {
        healthAmount = maxHealth;  // Initialize health
        targetHealth = healthAmount;
        UpdateHealthBar(); // Ensure the bar starts full
    }

    void Update()
    {
        // When health reaches 0
        if (healthAmount <= 0) {
             Destroy(gameObject);
        }

        // TESTING PURPOSES
        if (Input.GetKeyDown(KeyCode.Return)) {
            TakeDamage(50);
        }

        // Smoothly slide the side health bar down to match the main health bar
        if (sideHealthBar.fillAmount > mainHealthBar.fillAmount) {
            sideHealthBar.fillAmount = Mathf.Lerp(sideHealthBar.fillAmount, mainHealthBar.fillAmount, Time.deltaTime * sideBarSpeed);
        }
    }

    public void TakeDamage(float damage) {
        healthAmount -= damage;
        targetHealth = Mathf.Clamp(healthAmount, 0, maxHealth);
        UpdateHealthBar();
    }

    public void Heal(float healingAmount) {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);
        targetHealth = healthAmount;
        UpdateHealthBar();
    }

    private void UpdateHealthBar() {
        mainHealthBar.fillAmount = targetHealth / maxHealth;
    }
}


