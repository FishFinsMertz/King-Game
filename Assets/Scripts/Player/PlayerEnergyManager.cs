using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayerEnergyManager : MonoBehaviour
{
    public Image mainEnergyBar;
    public Image sideEnergyBar;

    public float energyAmount = 100f;
    private float targetEnergy;
    public float sideBarSpeed = 2f; 

    public GameObject player;

    void Start()
    {
        energyAmount = 0;
        mainEnergyBar.fillAmount = 0;
        sideEnergyBar.fillAmount = 0;
        targetEnergy = energyAmount;
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        // When health reaches 0 (hard cap at 0 energy just in case)
        if (energyAmount <= 0) {
            energyAmount = 0;
        }

        // TESTING PURPOSES
        if (Input.GetKeyDown(KeyCode.G)) {
            ChargeEnergy(50);
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            DecreaseEnergy(50); 
        }

        // Smoothly slide the side health bar down to match the main health bar
        if (sideEnergyBar.fillAmount > mainEnergyBar.fillAmount) {
            sideEnergyBar.fillAmount = Mathf.Lerp(sideEnergyBar.fillAmount, mainEnergyBar.fillAmount, Time.deltaTime * sideBarSpeed);
        }
    }

    public void DecreaseEnergy(float power) {
        energyAmount -= power;
        targetEnergy = energyAmount;
        mainEnergyBar.fillAmount = targetEnergy / 100f;
    }

    public void ChargeEnergy(float chargeAmount) {
        energyAmount += chargeAmount;
        energyAmount = Mathf.Clamp(energyAmount, 0, 100f);
        targetEnergy = energyAmount;
        mainEnergyBar.fillAmount = targetEnergy / 100f;
        sideEnergyBar.fillAmount = mainEnergyBar.fillAmount; // Heal instantly updates both
    }
}
