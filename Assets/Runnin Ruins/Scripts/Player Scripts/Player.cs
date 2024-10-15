using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    Rigidbody rb;
    MeshCollider meshCollider;
    [SerializeField] LayerMask groundLayer;
    public float moveInput;
    public float direction;

    public PlayerLeg leg1;
    public PlayerLeg leg2;

    public float savedDirection;
    public bool playerFlipped;

    public bool isGrounded;
    public bool usedGroundJump = false;
    public bool jumpKeyHeld;
    public bool isJumping;
    public bool jumpInputDown;
    public bool jumpInputUp;
    public float jumpHeight;
    private float jumpForce;

    // Start is called before the first frame update
    void Start()
    {
        direction = -1;
        rb = GetComponent<Rigidbody>();
        meshCollider = GetComponent<MeshCollider>();
        jumpForce = jumpForce = CalculateJumpForce(Physics.gravity.magnitude, jumpHeight);
    }

    private void FixedUpdate()
    {
        if (leg1.IsGrounded() || leg2.IsGrounded())
        {
            isGrounded = true;
            isJumping = false;
            usedGroundJump = false;
        }
        else
        {
            isGrounded = false;
            isJumping = true;
            usedGroundJump = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpInputDown = Input.GetButtonDown("Jump");
        jumpInputUp = Input.GetButtonUp("Jump");

        Move();
        Flip();

        if (jumpInputDown)
        {
            // Player jumping
            Jump();
        }
        else if (jumpInputUp)
        {
            jumpKeyHeld = false;
        }

        JumpStopped();

    }

    void Move()
    {
        rb.velocity = new Vector3(moveInput * moveSpeed, rb.velocity.y, rb.velocity.z);
    }

    void Flip()
    {
        if (moveInput < 0)
            direction = 1;
        else if (moveInput > 0)
            direction = -1;

        if (direction != 0 && direction != savedDirection)
        {
            savedDirection = direction;
            playerFlipped = true;
        }
        else
        {
            playerFlipped = false;
        }

        transform.localEulerAngles = new Vector3(0.0f, 90f * direction, 0.0f);
    }

    public void Jump()
    {
        jumpKeyHeld = true;
        isJumping = false;
        if (!usedGroundJump)
        {
            rb.isKinematic = false;
            isJumping = true;
            if (isGrounded) // Player jumps on the ground
            {
                jumpForce = CalculateJumpForce(Physics.gravity.magnitude, jumpHeight);
                rb.velocity = new Vector3(rb.velocity.x, 0f, 0f);
                rb.AddForce(Vector3.up * jumpForce * rb.mass, ForceMode.Impulse);
                usedGroundJump = true;
            }
        }
    }

    public void JumpStopped()
    {
        if (isJumping && !jumpKeyHeld && rb.velocity.y > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, 0f);
        }
    }

    public static float CalculateJumpForce(float gravityStrength, float jumpHeight)
    {
        return Mathf.Sqrt(2 * gravityStrength * jumpHeight);
    }

    public float GetMoveInput() { return moveInput; }

}
