using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyHealthManager : MonoBehaviour {
    public float maxHealth; // Set max health dynamically
    public float healthAmount;
    private float targetHealth; // Target health value for smooth animation
    public float sideBarSpeed = 2f; // Speed of the side bar animation

    public Canvas enemyHealth;
    private Image mainHealthBar;
    private Image sideHealthBar;

    public GameObject enemy;
    public Vector3 healthBarOffset = new Vector3(0, 2f, 0); // Adjust height above the enemy

    void Start()
    {
        mainHealthBar = enemyHealth.transform.Find("Health").GetComponent<Image>();
        sideHealthBar = enemyHealth.transform.Find("SideHealth").GetComponent<Image>();
        healthAmount = maxHealth;  // Initialize health
        targetHealth = healthAmount;
        UpdateHealthBar(); // Ensure the bar starts full

        // Ensure health bar canvas is in world space
        enemyHealth.renderMode = RenderMode.WorldSpace;
    }

    void Update()
    {
        // Make health bar follow the enemy in world space
        if (enemy != null)
        {
            enemyHealth.transform.position = enemy.transform.position + healthBarOffset;
        }

        // When health reaches 0
        if (healthAmount <= 0) {
             Destroy(enemy);  // Destroy the enemy object instead of just the health script
             Destroy(enemyHealth.gameObject); // Also remove the health bar
        }

        // Smoothly slide the side health bar down to match the main health bar
        if (sideHealthBar.fillAmount > mainHealthBar.fillAmount) {
            sideHealthBar.fillAmount = Mathf.Lerp(sideHealthBar.fillAmount, mainHealthBar.fillAmount, Time.deltaTime * sideBarSpeed);
        }
    }

    public void TakeDamage(float damage) {
        //Debug.Log("Enemy Damaged: " + damage);
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
