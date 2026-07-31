using NUnit.Framework;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private CharacterController characterController;

    [Header("Player")]
    [SerializeField, Tooltip("Move speed of the character")]
    private float walkSpeed = 2;

    [Tooltip("Sprint speed of the character")]
    [SerializeField]
    private float sprintSpeed = 8;

    [SerializeField]
    private float speedChangeRate = 10;

    [Space(10)]
    [SerializeField, Tooltip("The height the player can jump")]
    private float JumpForce = 1.2f;

    [SerializeField]
    private float airTime = 3;

    //Ground Check
    [Header("Ground Check Settings")]
    [SerializeField]
    private float groundedOffset = 0.5f;
    [SerializeField]
    private float groundedRadius = 0.1f;

    [SerializeField]
    private LayerMask GroundLayers;

    [Header("Wall Check Settings")]
    [SerializeField]
    private int maxWallJumps;
    
    [SerializeField]
    private float wallCheckOffset = 0.5f;
    
    [SerializeField]
    private float wallCheckRadius = 0.1f;
    
    [SerializeField]
    private LayerMask WallLayers; 

    #endregion

    private Vector3 knockbackVector;
    private Vector3 verticalVelocity;    
    private Vector3 lastDirection;

    private Vector3 groundedSpherePosition;
    private Vector3 wallSpherePosition;

    public float Speed {get; private set;}

    private bool isGrounded;
    private bool isNextToWall;
    private bool isSprinting;
    private bool isJumping;
    private bool jumpSafeControl;

    private float airTimeDelta;
    private float currentGravity;

    private int wallJumps;

    void Update()
    {
        if(airTimeDelta > 0)
        {
            airTimeDelta -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        GroundedCheck();
        WallCheck();
        if(isGrounded) wallJumps = 0;

    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        groundedSpherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(groundedSpherePosition, groundedRadius, GroundLayers);

    }

    private void WallCheck()
    {
        // set sphere position, with offset
        wallSpherePosition = new Vector3(transform.position.x + wallCheckOffset, transform.position.y, transform.position.z);
        isNextToWall = Physics.CheckSphere(wallSpherePosition, wallCheckRadius, WallLayers);

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.aliceBlue;


        Gizmos.DrawSphere(groundedSpherePosition, groundedRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(wallSpherePosition, wallCheckRadius);

    }

    public void Sprint(bool value)
    {
        if(isSprinting == value) return;

        isSprinting = value;
    }

    public void Move(Vector2 moveVector)
    {
        float targetSpeed = GetTargetSpeed(moveVector);

        Vector3 inputDirection = new Vector3(moveVector.x, 0.0f, moveVector.y).normalized;

        float currentHorizontalSpeed = GetCurrentHorizontalSpeed();
        
        CalculateSpeed(targetSpeed, inputDirection, currentHorizontalSpeed);

        Vector3 targetDirection = CalculateDirection(inputDirection);
        Vector3 velocity = (Speed * targetDirection.normalized) + verticalVelocity + knockbackVector;
        Debug.Log(verticalVelocity.y);
        characterController.Move(Time.deltaTime * velocity);
    }

    private Vector3 CalculateDirection(Vector3 inputDirection)
    {
        float _targetRotation = 0.0f;

        if (inputDirection != Vector3.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + transform.eulerAngles.y;
        }


        Vector3 targetDirection = (inputDirection == Vector3.zero) ? lastDirection : Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        lastDirection = targetDirection;
        return targetDirection;
    }


    private float GetCurrentHorizontalSpeed()
    {
        return new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;
    }

    private float GetTargetSpeed(Vector2 moveVector)
    {
        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;
        if(moveVector == Vector2.zero)
        {
            if(isGrounded) targetSpeed = 0;
            else targetSpeed = GetCurrentHorizontalSpeed();
        }

        return targetSpeed;
    }

    private void CalculateSpeed(float targetSpeed, Vector3 inputDirection, float currentHorizontalSpeed)
    {
        float speedOffset = 0.1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            Speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputDirection.magnitude, speedChangeRate * Time.deltaTime);

            // round speed to 3 decimal places
            Speed = Mathf.Round(Speed * 1000f) / 1000f;
        }
        else
        {
            Speed = targetSpeed;
        }
    }

    public void Jump(bool value)
    {
        if(!value) jumpSafeControl = false;
        
        if(value == isJumping || jumpSafeControl) return;

        if (value)
        {
            if (isGrounded || (isNextToWall && wallJumps < maxWallJumps))
            {
                verticalVelocity.y += JumpForce;
                airTimeDelta = airTime;

                if(isNextToWall) wallJumps++;
            }
        }
        

        isJumping = value;
        jumpSafeControl = true;
    }

    public void ApplyGravity(float gravityForce)
    {
        if(airTimeDelta > 0) return;
        if(isNextToWall && !isGrounded) gravityForce *= 0.5f;
        currentGravity = gravityForce;

        verticalVelocity.y += gravityForce;

        if(isGrounded) verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, -2, 100);
    }

    public void ApplyKnockback(Vector3 knockback)
    {
        knockbackVector = knockback;
    }
}
