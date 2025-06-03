using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Player;
    public Transform Boss;
    public float smoothing = 0.25f; //125 originally
    private Vector3 offset = new Vector3(0, 1, -10);
    private Vector3 velocity = Vector3.zero;
    private Vector3 shakeOffset = Vector3.zero;
    private bool isShaking = false;

    private Camera cam;

    // Zoom settings
    [Header("Zoom Settings")]
    public float baseZoom;
    public float zoomFactor;
    public float minZoom;
    public float maxZoom;
    public float zoomSmoothSpeed;

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

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector3 targetPosition;

        // Determine the target position
        if (Boss != null)
        {
            targetPosition = (Player.position + Boss.position + offset) / 2f;

            // Adjust zoom based on distance to Boss
            float distance = Vector3.Distance(Player.position, Boss.position);
            float targetZoom = Mathf.Clamp(baseZoom + (distance * zoomFactor), minZoom, maxZoom);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSmoothSpeed * Time.deltaTime);
        }
        else
        {
            targetPosition = Player.position + offset;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, minZoom, zoomSmoothSpeed * Time.deltaTime);
        }

        // Smoothly move the camera to the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothing);

        // Apply shake offset after smoothing
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

