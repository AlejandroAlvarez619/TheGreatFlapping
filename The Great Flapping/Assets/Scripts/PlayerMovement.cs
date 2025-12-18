using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;               // base movement speed
    public float sprintSpeed;             // faster movement when sprinting
    public float groundDrag;              // drag applied when grounded

    public float jumpForce;               // how strong the jump is
    public float jumpCooldown;            // delay before next jump
    public float airMultiplier;           // control multiplier while in air
    bool readyToJump;                     // prevents multi-jumping

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;       // jump button
    public KeyCode sprintKey = KeyCode.LeftShift; // sprint button

    [Header("Ground Check")]
    public float playerHeight;                    // still used for backup raycast
    public LayerMask whatIsGround;                // layer representing ground

    // NEW: feet-based ground check
    public Transform groundCheck;                 // small object under the player's feet
    public float groundCheckRadius = 0.25f;       // radius of sphere check
    bool grounded;                                // are we allowed to act like we’re on the floor?

    // NEW: slope handling
    [Header("Slope Settings")]
    public float maxSlopeAngle = 60f;             // how steep a surface can be and still count as "ground"
    bool onSlope;                                 // are we currently standing on a slope?
    RaycastHit slopeHit;                          // stores info about the surface under us

    [Header("References")]
    public Transform orientation;                 // determines movement direction

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    float currentMoveSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;         // prevents the player from tipping over
        readyToJump = true;               // we can jump at the start
        currentMoveSpeed = moveSpeed;     // start with normal speed
    }

    private void Update()
    {
        // ----------------------------------------------------
        // 1) Base grounded logic
        //    Use sphere check at the feet if we have groundCheck,
        //    otherwise fall back to the original raycast.
        // ----------------------------------------------------
        bool baseGrounded;

        if (groundCheck != null)
        {
            baseGrounded = Physics.CheckSphere(
                groundCheck.position,         // position under player's feet
                groundCheckRadius,            // radius of detection sphere
                whatIsGround                  // only detect ground layer objects
            );
        }
        else
        {
            // backup: original raycast method using playerHeight
            baseGrounded = Physics.Raycast(
                transform.position,
                Vector3.down,
                playerHeight * 0.5f + 0.2f,
                whatIsGround
            );
        }

        // ----------------------------------------------------
        // 2) Slope check
        //    Cast a ray downward from the feet and look at the
        //    surface normal. If the angle is not too steep,
        //    we treat it as "ground" as well.
        // ----------------------------------------------------
        onSlope = CheckOnSlope();

        // final grounded state = either flat ground OR a valid slope
        grounded = baseGrounded || onSlope;

        MyInput();        // handle keyboard buttons
        HandleSprint();   // update sprint vs normal speed
        SpeedLimiter();   // clamp movement speed

        // small downward velocity so we "stick" to the ground / slopes
        if (grounded && rb.linearVelocity.y < 0)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -2f, rb.linearVelocity.z);

        // apply more drag when grounded (helps stopping on slopes too)
        rb.linearDamping = grounded ? groundDrag : 0f;
    }

    private void FixedUpdate()
    {
        MovePlayer();    // move physics object inside FixedUpdate
    }

    private void MyInput()
    {
        // WASD input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput   = Input.GetAxis("Vertical");

        // jump logic
        // grounded already includes slopes now, so this also allows
        // jumping off slanted objects (as long as they aren’t too steep).
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;   // temporarily block jumping
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown); // reset cooldown delay
        }
    }

    private void HandleSprint()
    {
        // if holding sprint while grounded → use sprint speed
        if (Input.GetKey(sprintKey) && grounded)
            currentMoveSpeed = sprintSpeed;
        else
            currentMoveSpeed = moveSpeed;
    }

    private void MovePlayer()
    {
        // if orientation is missing, don’t move to avoid errors
        if (orientation == null)
            return;

        // movement direction based on camera/character orientation
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // grounded movement uses full force, air movement uses reduced control
        if (grounded)
            rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedLimiter()
    {
        // get the flat (horizontal) velocity of the player
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // if we're moving too fast, clamp it
        if (flatVel.magnitude > currentMoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * currentMoveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset Y velocity so jumps are consistent
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // upward force = jump
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        // allow the next jump
        readyToJump = true;
    }

    // --------------------------------------------------------
    // NEW: checks if the player is standing on a slope that is
    //      not too steep. We use the surface normal to measure
    //      the angle between the ground and straight "up".
    // --------------------------------------------------------
    private bool CheckOnSlope()
    {
        // need a groundCheck and something under us to test a slope
        if (groundCheck == null)
            return false;

        // ray length slightly larger than our feet radius
        float rayLength = playerHeight * 0.5f + 0.5f;

        if (Physics.Raycast(groundCheck.position, Vector3.down, out slopeHit, rayLength, whatIsGround))
        {
            // angle between "up" and the surface normal
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            // if angle is between 0 and maxSlopeAngle, it's a walkable slope
            return angle > 0f && angle <= maxSlopeAngle;
        }

        return false;
    }
}
