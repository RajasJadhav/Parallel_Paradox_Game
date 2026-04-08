using UnityEngine;

// The see-saw tilts based on weight on each side
// Equal weight = flat bridge = crossable

public class SeeSaw : MonoBehaviour, IResettable
{
    [Header("References")]
    public Transform pivotPoint;        // The SeeSaw_Pivot empty GO
    public Transform leftWeightZone;    // Trigger on left side
    public Transform rightWeightZone;   // Trigger on right side

    [Header("Settings")]
    public float maxTiltAngle = 25f;   // How far it tilts each side
    public float tiltSpeed = 2f;    // How fast it tilts
    public float balanceThreshold = 0.5f; // How close to flat = "balanced"

    // Weight counts on each side
    private int leftWeight = 0;
    private int rightWeight = 0;

    // Target rotation based on weight difference
    private float targetAngle = 0f;

    void Update()
    {
        // Calculate tilt based on weight difference
        int weightDiff = leftWeight - rightWeight;

        if (weightDiff > 0)
            targetAngle = -maxTiltAngle;  // Left heavy = tilt left
        else if (weightDiff < 0)
            targetAngle = maxTiltAngle;   // Right heavy = tilt right
        else
            targetAngle = 0f;             // Balanced = flat

        // Smoothly rotate toward target angle
        float currentAngle = pivotPoint.localEulerAngles.z;

        // Convert to -180 to 180 range for proper lerping
        if (currentAngle > 180f) currentAngle -= 360f;

        float newAngle = Mathf.Lerp(currentAngle, targetAngle, tiltSpeed * Time.deltaTime);
        pivotPoint.localEulerAngles = new Vector3(0f, 0f, newAngle);
    }

    // Called by trigger zones on each side
    public void AddWeight(string side)
    {
        if (side == "left") leftWeight++;
        if (side == "right") rightWeight++;
        Debug.Log($"SeeSaw: Left={leftWeight} Right={rightWeight}");
    }

    public void RemoveWeight(string side)
    {
        if (side == "left") leftWeight = Mathf.Max(0, leftWeight - 1);
        if (side == "right") rightWeight = Mathf.Max(0, rightWeight - 1);
    }

    public bool IsBalanced()
    {
        float angle = pivotPoint.localEulerAngles.z;
        if (angle > 180f) angle -= 360f;
        return Mathf.Abs(angle) < balanceThreshold;
    }

    public void ResetObject()
    {
        leftWeight = 0;
        rightWeight = 0;
        pivotPoint.localEulerAngles = Vector3.zero;
        Debug.Log("SeeSaw: Reset.");
    }
}