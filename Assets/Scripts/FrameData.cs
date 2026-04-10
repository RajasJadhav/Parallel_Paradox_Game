using UnityEngine;

[System.Serializable]
public struct FrameData
{
    public Vector3    position;
    public Quaternion rotation;
    public bool       isMoving;
    public bool       isJumping;
    public bool       isPressingSomething;

    // ── NEW ──────────────────────────────────────────────────────
    // True on the exact frame the player pressed C
    // GhostReplay reads this to know where to spawn a frozen clone
    public bool isStampFrame;
    // ─────────────────────────────────────────────────────────────

    public FrameData(Vector3 pos, Quaternion rot, bool moving,
                     bool jumping, bool pressing, bool stamp = false)
    {
        position            = pos;
        rotation            = rot;
        isMoving            = moving;
        isJumping           = jumping;
        isPressingSomething = pressing;
        isStampFrame        = stamp;
    }
}