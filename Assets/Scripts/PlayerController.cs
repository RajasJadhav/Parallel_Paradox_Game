using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 5f;
    private bool isOnGround;


    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        isOnGround = true;
    }

    private void Update()
    {
        Move();
        Jump();
    }

    private void Move()
    {
        float forward = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        playerRb.AddForce(Vector3.forward * forward * speed , ForceMode.Force);
        playerRb.AddForce(Vector3.right * horizontal * speed, ForceMode.Force);
    }

    private void Jump()
    {
         if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
          {
              playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
              isOnGround = false;
         }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }
}
