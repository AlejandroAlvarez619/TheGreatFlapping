using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float sprintSpeed = 10f;
    public float moveForce = 10f;

    public float groundDrag = 6f;
    public float airDrag = 0.5f;

    public float airSpeedMultiplier = 0.55f;
    public float extraFallGravity = 25f;
    public float maxFallSpeed = 35f;

    public float jumpForce = 6f;
    public float jumpWindowSeconds = 1f;
    public int jumpsPerWindow = 2;

    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    Rigidbody rb;
    float currentMoveSpeed;

    public Animator animator;

    private void Start();
    int jumpsUsed;
    float windowTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        jumpsUsed = 0;
        windowTimer = 0f;
    }

    void Update()
    {
        windowTimer += Time.deltaTime;
        if (windowTimer >= jumpWindowSeconds)
        {
            windowTimer = 0f;
            jumpsUsed = 0;
        }

        if (grounded) jumpsLeft = maxJumps;

        MyInput();
        HandleSprint();
        SpeedLimiter();
        UpdateAnimator();

        if (grounded && rb.linearVelocity.y < 0)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -2f, rb.linearVelocity.z);

        rb.linearDamping = grounded ? groundDrag : 0f;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        float baseSpeed = Input.GetKey(sprintKey) ? sprintSpeed : moveSpeed;

        bool inAir = rb.linearVelocity.y > 0.1f || rb.linearVelocity.y < -0.1f;

        currentMoveSpeed = inAir ? baseSpeed * airSpeedMultiplier : baseSpeed;
        rb.linearDamping = inAir ? airDrag : groundDrag;

        if (Input.GetKeyDown(jumpKey) && jumpsUsed < jumpsPerWindow)
        {
            Jump();
            jumpsUsed++;
        }

        if (rb.linearVelocity.y < 0f)
            rb.AddForce(Vector3.down * extraFallGravity, ForceMode.Acceleration);

        if (rb.linearVelocity.y < -maxFallSpeed)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -maxFallSpeed, rb.linearVelocity.z);
    }

    void FixedUpdate()
    {
        MovePlayer();
        SpeedLimiter();
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (moveDirection.sqrMagnitude < 0.0001f) return;

        rb.AddForce(moveDirection.normalized * currentMoveSpeed * moveForce, ForceMode.Force);
    }

    void SpeedLimiter()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float maxSpeed = currentMoveSpeed;

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float rawSpeed = flatVel.magnitude;

        float animSpeed = 0f;

        if (rawSpeed < 0.1f)
        {
            animSpeed = 0f;
        }
        else if (Input.GetKey(sprintKey))
        {
            animSpeed = 3f;
        }
        else
        {
            animSpeed = 1f;
        }

        animator.SetFloat("Speed", animSpeed);
    }
}