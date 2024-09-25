using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;   // The player that the camera will follow
    public Vector2 minBoundary;  // The minimum X and Y coordinates the camera can move to
    public Vector2 maxBoundary;  // The maximum X and Y coordinates the camera can move to
    public float smoothSpeed = 0.125f; // Smoothing factor for camera movement
    

    private Vector3 offset; // Offset from player to camera
    private bool isPlayerDestroyed = false; // Flag to check if player is destroyed

    void Start()
    {
        // If the player is assigned, calculate the initial offset
        if (player != null)
        {
            offset = transform.position - player.position;
        }
    }

    void LateUpdate()
    {
        // If the player has been destroyed, do nothing
        if (isPlayerDestroyed || player == null)
        {
            return; // Exit if no player is left to follow
        }

        // Calculate the target position of the camera
        Vector3 targetPosition = player.position + offset;

        // Clamp the camera's position to the defined boundaries
        float clampedX = Mathf.Clamp(targetPosition.x, minBoundary.x, maxBoundary.x);
        float clampedY = Mathf.Clamp(targetPosition.y, minBoundary.y, maxBoundary.y);

        // Set the new clamped position for the camera
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, targetPosition.z);

        // Smoothly move the camera to the clamped position
        transform.position = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);
    }

    public void PlayerDestroyed()
    {
        // Called when the player is destroyed
        isPlayerDestroyed = true;
    }
}