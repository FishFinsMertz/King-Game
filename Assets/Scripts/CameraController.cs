using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Player;
    public float smoothing; 
    public Vector3 offset = new Vector3(0, 0, -10);
    private Vector3 velocity = Vector3.zero; // Used by SmoothDamp for smoothing

    void LateUpdate()
    {
        Vector3 targetPosition = Player.position + offset;

        // Smoothly move the camera to the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothing);
    }
}
