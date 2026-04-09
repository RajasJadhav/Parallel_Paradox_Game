using UnityEngine;
using System.Collections.Generic;

// Attach to SeeSaw_Plank
// Moves riders WITH the platform without parenting them
// Parenting causes scale distortion — this avoids that entirely

public class PlatformRider : MonoBehaviour , IResettable 
{
    // Everyone currently standing on this platform
    private List<Transform> riders = new List<Transform>();

    // Platform position and rotation from LAST frame
    // Used to calculate how much the platform moved
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void LateUpdate()
    {
        // LateUpdate runs AFTER all physics and movement
        // So the platform has already moved this frame
        // We calculate the delta and push all riders by that delta

        // ── Position delta ───────────────────────────────────────
        Vector3 positionDelta = transform.position - lastPosition;

        // ── Rotation delta ───────────────────────────────────────
        // How much did the platform rotate this frame?
        Quaternion rotationDelta = transform.rotation * Quaternion.Inverse(lastRotation);

        // Apply delta to every rider
        foreach (Transform rider in riders)
        {
            if (rider == null) continue;

            // Move rider by the same amount platform moved
            rider.position += positionDelta;

            // Rotate rider AROUND the platform's pivot point
            // This keeps them on the surface as it tilts
            rider.position = RotatePointAround(
                rider.position,
                transform.position,   // Rotate around platform center
                rotationDelta
            );
        }

        // Store this frame's values for next frame
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    // Rotates a point around a pivot by a given rotation delta
    Vector3 RotatePointAround(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        // Move point to be relative to pivot
        Vector3 direction = point - pivot;

        // Apply the rotation delta
        direction = rotation * direction;

        // Move back to world space
        return pivot + direction;
    }

    // ── Trigger Detection ─────────────────────────────────────────

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ghost"))
        {
            if (!riders.Contains(other.transform))
            {
                riders.Add(other.transform);
                Debug.Log($"PlatformRider: {other.tag} mounted. Riders: {riders.Count}");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ghost"))
        {
            riders.Remove(other.transform);
            Debug.Log($"PlatformRider: {other.tag} dismounted. Riders: {riders.Count}");
        }
    }

    // Called by LevelManager on loop reset
    public void ClearRiders()
    {
        riders.Clear();
        Debug.Log("PlatformRider: All riders cleared.");
    }

    // Add this method at the bottom:
    public void ResetObject()
    {
        riders.Clear();
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        Debug.Log("PlatformRider: Reset.");
    }
}