using UnityEngine;

// This is NOT a MonoBehaviour — it is a plain data container
// One FrameData is created every fixed frame during recording
// A List of FrameData is what makes the ghost replay possible

[System.Serializable]
public struct FrameData
{
    // ── Position & Rotation ─────────────────────────────────────
    public Vector3 position;       // Where was the player this frame?
    public Quaternion rotation;       // Which way were they facing?

    // ── Action Flags ────────────────────────────────────────────
    // These tell the ghost what the player WAS doing this frame
    // so the ghost can trigger the same effects (sounds, animations)
    public bool isMoving;             // Was the player walking?
    public bool isJumping;            // Were they in the air?
    public bool isPressingSomething;  // Were they standing on a pressure plate?

    // ── Constructor ─────────────────────────────────────────────
    // Makes it easy to create a FrameData in one line
    public FrameData(Vector3 pos, Quaternion rot, bool moving, bool jumping, bool pressing)
    {
        position = pos;
        rotation = rot;
        isMoving = moving;
        isJumping = jumping;
        isPressingSomething = pressing;
    }
}