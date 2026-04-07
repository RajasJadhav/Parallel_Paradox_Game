using UnityEngine;

// The trap in Level 4
// Destroys any ghost that touches it
// Disables itself permanently after first trigger
// The player can walk through safely after

public class SacrificeTrap : MonoBehaviour, IResettable
{
    // ── Inspector Variables ─────────────────────────────────────
    [Header("References")]
    public AudioSource audioSource;
    public AudioClip staticCrackleSound;   // The film static sound on ghost death

    [Header("Settings")]
    public bool resetBetweenLoops = false;   // Usually false — sacrifice is permanent

    // ── State ────────────────────────────────────────────────────
    private bool hasTriggered = false;       // Once triggered, trap is done
    private Collider trapCollider;

    // ── Events ───────────────────────────────────────────────────
    // CameraEffects and NarratorManager listen to this
    public System.Action OnGhostSacrificed;

    void Start()
    {
        trapCollider = GetComponent<Collider>();

        Debug.Log($"SacrificeTrap [{gameObject.name}]: Ready and armed.");
    }

    void OnTriggerEnter(Collider other)
    {
        // Only triggers once and only for ghosts — player is safe
        if (hasTriggered) return;

        if (other.CompareTag("Ghost"))
        {
            Debug.Log($"SacrificeTrap: Ghost '{other.gameObject.name}' sacrificed.");

            // Fire the sacrifice event BEFORE destroying
            // so listeners (CameraEffects etc.) can react
            OnGhostSacrificed?.Invoke();

            // Play the static crackle sound
            if (audioSource != null && staticCrackleSound != null)
                audioSource.PlayOneShot(staticCrackleSound);

            // Destroy the ghost GameObject
            Destroy(other.gameObject);

            // Disable the trap — it has done its job
            hasTriggered = true;
            trapCollider.enabled = false;

            Debug.Log("SacrificeTrap: Trap disabled. Path is now clear.");
        }
    }

    // ── IResettable ───────────────────────────────────────────────

    public void ResetObject()
    {
        // Only re-arm the trap if the designer wants it to reset
        if (resetBetweenLoops)
        {
            hasTriggered = false;
            trapCollider.enabled = true;
            Debug.Log($"SacrificeTrap [{gameObject.name}]: Re-armed.");
        }
        else
        {
            Debug.Log($"SacrificeTrap [{gameObject.name}]: Permanent — not re-armed.");
        }
    }
}