using UnityEngine;

// This is NOT a MonoBehaviour — it is a plain data container
// One FrameData is created every fixed frame during recording
// A List of FrameData is what makes the ghost replay possible

[System.Serializable]
public struct FrameData
{
    public Vector3 position;
    public Quaternion rotation;
    public bool isMoving;
    public bool isJumping;
    public bool isPressingSomething;
    public float speed;           // ← ADD: store actual speed value

    public FrameData(Vector3 pos, Quaternion rot, bool moving, bool jumping, bool pressing, float spd)
    {
        position = pos;
        rotation = rot;
        isMoving = moving;
        isJumping = jumping;
        isPressingSomething = pressing;
        speed = spd;              // ← ADD
    }
}