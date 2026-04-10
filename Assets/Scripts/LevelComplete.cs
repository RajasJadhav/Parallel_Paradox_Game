using UnityEngine;
using UnityEngine.SceneManagement;

// Place a trigger zone at the level exit
// When the player walks through, the level is marked complete

public class LevelComplete : MonoBehaviour
{
    // ── Inspector Variables ─────────────────────────────────────
    [Header("References")]
    public LevelManager levelManager;

    [Header("Settings")]
    public string nextSceneName = "Level2"; // Name of the next scene to load

    // ── State ────────────────────────────────────────────────────
    private bool hasCompleted = false; // Prevent triggering twice

    void OnTriggerEnter(Collider other)
    {
        // Only the player can complete the level — not a ghost
        if (hasCompleted) return;

        if (other.CompareTag("Player"))
        {
            hasCompleted = true;

            Debug.Log($"LevelComplete: Player reached the exit! Loading {nextSceneName}.");

            levelManager?.LevelComplete();

            SceneManager.LoadScene(nextSceneName); // ← Replace the TODO with this

            // TODO: Uncomment when SceneLoader is ready
            // SceneLoader.Instance.LoadScene(nextSceneName);
        }
    }
}