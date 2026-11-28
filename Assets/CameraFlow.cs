using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Drag your Circle Player here")]
    public Transform target;

    [Tooltip("How smoothly the camera catches up (0 = instant lock, 0.15 = smooth)")]
    public float smoothTime = 0.15f;

    // Keeps the camera at its standard 2D viewing distance (-10 on Z)
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private Vector3 velocity = Vector3.zero;

    // LateUpdate runs AFTER the player finishes moving each frame, preventing jitter
    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
