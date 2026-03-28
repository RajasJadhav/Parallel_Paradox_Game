using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    private Vector3 offset = new Vector3(0f, 3f, -6f);

    private float followSpeed = 8f;
    private float mouseSensitivity = 2f;
    private float minVerticalAngle = -20f;
    private float maxVerticalAngle = 60f;

    private float currentyaw = 0f;
    private float currentpitch = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        HandleMouse();
        FollowPlayer();
    }

    private void HandleMouse()
    {
        //read mouse movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // yaw = left/right roatation : player
        // pitch = up/dpwn rotation : camera
        currentyaw += mouseX;
        currentpitch += mouseY;

        currentpitch = Mathf.Clamp(currentpitch,minVerticalAngle,maxVerticalAngle);

        target.rotation = Quaternion.Euler(0f, currentyaw, 0f);
    }

    private void FollowPlayer()
    {
        Quaternion rotation = Quaternion.Euler(currentpitch , currentyaw, 0f);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
