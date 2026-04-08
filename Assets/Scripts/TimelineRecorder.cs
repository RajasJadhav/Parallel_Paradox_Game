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
        bool moving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        bool jumping = !playerController.IsGrounded();

        // Get the bottom of the capsule (feet position) instead of center
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        Vector3 feetPosition = transform.position;
        if (col != null)
        {
            // Subtract the capsule's center offset to get true feet position
            feetPosition = transform.position - new Vector3(0, col.height / 2f - col.radius, 0);
        }

        FrameData frame = new FrameData(
            feetPosition,        // ← Record feet, not center
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