using UnityEngine;
using System.Collections.Generic;

public class TimelineRecorder : MonoBehaviour
{
    [HideInInspector] public bool isRecording = false;

    // ── NEW — tracks if C was pressed this fixed frame ───────────
    private bool stampPressedThisFrame = false;
    // ─────────────────────────────────────────────────────────────

    private List<FrameData>  recordedFrames   = new List<FrameData>();
    private PlayerController playerController;

    public System.Action OnRecordingTimedOut;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (playerController == null)
            Debug.LogError("TimelineRecorder: No PlayerController found!");

        Debug.Log("TimelineRecorder: Ready. Press R to record. Press C to stamp position.");
    }

    void Update()
    {
        // ── R key — start / stop recording ───────────────────────
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isRecording)
                StartRecording();
            else
            {
                StopRecording();
                OnRecordingTimedOut?.Invoke();
            }
        }

        // ── C key — stamp current position during recording ───────
        // Read in Update so no keypresses are missed
        // The flag is consumed in the next FixedUpdate frame
        if (isRecording && Input.GetKeyDown(KeyCode.C))
        {
            stampPressedThisFrame = true;

            Debug.Log($"TimelineRecorder: Stamp marked at {transform.position}. " +
                      $"Will appear as frozen clone on next loop.");
        }
    }

    void FixedUpdate()
    {
        if (!isRecording) return;
        CaptureFrame();
    }

    void CaptureFrame()
    {
        bool moving  = Input.GetAxis("Horizontal") != 0
                    || Input.GetAxis("Vertical")   != 0;
        bool jumping = !playerController.IsGrounded();

        // ── Pass stamp flag into this frame ───────────────────────
        FrameData frame = new FrameData(
            transform.position,
            transform.rotation,
            moving,
            jumping,
            false,
            stampPressedThisFrame  // ← NEW: marks this as a stamp frame
        );

        recordedFrames.Add(frame);

        // ── Consume the stamp flag after one frame ────────────────
        // Only one frame gets marked per C press
        if (stampPressedThisFrame)
            stampPressedThisFrame = false;
    }

    public void StartRecording()
    {
        recordedFrames.Clear();
        isRecording = true;
        stampPressedThisFrame = false;

        Debug.Log("TimelineRecorder: Recording started. " +
                  "Walk around. Press C to stamp positions.");
    }

    public void StopRecording()
    {
        isRecording           = false;
        stampPressedThisFrame = false;

        Debug.Log($"TimelineRecorder: Stopped. {recordedFrames.Count} frames captured.");
    }

    public List<FrameData> GetRecordedFrames()
    {
        return new List<FrameData>(recordedFrames);
    }
}