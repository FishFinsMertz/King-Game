using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System;

public class PlayerHealthManager : MonoBehaviour
{
    private FlashFX flashScript;

    public Image mainHealthBar;
    public Image sideHealthBar;

    public float healthAmount = 100f;
    private float targetHealth; // Target health value for smooth animation
    public float sideBarSpeed = 2f; // Speed of the side bar animation

    public GameObject player;

    void Start()
    {
        targetHealth = healthAmount;
        player = GameObject.FindWithTag("Player");
        flashScript = player.GetComponentInChildren<FlashFX>();
    }

    void Update()
    {
        // When health reaches 0
        if (healthAmount <= 0)
        {
            Death();
        }

        // TESTING PURPOSES
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(50);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TakeDamage(30f, Vector2.up, 20f);
        }

        // Smoothly slide the side health bar down to match the main health bar
        if (sideHealthBar.fillAmount > mainHealthBar.fillAmount)
        {
            sideHealthBar.fillAmount = Mathf.Lerp(sideHealthBar.fillAmount, mainHealthBar.fillAmount, Time.deltaTime * sideBarSpeed);
        }
    }

    public void TakeDamage(float damage, Vector2 hitDirection, float knockbackForce)
    {
        //Debug.Log("Player Damaged: " + damage);
        healthAmount -= damage;
        targetHealth = healthAmount;
        mainHealthBar.fillAmount = targetHealth / 100f;

        flashScript.Flash();

        // Apply Knockback
        if (healthAmount > 0)
        {
            player.GetComponent<PlayerController>().ChangeState(new PlayerKnockbackState(
                player.GetComponent<PlayerController>(),
                hitDirection.normalized * knockbackForce
            ));
        }
    }

    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100f);
        targetHealth = healthAmount;
        mainHealthBar.fillAmount = targetHealth / 100f;
        sideHealthBar.fillAmount = mainHealthBar.fillAmount; // Heal instantly updates both
    }

    public void Death()
    {
        // Temporary death, change later
        StartCoroutine(Die());
    }
    IEnumerator Die() {
        yield return new WaitForSecondsRealtime(0.6f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
