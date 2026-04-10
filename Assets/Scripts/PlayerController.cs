using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Animator anim; // 1. Reference to the Animator
    private bool isGrounded;
    private Vector3 moveDirection;
    private int groundContactCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>(); // 2. Initialize Animator
        rb.freezeRotation = true;
        Debug.Log("PlayerController: Ready.");
    }

    void Update()
    {
        HandleInput();
        UpdateAnimations(); // 3. Call the animation update loop

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
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

    // 4. New method to send data to the Animator Controller
    void UpdateAnimations()
    {
        // Update the 'IsGrounded' boolean parameter seen in your screenshot
        anim.SetBool("IsGrounded", isGrounded);

        // Calculate horizontal speed (ignoring vertical velocity)
        float horizontalSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        // Use 'Speed' parameter to transition from Idle to Walking
        // (Assuming you have or will add a 'Speed' float parameter)
        anim.SetFloat("Speed", horizontalSpeed);
    }

    void MovePlayer()
    {
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Optional: Trigger a jump animation if you have a "Jump" trigger
        // anim.SetTrigger("Jump");

        Debug.Log("Jumped!");
    }

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