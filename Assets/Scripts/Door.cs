using UnityEngine;
using System.Collections;

// Attach to a door GameObject
// Opens by sliding upward (or you can change the openOffset direction)

public class Door : MonoBehaviour, IResettable
{
    // ── Inspector Variables ─────────────────────────────────────
    [Header("Movement")]
    public Vector3 openOffset = new Vector3(0f, 3f, 0f);  // How far door slides when open
    public float moveSpeed = 2f;                         // How fast it opens/closes

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    // ── Private State ─────────────────────────────────────────────
    private Vector3 closedPosition;   // Door's starting position
    private Vector3 openPosition;     // Door's open position
    private bool isOpen = false;
    private Coroutine moveCoroutine;  // Track the active movement so we can cancel it

    void Start()
    {
        // Remember where the door starts — this is the closed position
        closedPosition = transform.position;
        openPosition = closedPosition + openOffset;

        Debug.Log($"Door [{gameObject.name}]: Ready. Closed at {closedPosition}.");
    }

    // ── Public Methods ───────────────────────────────────────────

    public void Open()
    {
        if (isOpen) return; // Already open — do nothing

        isOpen = true;

        // Stop any in-progress movement before starting new one
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveDoor(openPosition));

        // Play open sound
        if (audioSource != null && openSound != null)
            audioSource.PlayOneShot(openSound);

        Debug.Log($"Door [{gameObject.name}]: Opening.");
    }

    public void Close()
    {
        if (!isOpen) return; // Already closed

        isOpen = false;

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveDoor(closedPosition));

        if (audioSource != null && closeSound != null)
            audioSource.PlayOneShot(closeSound);

        Debug.Log($"Door [{gameObject.name}]: Closing.");
    }

    // ── Private Methods ──────────────────────────────────────────

    IEnumerator MoveDoor(Vector3 targetPosition)
    {
        // Smoothly move the door toward the target position
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null; // Wait one frame before continuing
        }

        // Snap to exact position when close enough
        transform.position = targetPosition;
    }

    // ── IResettable ───────────────────────────────────────────────

    public void ResetObject()
    {
        // Stop any movement and snap door shut instantly
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);

        isOpen = false;
        transform.position = closedPosition;

        Debug.Log($"Door [{gameObject.name}]: Reset to closed.");
    }
}