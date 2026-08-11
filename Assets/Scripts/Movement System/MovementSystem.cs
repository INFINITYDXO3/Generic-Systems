using System;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private CharacterController characterController;

    [Header("Player")]
    [SerializeField]
    private float mass = 60;

    [SerializeField, Tooltip("Move speed of the character")]
    private float walkSpeed = 2;

    [SerializeField, Range(0, 1)]
    private float airSpeed = 0.5f;

    [Tooltip("Sprint speed of the character")]
    [SerializeField]
    private float sprintSpeed = 8;

    [SerializeField]
    private float speedChangeRate = 10;

    [SerializeField, Tooltip("The height the player can jump")]
    private float JumpForce = 15f;

    [SerializeField]
    private float playerHeight = 2;

    [SerializeField]
    private Vector3 playerCenter = Vector3.zero;

    [SerializeField, Range(0, 1), Tooltip("The percentage from the normal height, default crouch height is half the player height")]
    private float crouchPercentage = 0.5f;

    
    //Ground Check
    [Space(10)]
    [Header("Ground Interactions Settings")]
    [SerializeField, Range(0,1), Tooltip("the percentage of the speed damping caused by the ground on both x and z axis")]
    private float groundFriction;

    [SerializeField, Tooltip("The time the player gets when landing on the floor before friction gets applied")]
    private float groundFrictionTimeout = 0.05f;

    [SerializeField]
    private float groundedOffset = 0.5f;

    [SerializeField]
    private float groundedBoxSize = 0.1f;

    [SerializeField]
    private float groundedRayMaxDistance = 0.5f;

    [SerializeField]
    private LayerMask groundLayers;

    [Space(10)]
    [Header("Wall Interactions Settings")]
    [SerializeField]
    private int maxWallJumps;
    
    [SerializeField, Range(0,1), Tooltip("the percentage of the speed damping caused by the wall on the y axis")]
    private float wallFriction;

    [SerializeField]
    private float wallCheckOffset = 0.5f;
    
    [SerializeField]
    private float wallCheckRadius = 0.1f;
    
    [SerializeField]
    private LayerMask wallLayers; 

    #endregion

    private Vector3 inputVector;
    private Vector3 velocity;
    private Vector3 knockbackVector;
    private Vector3 verticalVelocity;
    private Vector3 lastDirection;

    private Vector3 groundedSpherePosition;
    private Vector3 wallSpherePosition;
    private Vector3 wallNormal;

    public float Speed {get; private set;}

    private bool isGrounded;
    private bool isNextToWall;
    private bool isSprinting;
    private bool isCrouching;
    private bool isJumping;
    private bool jumpSafeControl;

    private float currentGravity;
    private float deltaGroundFrictionTimeout;
    private float crouchHeight {get => playerHeight * crouchPercentage;}


    private int wallJumps;

    private void Start()
    {
        characterController.height = playerHeight;
        characterController.center = playerCenter;
    }


    private void LateUpdate()
    {
        GroundedCheck();
        WallCheck();
        if(isGrounded) wallJumps = 0;

    }

    private void GroundedCheck()
    {
        groundedSpherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset + characterController.center.y, transform.position.z);
        isGrounded = Physics.BoxCast(groundedSpherePosition, (new Vector3(1, 0.1f, 1) * groundedBoxSize) / 2, Vector3.down, Quaternion.identity, groundedRayMaxDistance, groundLayers);
    }

    private void WallCheck()
    {
        wallSpherePosition = new Vector3(transform.position.x , transform.position.y - wallCheckOffset + characterController.center.y, transform.position.z);
        isNextToWall = Physics.CheckSphere(wallSpherePosition, wallCheckRadius, wallLayers);

        if (isNextToWall)
        {
            RaycastHit hitInfo;
            if(!Physics.SphereCast(wallSpherePosition, wallCheckRadius, Vector3.forward, out hitInfo, wallLayers)) {}
            else if(!Physics.SphereCast(wallSpherePosition, wallCheckRadius, Vector3.back, out hitInfo, wallLayers)) {}
            else if(!Physics.SphereCast(wallSpherePosition, wallCheckRadius, Vector3.left, out hitInfo, wallLayers)) {}
            else if(Physics.SphereCast(wallSpherePosition, wallCheckRadius, Vector3.right, out hitInfo, wallLayers)) {}
            wallNormal = hitInfo.normal;
        }
    }

    private Vector3 ApplyFriction(Vector3 velocity)
    {
        if(isGrounded && deltaGroundFrictionTimeout > 0)
        {
            deltaGroundFrictionTimeout -= Time.deltaTime;
            return velocity;
        }

        Vector3 frictionVector;    
    
        if(isNextToWall && !isGrounded)
        {
            frictionVector = new (0, wallFriction * verticalVelocity.y, 0);
        }else if(isGrounded)
        {
            frictionVector = new (groundFriction * velocity.x, 0, groundFriction * velocity.z);

        }else frictionVector = Vector3.zero;

        return velocity - frictionVector;
    }

    void OnDrawGizmos()
    {
        groundedSpherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset + characterController.center.y, transform.position.z);
        wallSpherePosition = new Vector3(transform.position.x , transform.position.y - wallCheckOffset + characterController.center.y, transform.position.z);

        Gizmos.color = Color.aliceBlue;


        Gizmos.DrawCube(groundedSpherePosition, new Vector3(1, 0.1f, 1) * groundedBoxSize);
        Gizmos.DrawLine(groundedSpherePosition + (new Vector3(0, 0.1f, 0) * groundedBoxSize)/2, groundedSpherePosition + (new Vector3(0, 0.1f, 0)* groundedBoxSize)/2 - new Vector3(0, groundedRayMaxDistance, 0));


        Gizmos.color = Color.red;
        Gizmos.DrawSphere(wallSpherePosition, wallCheckRadius);

    }

    

    public void Move(Vector2 moveVector)
    {
        Vector3 inputDirection = new Vector3(moveVector.x, 0.0f, moveVector.y).normalized;

        Vector3 targetDirection = CalculateDirection(inputDirection);
        
        CalculateSpeed(inputDirection, targetDirection);

        Vector3 speedVector = (moveVector == Vector2.zero)? GetCurrentHorizontalSpeed() * lastDirection : inputVector;

        velocity = speedVector + verticalVelocity;
        velocity = ApplyFriction(velocity);
        velocity = ApplyKnockback(velocity);

        characterController.Move(Time.deltaTime * velocity);


        lastDirection = targetDirection;

        Debug.Log(speedVector.magnitude);
    }

    private Vector3 CalculateDirection(Vector3 inputDirection)
    {
        float _targetRotation = 0.0f;

        if (inputDirection != Vector3.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + transform.eulerAngles.y;
        }


        Vector3 targetDirection = (inputDirection == Vector3.zero) ? lastDirection : Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        return targetDirection;
    }


    private float GetCurrentHorizontalSpeed()
    {
        return new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;
    }

    private void CalculateSpeed(Vector3 inputDirection, Vector3 targetDirection)
    {
        float currentHorizontalSpeed = GetCurrentHorizontalSpeed();
        
        if(inputVector.magnitude < currentHorizontalSpeed || (!isGrounded && !isNextToWall))
        {
            inputVector = currentHorizontalSpeed * lastDirection;
            return;
        }

        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;
        if (inputDirection == Vector3.zero && isGrounded) targetSpeed = 0;

        if(!characterController.isGrounded) inputDirection *= airSpeed;

        inputVector = Vector3.Lerp(inputVector, targetSpeed * inputDirection.magnitude * targetDirection, speedChangeRate * Time.deltaTime);
        
    }

    public void Jump(bool value)
    {
        if(!value) jumpSafeControl = false;

        if(value == isJumping || jumpSafeControl) return;

        if (value)
        {
            if (isGrounded )
            {
                verticalVelocity.y = JumpForce;

            } else if (isNextToWall && wallJumps < maxWallJumps)
            {
                verticalVelocity.y = JumpForce;
                
                wallJumps++;
            }

            deltaGroundFrictionTimeout = groundFrictionTimeout;
        }
        

        isJumping = value;
        jumpSafeControl = true;

    }

    public void ToggleCrouch(bool value)
    {
        if(isCrouching == value) return;
        float height = characterController.height;
        Vector3 center = characterController.center;

        if (value)
        {
            height = crouchHeight;
            center = new Vector3(0, playerCenter.y + crouchHeight , 0);
        }
        else if(TryStandUp())
        {
            height = playerHeight;
            center = playerCenter;
        }else value = true;

        characterController.center = center;
        characterController.height = height;

        isCrouching = value;
    }

    private bool TryStandUp()
    {
        Vector3 headPoint = new (0, characterController.center.y  + characterController.height / 2, 0);
        Vector3 worldHeadPoint = characterController.transform.TransformPoint(headPoint);
        Debug.DrawRay(worldHeadPoint, transform.up, Color.blue, 50);
        
        if(Physics.Raycast(worldHeadPoint, transform.up, out RaycastHit hitInfo))
        {
            if(hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Player")) return true;
            else
            {
                Debug.Log(hitInfo.collider.name);
                return false;
            }
        }else return true;

    }

    public void ToggleSprint(bool value)
    {
        if(isSprinting == value) return;

        isSprinting = value && isGrounded;
    }

    public void ApplyGravity(float gravityForce)
    {
        gravityForce *= Time.timeScale * Time.deltaTime;
        currentGravity = gravityForce ;

        verticalVelocity.y = Mathf.Lerp(verticalVelocity.y, verticalVelocity.y + gravityForce * mass, 1);

        if(isGrounded) verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, -2, 100);
        else if (isNextToWall) verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, -5, 100);
    }

    
    private Vector3 ApplyKnockback(Vector3 velocity)
    {
        velocity += knockbackVector;
        knockbackVector = Vector3.Lerp(knockbackVector, Vector3.zero, Time.deltaTime * speedChangeRate);
        return velocity;
    }

    public void SetKnockback(Vector3 knockback)
    {
         knockbackVector = knockback;
    }
}
