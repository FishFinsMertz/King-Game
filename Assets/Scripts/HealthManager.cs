using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour
{
    public Image mainHealthBar;
    public Image sideHealthBar;

    public float healthAmount = 100f;
    private float targetHealth; // Target health value for smooth animation
    public float sideBarDelay = 0.5f; // Delay before side bar starts to slide
    public float sideBarSpeed = 2f; // Speed of the side bar animation

    void Start()
    {
        targetHealth = healthAmount;
    }

    void Update()
    {
        // When health reaches 0
        if (healthAmount <= 0) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // TESTING PURPOSES
        if (Input.GetKeyDown(KeyCode.Return)) {
            TakeDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.H)) {
            Heal(50);
        }

        // Smoothly slide the side health bar down to match the main health bar
        if (sideHealthBar.fillAmount > mainHealthBar.fillAmount) {
            sideHealthBar.fillAmount = Mathf.Lerp(sideHealthBar.fillAmount, mainHealthBar.fillAmount, Time.deltaTime * sideBarSpeed);
        }
    }

    public void TakeDamage(float damage) {
        healthAmount -= damage;
        targetHealth = healthAmount;
        mainHealthBar.fillAmount = targetHealth / 100f;

        // Delay before the side bar starts to slide down
        StartCoroutine(UpdateSideHealthBar());
    }

    public void Heal(float healingAmount) {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100f);
        targetHealth = healthAmount;
        mainHealthBar.fillAmount = targetHealth / 100f;
        sideHealthBar.fillAmount = mainHealthBar.fillAmount; // Heal instantly updates both
    }

    private IEnumerator UpdateSideHealthBar() {
        yield return new WaitForSeconds(sideBarDelay);
        while (sideHealthBar.fillAmount > mainHealthBar.fillAmount) {
            sideHealthBar.fillAmount = Mathf.Lerp(sideHealthBar.fillAmount, mainHealthBar.fillAmount, Time.deltaTime * sideBarSpeed);
            yield return null;
        }
    }
}
