using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 5f;
    public bool isOnGround;


    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        isOnGround = true;
    }

    private void Update()
    {
        RotatePlayer();
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
    }

    private void Move()
    {
        float forward = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 moveDir = transform.forward * forward + transform.right * horizontal;

        playerRb.AddForce(moveDir * speed, ForceMode.Force);
    } 

    private void Jump()
    {
         if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
          {
              playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
              isOnGround = false;
         }
    }

    private void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * 200f * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }
}
