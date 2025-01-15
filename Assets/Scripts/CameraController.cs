using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Player;
    private float smoothing = 0.125f;
    private Vector3 offset = new Vector3(0, 0, -10);
    private Vector3 velocity = Vector3.zero;

    private Vector3 shakeOffset = Vector3.zero;
    private bool isShaking = false;

    // Enum for shake levels
    public enum ShakeLevel
    {
        light,
        medium,
        heavy
    }

    // Dictionary to map shake levels to magnitude and duration
    private Dictionary<ShakeLevel, (float magnitude, float duration)> shakeSettings = new Dictionary<ShakeLevel, (float, float)>()
    {
        { ShakeLevel.light, (0.07f, 0.1f) },   // Light shake
        { ShakeLevel.medium, (0.15f, 0.15f) },  // Medium shake
        { ShakeLevel.heavy, (0.2f, 0.2f) }     // Strong shake
    };

    void LateUpdate() {
        // Calculate target position based on player position
        Vector3 targetPosition = Player.position + offset;

        // Smoothly move the camera to the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothing);

        // Apply shake offset directly after smooth following
        transform.position += shakeOffset;
    }

    // Shake based on strength
    public void StartShake(ShakeLevel shakeLevel) {
        if (!isShaking)
        {
            var shakeData = shakeSettings[shakeLevel];
            StartCoroutine(Shake(shakeData.magnitude, shakeData.duration));
        }
    }

    private IEnumerator Shake(float magnitude, float duration) {
        isShaking = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float offsetX = Random.Range(-0.5f, 0.5f) * magnitude;
            float offsetY = Random.Range(-0.5f, 0.5f) * magnitude;

            shakeOffset = new Vector3(offsetX, offsetY, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        isShaking = false;
    }

    // FOR TESTING PURPOSES
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartShake(ShakeLevel.light); // Small shake
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartShake(ShakeLevel.medium); // Medium shake
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartShake(ShakeLevel.heavy); // Strong shake
        }
    }
}

