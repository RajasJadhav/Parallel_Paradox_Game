using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// Attach this to the Player GameObject
// It records everything the player does, frame by frame

public class TimelineRecorder : MonoBehaviour
{
    // ── Inspector Variables ─────────────────────────────────────
    [Header("Recording Settings")]
    public float maxRecordingTime = 30f;  // Maximum length of one loop in seconds

    // ── State ───────────────────────────────────────────────────
    [HideInInspector] public bool isRecording = false;

    // ── Private Data ─────────────────────────────────────────────
    private List<FrameData> recordedFrames = new List<FrameData>();
    private float recordingTimer = 0f;
    private PlayerController playerController; // Reference to check isGrounded

    // ── Events ───────────────────────────────────────────────────
    // LevelManager listens to this — fires when max time is reached
    public System.Action OnRecordingTimedOut;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (playerController == null)
            Debug.LogError("TimelineRecorder: No PlayerController found on this GameObject!");

        Debug.Log("TimelineRecorder: Ready.");
    }

    void FixedUpdate()
    {
        // FixedUpdate runs at a fixed rate (default 50 times/second)
        // We record here so playback is frame-rate independent

        if (!isRecording) return;

        // Tick the recording timer
        recordingTimer += Time.fixedDeltaTime;

        // Auto-stop if we hit the max recording time
        if (recordingTimer >= maxRecordingTime)
        {
            Debug.Log("TimelineRecorder: Max recording time reached. Auto-stopping.");
            OnRecordingTimedOut?.Invoke(); // Tell LevelManager
            return;
        }

        // Capture this frame and add it to the list
        CaptureFrame();
    }

    // ── Private Methods ──────────────────────────────────────────

    void CaptureFrame()
    {
        // Read what the player is doing RIGHT NOW
        bool moving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        bool jumping = !playerController.IsGrounded();

        // Build a FrameData and store it
        FrameData frame = new FrameData(
            transform.position,
            transform.rotation,
            moving,
            jumping,
            false  // isPressingSomething handled by PressurePlate.cs separately
        );

        recordedFrames.Add(frame);
    }

    // ── Public Methods ───────────────────────────────────────────

    public void StartRecording()
    {
        recordedFrames.Clear(); // Wipe any previous recording
        recordingTimer = 0f;
        isRecording = true;

        Debug.Log("TimelineRecorder: Recording started.");
    }

    public void StopRecording()
    {
        isRecording = false;

        Debug.Log($"TimelineRecorder: Recording stopped. {recordedFrames.Count} frames captured.");
    }

    // Returns a COPY of the recorded frames
    // GhostReplay gets this list to play back
    public List<FrameData> GetRecordedFrames()
    {
        return new List<FrameData>(recordedFrames);
    }
}