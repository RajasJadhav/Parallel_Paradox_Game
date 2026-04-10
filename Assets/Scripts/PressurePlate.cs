using UnityEngine;

// Attach to a pressure plate GameObject with a Box Collider set to Is Trigger
// Works with BOTH the player and ghost clones
// Supports requiring multiple activators (for Level 2's two-switch puzzle)

public class PressurePlate : MonoBehaviour, IResettable
{
    // ── Inspector Variables ─────────────────────────────────────
    [Header("Settings")]
    public int requiredActivators = 1;    // How many bodies needed to activate
    public Door linkedDoor;               // Which door does this control?

    [Header("Visual Feedback")]
    public Renderer plateRenderer;        // The plate's visual mesh
    public Color plateColor = new Color(223f / 255f, 192f / 255f, 137f / 255f);

    // ── State ────────────────────────────────────────────────────
    private int currentActivators = 0;    // How many are standing on it right now
    private bool isActive = false;

    void Start()
    {
        UpdateVisual();
        Debug.Log($"PressurePlate [{gameObject.name}]: Ready. Requires {requiredActivators} activator(s).");
    }

    // ── Trigger Events ────────────────────────────────────────────

    void OnTriggerEnter(Collider other)
    {
        // Accept both Player and Ghost tags
        if (other.CompareTag("Player") || other.CompareTag("Ghost"))
        {
            currentActivators++;
            Debug.Log($"PressurePlate [{gameObject.name}]: {other.tag} stepped on. Count: {currentActivators}/{requiredActivators}");

            CheckActivation();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ghost"))
        {
            currentActivators--;
            currentActivators = Mathf.Max(0, currentActivators); // Never go below 0
            Debug.Log($"PressurePlate [{gameObject.name}]: {other.tag} stepped off. Count: {currentActivators}/{requiredActivators}");

            CheckActivation();
        }
    }

    // ── Private Methods ──────────────────────────────────────────

    void CheckActivation()
    {
        bool shouldBeActive = currentActivators >= requiredActivators;

        if (shouldBeActive && !isActive)
        {
            Activate();
        }
        else if (!shouldBeActive && isActive)
        {
            Deactivate();
        }
    }

    void Activate()
    {
        isActive = true;
        linkedDoor?.Open();   // The ?. means "only call if linkedDoor is not null"
        UpdateVisual();
        Debug.Log($"PressurePlate [{gameObject.name}]: ACTIVATED.");
    }

    void Deactivate()
    {
        isActive = false;
        linkedDoor?.Close();
        UpdateVisual();
        Debug.Log($"PressurePlate [{gameObject.name}]: DEACTIVATED.");
    }

    void UpdateVisual()
    {
        if (plateRenderer != null)
            plateRenderer.material.color = plateColor;
    }

    // ── IResettable ───────────────────────────────────────────────

    public void ResetObject()
    {
        // Called by LevelManager at the start of every loop
        currentActivators = 0;
        isActive = false;
        UpdateVisual();

        Debug.Log($"PressurePlate [{gameObject.name}]: Reset.");
    }

    // Add this method to PressurePlate.cs
    public bool IsActive()
    {
        return isActive;
    }
}