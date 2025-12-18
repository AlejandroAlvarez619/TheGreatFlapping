using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float sprintSpeed = 10f;
    public float moveForce = 10f;

    public float groundDrag = 6f;
    public float airDrag = 0.5f;

    public float airSpeedMultiplier = 0.55f;
    public float extraFallGravity = 3f;
    public float maxFallSpeed = 35f;
    public Animator animator;
    public float jumpForce = 6f;
    public float jumpWindowSeconds = 1f;
    public int jumpsPerWindow = 2;
    
    [Header("Rotation")]
    public float turnSpeed = 12f;
    public Transform model;          
    public float modelYawOffset = 0; 


    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    Rigidbody rb;
    float currentMoveSpeed;

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

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        float baseSpeed = Input.GetKey(sprintKey) ? sprintSpeed : moveSpeed;

        bool inAir = rb.linearVelocity.y > 0.1f || rb.linearVelocity.y < -0.1f;
        UpdateAnimator();
        currentMoveSpeed = inAir ? baseSpeed * airSpeedMultiplier : baseSpeed;
        rb.linearDamping = inAir ? airDrag : groundDrag;

        if (Input.GetKeyDown(jumpKey) && jumpsUsed < jumpsPerWindow)
        {
            Jump();
            jumpsUsed++;
        }

        
    }

   void FixedUpdate()
{
    MovePlayer();
    RotatePlayer();
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

    void RotatePlayer()
{
    Vector3 lookDir = new Vector3(moveDirection.x, 0f, moveDirection.z);
    if (lookDir.sqrMagnitude < 0.0001f) return;

    Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.fixedDeltaTime);

    
    if (model != null)
        model.localRotation = Quaternion.Euler(0f, modelYawOffset, 0f);
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
