using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaManager : MonoBehaviour
{
    public Canvas staminaBarPrefab;
    private Canvas staminaBar;
    private Image mainStaminaBar;
    private Image sideStaminaBar;

    public float staminaAmount = 100f;
    private float targetStamina; // Target stamina value for smooth animation
    public float sideBarSpeed; // Speed of the side bar animation
    public float recoverySpeed; // Speed at which stamina recovers

    [HideInInspector] public GameObject player;

    void Start()
    {
        // Instantiate the health bar prefab and keep a reference
        staminaBar = Instantiate(staminaBarPrefab);

        // Get health bar images from the instance
        mainStaminaBar = staminaBar.transform.Find("Stamina").GetComponent<Image>();
        sideStaminaBar = staminaBar.transform.Find("SideStamina").GetComponent<Image>();

        targetStamina = staminaAmount;
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (staminaBar != null)
        {
            // Hard cap for negative stamina
            if (staminaAmount <= -30)
            {
                staminaAmount = -30;
            }

            // Stamina recovery
            refillStamina();

            // Lerp the side stamina bar smoothly to match the main stamina bar
            sideStaminaLerp();
        }
    }

    public void DecreaseStamina(float amount)
    {
        staminaAmount -= amount;
        targetStamina = staminaAmount;
        mainStaminaBar.fillAmount = staminaAmount / 100f;
    }

    private void sideStaminaLerp()
    {
        // Smoothly slide the side health bar down to match the main health bar
        if (sideStaminaBar.fillAmount != mainStaminaBar.fillAmount)
        {
            sideStaminaBar.fillAmount = Mathf.Lerp(sideStaminaBar.fillAmount, mainStaminaBar.fillAmount, Time.deltaTime * sideBarSpeed);
        }
    }

    private void refillStamina()
    {
        // Gradually refill stamina (always regenerating)
        if (staminaAmount < 100f)
        {
            staminaAmount += recoverySpeed * Time.deltaTime;
            staminaAmount = Mathf.Min(staminaAmount, 100f); // Clamp to max of 100

            targetStamina = staminaAmount;
            mainStaminaBar.fillAmount = staminaAmount / 100f;
        }
    }
}
