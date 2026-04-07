using UnityEngine;

// Attach to a platform that moves between two points
// Only moves while isActive is true
// isActive is set by HoldButton.cs

public class MovingPlatform : MonoBehaviour, IResettable
{
    // ── Inspector Variables ─────────────────────────────────────
    [Header("Movement Points")]
    public Transform pointA;         // Starting position (drag an empty GameObject here)
    public Transform pointB;         // Ending position   (drag an empty GameObject here)
    public float moveSpeed = 2f; // How fast the platform moves

    [Header("State")]
    public bool isActive = false;    // Controlled by HoldButton.cs

    // ── Private State ────────────────────────────────────────────
    private Vector3 targetPosition;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError($"MovingPlatform [{gameObject.name}]: Point A or B not assigned!");
            return;
        }

        // Start moving toward Point B
        targetPosition = pointB.position;
        transform.position = pointA.position;

        Debug.Log($"MovingPlatform [{gameObject.name}]: Ready.");
    }

    void Update()
    {
        // Only move if a button is being held
        if (!isActive) return;

        // Move toward the current target
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // When we reach the target, switch direction
        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            // Toggle between A and B
            targetPosition = targetPosition == pointB.position
                ? pointA.position
                : pointB.position;
        }
    }

    // ── IResettable ───────────────────────────────────────────────

    public void ResetObject()
    {
        isActive = false;
        targetPosition = pointB.position;
        transform.position = pointA.position;

        Debug.Log($"MovingPlatform [{gameObject.name}]: Reset.");
    }
}