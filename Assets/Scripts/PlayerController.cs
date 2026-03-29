//using System;
//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    private Rigidbody playerRb;

//    [SerializeField] private float speed = 10f;
//    [SerializeField] private float jumpForce = 5f;
//    public bool isOnGround;


//    private void Start()
//    {
//        playerRb = GetComponent<Rigidbody>();
//        isOnGround = true;
//    }

//    private void Update()
//    {
//        RotatePlayer();
//    }

//    private void FixedUpdate()
//    {
//        Move();
//        Jump();
//    }

//    private void Move()
//    {
//        float forward = Input.GetAxis("Vertical");
//        float horizontal = Input.GetAxis("Horizontal");

//        Vector3 moveDir = transform.forward * forward + transform.right * horizontal;

//        playerRb.AddForce(moveDir * speed, ForceMode.Force);
//    } 

//    private void Jump()
//    {
//         if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
//          {
//              playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
//              isOnGround = false;
//         }
//    }

//    private void RotatePlayer()
//    {
//        float mouseX = Input.GetAxis("Mouse X") * 200f * Time.deltaTime;
//        transform.Rotate(Vector3.up * mouseX);
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if(collision.gameObject.CompareTag("Ground"))
//        {
//            isOnGround = true;
//        }
//    }
//}


using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ── Inspector Variables ─────────────────────────────────────
    [Header("Movement")]
    public float moveSpeed = 5f;         // How fast the player walks
    public float jumpForce = 7f;         // How high the player jumps
    public float groundCheckDistance = 0.6f; // How far below to check for ground

    [Header("Ground Detection")]
    public LayerMask groundLayer;        // Set this to your Ground layer in Inspector

    // ── Private Variables ───────────────────────────────────────
    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 moveDirection;

    // ── Unity Messages ──────────────────────────────────────────
    void Start()
    {
        // Grab the Rigidbody component attached to this GameObject
        rb = GetComponent<Rigidbody>();

        // Freeze rotation so the player doesn't tip over
        rb.freezeRotation = true;

        Debug.Log("PlayerController: Ready.");
    }

    void Update()
    {
        // Read keyboard input every frame
        HandleInput();

        // Check if player is standing on ground
        CheckGrounded();

        // Jump input must be in Update (not FixedUpdate) so no keypresses are missed
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        // Movement goes in FixedUpdate because we are moving a Rigidbody
        MovePlayer();
    }

    // ── Private Methods ─────────────────────────────────────────

    void HandleInput()
    {
        // GetAxis returns a value between -1 and 1
        // Horizontal = A/D keys or Left/Right arrows
        // Vertical   = W/S keys or Up/Down arrows
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Build a direction vector relative to where the player is facing
        moveDirection = transform.right * horizontal
                      + transform.forward * vertical;
    }

    void MovePlayer()
    {
        // Move the Rigidbody by setting its velocity directly
        // We keep the Y velocity (gravity) and only override X and Z
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.linearVelocity.y;      // Preserve existing vertical velocity
        rb.linearVelocity = velocity;
    }

    void Jump()
    {
        // Reset Y velocity before jumping so double-jumps don't stack
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Apply an upward impulse force
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        Debug.Log("PlayerController: Jumped.");
    }

    void CheckGrounded()
    {
        // Cast a short ray downward from the player's feet
        // If it hits something on the Ground layer, we are grounded
        isGrounded = Physics.Raycast(
            transform.position,        // Start at player's position
            Vector3.down,              // Shoot downward
            groundCheckDistance,       // This far
            groundLayer                // Only hit Ground layer objects
        );
    }

    // ── Public Getters (used by TimelineRecorder) ───────────────

    public bool IsGrounded()
    {
        return isGrounded;
    }
}