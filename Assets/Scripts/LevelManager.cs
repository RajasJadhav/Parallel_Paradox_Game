using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The brain of the game. Attach this to an empty GameObject called "LevelManager".
// It controls the entire loop lifecycle:
// Record → End Loop → Spawn Ghost → Reset Everything → Repeat

public class LevelManager : MonoBehaviour
{
    // ── Inspector Variables ─────────────────────────────────────
    [Header("References")]
    public TimelineRecorder playerRecorder;  // Drag Player here
    public Transform spawnPoint;      // Where player and ghosts start
    public GameObject ghostPrefab;     // Drag Ghost Prefab here

    [Header("Loop Settings")]
    public KeyCode endLoopKey = KeyCode.E;   // Player presses this to manually end a loop
    public float slowMoDuration = 0.5f;    // How long the slow-mo lasts on loop end
    public float slowMoTimeScale = 0.3f;   // How slow time gets (0.3 = 30% speed)

    // ── State ────────────────────────────────────────────────────
    private List<GhostReplay> activeGhosts = new List<GhostReplay>();
    private int loopNumber = 1;
    private bool loopActive = false;

    // ── Events ───────────────────────────────────────────────────
    // Other scripts (ViewfinderHUD) listen to these
    public System.Action<int> OnLoopStarted;   // Passes loop number
    public System.Action OnLoopEnded;
    public System.Action<int> OnGhostSpawned;  // Passes ghost count

    void Start()
    {
        // Subscribe to the recorder's timeout event
        playerRecorder.OnRecordingTimedOut += EndLoop;

        // Begin Loop 1 immediately
        StartLoop();

        Debug.Log("LevelManager: Game started. Loop 1 beginning.");
    }

    void Update()
    {
        // Player can manually end the loop by pressing E
        if (loopActive && Input.GetKeyDown(endLoopKey))
        {
            Debug.Log("LevelManager: Player manually ended the loop.");
            EndLoop();
        }
    }

    // ── Loop Lifecycle ───────────────────────────────────────────

    void StartLoop()
    {
        loopActive = true;

        // Reset player position
        playerRecorder.transform.position = spawnPoint.position;
        playerRecorder.transform.rotation = spawnPoint.rotation;

        // ❌ Don't call playerRecorder.StartRecording() here anymore
        // ✅ Player will press R when they're ready

        foreach (GhostReplay ghost in activeGhosts)
            ghost.StartReplay();

        OnLoopStarted?.Invoke(loopNumber);

        Debug.Log($"LevelManager: Loop {loopNumber} ready. Press R to start recording.");
    }

    public void EndLoop()
    {
        if (!loopActive) return;

        loopActive = false;

        // Stop recording the player
        playerRecorder.StopRecording();

        // Tell the HUD the loop ended (triggers white flash etc.)
        OnLoopEnded?.Invoke();

        // Start the slow-mo + spawn sequence
        StartCoroutine(EndLoopSequence());
    }

    IEnumerator EndLoopSequence()
    {
        // Step 1: Slow motion effect
        Time.timeScale = slowMoTimeScale;
        yield return new WaitForSecondsRealtime(slowMoDuration);
        Time.timeScale = 1f;

        // Step 2: Spawn a new ghost using what the player just recorded
        SpawnGhost();

        // Step 3: Reset all puzzle objects (doors, buttons back to initial state)
        ResetLevelObjects();

        // Step 4: Start the next loop
        loopNumber++;
        StartLoop();
    }

    void SpawnGhost()
    {
        // Create a new ghost at the spawn point
        GameObject newGhostObject = Instantiate(
            ghostPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        // Give the ghost the frames we just recorded
        GhostReplay newGhost = newGhostObject.GetComponent<GhostReplay>();
        newGhost.SetFrames(playerRecorder.GetRecordedFrames());

        // Add it to our list of all active ghosts
        activeGhosts.Add(newGhost);

        // Tell the HUD how many ghosts there are now
        OnGhostSpawned?.Invoke(activeGhosts.Count);

        Debug.Log($"LevelManager: Ghost {activeGhosts.Count} spawned with {playerRecorder.GetRecordedFrames().Count} frames.");
    }
    
// In LevelManager.cs
// Find your existing ResetLevelObjects() method
// Add this at the TOP of that method:

void ResetLevelObjects()
{
    // ── Frozen clones persist between loops — DO NOT clear here ──
    // They only clear when the full level resets (scene reload)
    // This is intentional — stamps accumulate across loops

    // All your existing IResettable reset code stays below:
    MonoBehaviour[] allObjects = FindObjectsOfType<MonoBehaviour>();
    foreach (MonoBehaviour obj in allObjects)
    {
        if (obj is IResettable resettable)
        {
            resettable.ResetObject();
        }
    }
}

// Add this to LevelComplete() — called when player exits level:
public void LevelComplete()
{
    loopActive = false;
    playerRecorder.StopRecording();

    // NOW clear frozen clones — full level is done
    GhostReplay.ClearAllFrozenClones();

    Debug.Log("LevelManager: Level Complete! Frozen clones cleared.");
}


    // ── Public Methods ───────────────────────────────────────────

    public int GetLoopNumber() { return loopNumber; }
    public int GetGhostCount() { return activeGhosts.Count; }



    void OnDestroy()
    {
        // Always unsubscribe from events when the object is destroyed
        if (playerRecorder != null)
            playerRecorder.OnRecordingTimedOut -= EndLoop;
    }
}