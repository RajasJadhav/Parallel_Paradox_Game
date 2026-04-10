using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Animator animator;          // ← ADD
    private bool isGrounded;
    private Vector3 moveDirection;
    private int groundContactCount = 0;

    // Animator parameter hashes (faster than string lookups)
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();   // ← ADD

        rb.freezeRotation = true;

        if (animator == null)
            Debug.LogError("PlayerController: No Animator found!");

        Debug.Log("PlayerController: Ready.");
    }

    void Update()
    {
        HandleInput();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        UpdateAnimator();   // ← ADD
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        moveDirection = transform.right * horizontal + transform.forward * vertical;
    }

    void MovePlayer()
    {
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }

    // ── Animator ──────────────────────────────────────────────────

    void UpdateAnimator()
    {
        if (animator == null) return;

        // Speed: magnitude of the horizontal input direction (0–1)
        float speed = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        ).magnitude;

        animator.SetFloat(SpeedHash, speed);
        animator.SetBool(IsGroundedHash, isGrounded);
    }

    // ── Jump ──────────────────────────────────────────────────────

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Debug.Log("Jumped!");
    }

    // ── Collision-based ground detection ──────────────────────────

    void OnCollisionEnter(Collision collision)
    {
        if (IsGroundLayer(collision))
        {
            groundContactCount++;
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (IsGroundLayer(collision))
        {
            groundContactCount--;
            if (groundContactCount <= 0)
            {
                groundContactCount = 0;
                isGrounded = false;
            }
        }
    }

    bool IsGroundLayer(Collision collision)
    {
        return (groundLayer.value & (1 << collision.gameObject.layer)) != 0;
    }

    public bool IsGrounded() => isGrounded;
}