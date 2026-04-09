using System.Collections.Generic;
using UnityEngine;

public class TimelineRecorder : MonoBehaviour
{
    [HideInInspector] public bool isRecording = false;

    private List<FrameData> recordedFrames = new List<FrameData>();
    private PlayerController playerController;

    public System.Action OnRecordingTimedOut; // Still used — fires when R is pressed to stop

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (playerController == null)
            Debug.LogError("TimelineRecorder: No PlayerController found!");

        Debug.Log("TimelineRecorder: Ready. Press R to start recording.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isRecording)
            {
                StartRecording();
            }
            else
            {
                StopRecording();
                OnRecordingTimedOut?.Invoke(); // Tells LevelManager to end the loop + spawn ghost
            }
        }
    }

    void FixedUpdate()
    {
        if (!isRecording) return;
        CaptureFrame();
    }

    void CaptureFrame()
    {
        bool moving = Input.GetAxis("Horizontal") != 0
                    || Input.GetAxis("Vertical") != 0;
        bool jumping = !playerController.IsGrounded();

        // Record the EXACT transform.position — no offset math
        // transform.position on a capsule IS the center
        // Ghost will replay to the same center → no float
        FrameData frame = new FrameData(
            transform.position,   // ← center, exactly as Unity stores it
            transform.rotation,
            moving,
            jumping,
            false
        );

        recordedFrames.Add(frame);
    }

    public void StartRecording()
    {
        recordedFrames.Clear();
        isRecording = true;
        Debug.Log("TimelineRecorder: Recording started.");
    }

    public void StopRecording()
    {
        isRecording = false;
        Debug.Log($"TimelineRecorder: Stopped. {recordedFrames.Count} frames captured.");
    }

    public List<FrameData> GetRecordedFrames()
    {
        return new List<FrameData>(recordedFrames);
    }
}