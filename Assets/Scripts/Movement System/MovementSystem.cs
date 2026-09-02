using System;
using System.Collections;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private CharacterController characterController;

    [SerializeField]
    private CapsuleCollider otherCollider;

    [Header("Player")]
    [SerializeField]
    private float mass = 60;

    [SerializeField]
    private float terminalVelocity = 35;

    [SerializeField, Tooltip("Move speed of the character")]
    private float walkSpeed = 5;

    [SerializeField, Range(0, 1)]
    private float airSpeed = 0.5f;

    [Tooltip("Sprint speed of the character")]
    [SerializeField]
    private float sprintSpeed = 15;

    [SerializeField]
    private float slideSpeed = 15;

    [SerializeField]
    private float slideTime = 1;

    [SerializeField]
    private float speedChangeRate = 10;

    [SerializeField, Tooltip("The height the player can jump")]
    private float jumpForce = 15f;

    [SerializeField]
    private float playerHeight = 2;

    [SerializeField]
    private Vector3 playerCenter = Vector3.zero;

    [SerializeField, Range(0, 1), Tooltip("The percentage from the normal height, default crouch height is half the player height")]
    private float crouchPercentage = 0.5f;

    
    //Ground Check
    [Space(10)]
    [Header("Ground Interactions Settings")]
    [SerializeField]
    private GroundSensor groundSensor;
    
    [SerializeField, Range(0,1), Tooltip("the percentage of the speed damping caused by the ground on both x and z axis")]
    private float groundFriction = 0.05f;

    [SerializeField, Tooltip("The time the player gets when landing on the floor before friction gets applied")]
    private float groundFrictionTimeout = 0.05f;

    [Space(10)]
    [Header("Wall Interactions Settings")]
    [SerializeField]
    private WallSensor wallSensor;
    
    [SerializeField]
    private int maxWallJumps = 2;
    
    [SerializeField, Range(0,1), Tooltip("the percentage of the speed damping caused by the wall on the y axis")]
    private float wallFriction = 0.1f;

    

    #endregion

    private Vector2 cachedInput;
    private Vector3 horizontalVelocity;
    private Vector3 velocity;
    private Vector3 knockbackVector;
    private Vector3 verticalVelocity;
    private Vector3 lastDirection;
    private Vector3 additionalMovementVector;

    private Vector3 currentHorizontalVelocity;

    private FrictionSurface currentFrictionSurface;

    public float Speed {get; private set;}

    private bool isSprinting;
    private bool isSliding;
    private bool isCrouching;
    private bool isJumping;
    private bool jumpSafeControl;

    private Vector3 currentGravity;
    private float deltaGroundFrictionTimeout;
    private float crouchHeight {get => playerHeight * crouchPercentage;}


    private int wallJumps;

    private Coroutine slidingCoroutine;

    private void Start()
    {
        characterController.height = playerHeight;
        characterController.center = playerCenter;
    }


    private void Update()
    {
        if(groundSensor.IsGrounded) wallJumps = 0;
        Move();
        ApplyGravity();
    }

    public void SetInput(Vector2 moveVector)
    {
        cachedInput = moveVector;
    }
 
    public void Move()
    {
        Vector3 inputDirection = ProjectOnPlane(Vector3.one * cachedInput.magnitude);
        Vector3 targetDirection = CalculateDirection(cachedInput);
        currentHorizontalVelocity = GetCurrentHorizontalVelocity();

        if(currentHorizontalVelocity.magnitude < 0.5f) currentHorizontalVelocity = Vector3.zero;


        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;

        if(!groundSensor.IsGrounded) inputDirection *= airSpeed;

        if(inputDirection != Vector3.zero) 
        {
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, targetSpeed * inputDirection.normalized.magnitude * targetDirection, speedChangeRate * Time.deltaTime);
            

            float directionDifference = Vector3.Dot(targetDirection, currentHorizontalVelocity.normalized);
            if(directionDifference > 0 && currentHorizontalVelocity.magnitude > horizontalVelocity.magnitude && !groundSensor.IsGrounded)
            {
                horizontalVelocity = targetDirection * currentHorizontalVelocity.magnitude;
            }else if(directionDifference < 0 && !groundSensor.IsGrounded)
            {
                horizontalVelocity = targetSpeed * inputDirection.magnitude * targetDirection;
            }

       
        }else horizontalVelocity = currentHorizontalVelocity;
        
        velocity = horizontalVelocity + verticalVelocity;

        velocity = ApplyVector(ref knockbackVector, velocity);
        velocity = ApplyVector(ref additionalMovementVector, velocity);
        velocity = ApplyFriction(velocity);

        if(velocity.magnitude > terminalVelocity)
        {
            velocity *= terminalVelocity/velocity.magnitude;
        }


        characterController.Move(Time.deltaTime * velocity);

        lastDirection = (velocity != Vector3.zero)?  velocity.normalized : targetDirection;

        if(GetCurrentVerticalVelocity().magnitude < 0.1f && Vector3.Dot(verticalVelocity.normalized, currentGravity.normalized) < 0) verticalVelocity = GetCurrentVerticalVelocity();

    }

    public void Jump()
    {
        if (groundSensor.IsGrounded )
        {
            verticalVelocity = Project(Vector3.one).normalized * jumpForce;

        } else if (wallSensor.IsNextToWall && wallJumps < maxWallJumps)
        {
            verticalVelocity = Project(Vector3.one).normalized * jumpForce;
            
            if(wallSensor != null) additionalMovementVector = wallSensor.WallNormal * jumpForce;

            wallJumps++;
        }

        deltaGroundFrictionTimeout = groundFrictionTimeout;

    }

    public void ToggleCrouch(bool value)
    {
        if(isCrouching == value || isSliding) return;
        float height = characterController.height;
    
        if (value)
        {
            height = crouchHeight;

            if(isSprinting && groundSensor.IsGrounded) ToggleSlide();
        }
        else if(TryStandUp())
        {
            height = playerHeight;
        }else value = true;

        characterController.height = height;
        otherCollider.height = height;
        isCrouching = value;
    }

    private void ToggleSlide()
    {
        isSliding = true;
        slidingCoroutine ??= StartCoroutine(SlideC());
    }


    private Vector3 ApplyFriction(Vector3 velocity)
    {
        if(groundSensor.IsGrounded && deltaGroundFrictionTimeout > 0)
        {
            deltaGroundFrictionTimeout -= Time.deltaTime;
            return velocity;
        }

        Vector3 frictionVector;    
    
        if(wallSensor.IsNextToWall && !groundSensor.IsGrounded)
        {
            frictionVector = wallFriction * Project(velocity);
        }else if(groundSensor.IsGrounded)
        {
            frictionVector = currentHorizontalVelocity * groundFriction;

        }else frictionVector = Vector3.zero;

        return velocity - frictionVector;
    }

    private Vector3 ApplyVector(ref Vector3 appliedVector, Vector3 velocity)
    {
        velocity += (groundSensor != null && groundSensor.IsGrounded)? appliedVector : appliedVector/(mass / 10);
        appliedVector = Vector3.zero;
        return velocity;
    }

    private Vector3 CalculateDirection(Vector2 inputDirection)
    {
        // 1. If no input, maintain last direction
        if (inputDirection.sqrMagnitude < 0.01f) return lastDirection;



        // 4. Project them onto the plane perpendicular to gravity (the "floor")
        Vector3 planarForward = ProjectOnPlane(transform.forward);
        Vector3 planarRight = ProjectOnPlane(transform.right);

        // 5. Edge-case safeguard: Looking exactly along the gravity axis (prevents zero-vectors)
        if (planarForward.sqrMagnitude < 0.01f)
        {
            planarForward = ProjectOnPlane(transform.up);
        }
        if (planarRight.sqrMagnitude < 0.01f)
        {
            planarRight = Vector3.Cross((currentGravity != Vector3.zero)? currentGravity.normalized : -transform.up, planarForward.normalized);
        }

        // 6. Build the movement direction using the 2D input
        // inputDirection.x is Strafe (A/D), inputDirection.z is Forward/Back (W/S)
        Vector3 targetDirection = (planarRight.normalized * inputDirection.x) + (planarForward.normalized * inputDirection.y);


        return targetDirection.normalized;
    }


    private Vector3 GetCurrentHorizontalVelocity() => ProjectOnPlane(characterController.velocity);

    private Vector3 GetCurrentVerticalVelocity() => Project(characterController.velocity);

    private Vector3 Project(Vector3 input)
    {
        return Vector3.Project(input, (currentGravity != Vector3.zero)? currentGravity.normalized : -transform.up);
    }

    private Vector3 ProjectOnPlane(Vector3 input)
    {
        return Vector3.ProjectOnPlane(input, (currentGravity != Vector3.zero)? currentGravity.normalized : -transform.up);
    }


    private IEnumerator SlideC()
    {
        float deltaSlideTime = slideTime;
        while (deltaSlideTime > 0 && groundSensor.IsGrounded)
        {
            additionalMovementVector = transform.forward.normalized * slideSpeed;

            deltaSlideTime -= Time.deltaTime;

            yield return null;
        }

        isSliding = false;

        slidingCoroutine = null;
    }

    private bool TryStandUp()
    {
        Vector3 headPoint = new (0, characterController.center.y  + characterController.height / 2, 0);
        Vector3 worldHeadPoint = characterController.transform.TransformPoint(headPoint);
        Debug.DrawRay(worldHeadPoint, transform.up, Color.blue, 50);
        
        if(Physics.Raycast(worldHeadPoint, transform.up, out RaycastHit hitInfo))
        {
            if(hitInfo.collider.gameObject.layer == gameObject.layer) return true;
            else
            {
                Debug.Log(hitInfo.collider.name);
                return false;
            }
        }else return true;

    }

    public void ToggleSprint(bool value)
    {
        isSprinting = value && groundSensor.IsGrounded;
    }

    public void SetGravity(Vector3 gravityForce)
    {    
        if(currentGravity != gravityForce)
        {
            currentGravity = gravityForce;
            groundSensor.ChangeGroundDirection(gravityForce.normalized);
            
            transform.rotation = Quaternion.FromToRotation(transform.up, -gravityForce.normalized) * transform.rotation;


            if(Project(Vector3.one) != Vector3.up)
            {
                characterController.height = 0.5f * playerHeight;
                characterController.center = new Vector3(0, -characterController.height / 2, 0);
            }
        }

    }

    private void ApplyGravity()
    {
        verticalVelocity += ((wallSensor.IsNextToWall)? mass/2 :mass) * Time.deltaTime * 0.1f * currentGravity;

        if( groundSensor != null && groundSensor.IsGrounded && Vector3.Dot(verticalVelocity.normalized, currentGravity.normalized) > 0.9f) verticalVelocity = verticalVelocity.normalized * 2;
    }


    public void SetKnockback(Vector3 knockback)
    {
         knockbackVector = knockback;
    }
}

