using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Player;
    public Transform Boss;
    public float smoothing = 0.25f; 
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

    // Camera borders
    [Header("Camera Borders")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    // Shake settings
    public enum ShakeLevel { light, medium, heavy }

    private Dictionary<ShakeLevel, (float magnitude, float duration)> shakeSettings =
        new Dictionary<ShakeLevel, (float, float)>()
    {
        { ShakeLevel.light, (0.07f, 0.15f) },
        { ShakeLevel.medium, (0.15f, 0.4f) },
        { ShakeLevel.heavy, (0.2f, 0.6f) }
    };

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector3 targetPosition;

        // --- Calculate target zoom ---
        float targetZoom = minZoom;
        if (Boss != null)
        {
            targetPosition = (Player.position + Boss.position) / 2f + offset;

            float distance = Vector3.Distance(Player.position, Boss.position);
            targetZoom = Mathf.Clamp(baseZoom + (distance * zoomFactor), minZoom, maxZoom);
        }
        else
        {
            targetPosition = Player.position + offset;
            targetZoom = minZoom;
        }

        // --- Predict next zoom value (before clamping) ---
        float nextZoom = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSmoothSpeed * Time.deltaTime);

        // --- Calculate camera bounds with that zoom ---
        float camHeight = nextZoom;
        float camWidth = camHeight * cam.aspect;

        // --- Smoothly move camera ---
        Vector3 newPos = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothing);

        // --- Clamp to borders ---
        newPos.x = Mathf.Clamp(newPos.x, minX + camWidth, maxX - camWidth);
        newPos.y = Mathf.Clamp(newPos.y, minY + camHeight, maxY - camHeight);

        // --- Apply position & shake ---
        transform.position = newPos + shakeOffset;

        // --- Apply zoom AFTER clamping ---
        cam.orthographicSize = nextZoom;
    }


    // Shake
    public void StartShake(ShakeLevel shakeLevel)
    {
        if (!isShaking)
        {
            var shakeData = shakeSettings[shakeLevel];
            StartCoroutine(Shake(shakeData.magnitude, shakeData.duration));
        }
    }

    private IEnumerator Shake(float magnitude, float duration)
    {
        isShaking = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float decay = 1f - (elapsedTime / duration);
            float currentMagnitude = magnitude * decay;

            float offsetX = Random.Range(-1f, 1f) * currentMagnitude;
            float offsetY = Random.Range(-1f, 1f) * currentMagnitude;

            shakeOffset = new Vector3(offsetX, offsetY, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        isShaking = false;
    }

    // Draw borders in scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 bottomLeft = new Vector3(minX, minY, 0);
        Vector3 bottomRight = new Vector3(maxX, minY, 0);
        Vector3 topLeft = new Vector3(minX, maxY, 0);
        Vector3 topRight = new Vector3(maxX, maxY, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}
