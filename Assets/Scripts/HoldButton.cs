using UnityEngine;

// The button that must be HELD to keep the platform moving
// Works with both player and ghost (both have colliders)

public class HoldButton : MonoBehaviour, IResettable
{
    // ── Inspector Variables ─────────────────────────────────────
    [Header("References")]
    public MovingPlatform linkedPlatform;  // Which platform does this control?

    [Header("Visual")]
    public Renderer buttonRenderer;
    public Color heldColor = Color.green;
    public Color releasedColor = Color.red;

    // ── Private State ────────────────────────────────────────────
    private int holdersCount = 0; // How many things are standing on it

    void Start()
    {
        UpdateVisual();
        Debug.Log($"HoldButton [{gameObject.name}]: Ready.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ghost"))
        {
            holdersCount++;
            UpdatePlatform();
            UpdateVisual();
            Debug.Log($"HoldButton [{gameObject.name}]: Held by {other.tag}. Total holders: {holdersCount}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ghost"))
        {
            holdersCount = Mathf.Max(0, holdersCount - 1);
            UpdatePlatform();
            UpdateVisual();
            Debug.Log($"HoldButton [{gameObject.name}]: Released by {other.tag}. Total holders: {holdersCount}");
        }
    }

    void UpdatePlatform()
    {
        if (linkedPlatform != null)
            linkedPlatform.isActive = holdersCount > 0;
    }

    void UpdateVisual()
    {
        if (buttonRenderer != null)
            buttonRenderer.material.color = holdersCount > 0 ? heldColor : releasedColor;
    }

    // ── IResettable ───────────────────────────────────────────────

    public void ResetObject()
    {
        holdersCount = 0;
        UpdatePlatform();
        UpdateVisual();

        Debug.Log($"HoldButton [{gameObject.name}]: Reset.");
    }
}