//using UnityEngine;

//public class CameraFollow : MonoBehaviour
//{
//    public Transform target;

//    private Vector3 offset = new Vector3(0f , 3f , -6f);
//    private float followSpeed = 10f;

//    private void LateUpdate()
//    {
//        FollowPlayer();
//    }

//    private void FollowPlayer()
//    {
//        Vector3 desiredPosition = target.position + target.rotation * offset;
//        transform.position = Vector3.Lerp(transform.position , desiredPosition , followSpeed * Time.deltaTime);

//        transform.LookAt(target.position + Vector3.up * 1.5f);
//    }
//}


using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // ── Inspector Variables ─────────────────────────────────────
    [Header("Target")]
    public Transform target;             // Drag the Player here in Inspector

    [Header("Position")]
    public Vector3 offset = new Vector3(0f, 3f, -6f); // Camera position relative to player
    public float followSpeed = 8f;       // How smoothly camera follows (higher = snappier)

    [Header("Rotation")]
    public float mouseSensitivity = 2f;  // Mouse look speed
    public float minVerticalAngle = -20f; // Can't look too far down
    public float maxVerticalAngle = 60f;  // Can't look too far up

    // ── Private Variables ───────────────────────────────────────
    private float currentYaw = 0f;     // Horizontal rotation
    private float currentPitch = 0f;     // Vertical rotation

    void Start()
    {
        // Lock and hide the mouse cursor while playing
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("CameraFollow: Ready.");
    }

    void LateUpdate()
    {
        // LateUpdate runs after all Update() calls
        // This prevents the camera from jittering behind the player

        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned!");
            return;
        }

        HandleMouseLook();
        FollowTarget();
    }

    void HandleMouseLook()
    {
        // Read mouse movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Yaw   = left/right rotation (applied to the player so movement matches look direction)
        // Pitch = up/down rotation   (applied to the camera only)
        currentYaw += mouseX;
        currentPitch -= mouseY;

        // Clamp pitch so player can't flip the camera upside down
        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);

        // Rotate the player left/right so WASD always moves in the look direction
        target.rotation = Quaternion.Euler(0f, currentYaw, 0f);
    }

    void FollowTarget()
    {
        // Calculate where the camera should be
        // Rotate the offset by our current yaw and pitch
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 desiredPosition = target.position + rotation * offset;

        // Smoothly move camera toward the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Always look at the player
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}