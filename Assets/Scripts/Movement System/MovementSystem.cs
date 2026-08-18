using System;
using System.Collections;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private CharacterController characterController;

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

    private Vector3 inputVector;
    private Vector3 velocity;
    private Vector3 knockbackVector;
    private Vector3 verticalVelocity;
    private Vector3 lastDirection;
    private Vector3 additionalMovementVector;

    private FrictionSurface currentFrictionSurface;

    public float Speed {get; private set;}

    private bool isSprinting;
    private bool isSliding;
    private bool isCrouching;
    private bool isJumping;
    private bool jumpSafeControl;

    private float currentGravity;
    float currentHorizontalSpeed;
    private float deltaGroundFrictionTimeout;
    private float crouchHeight {get => playerHeight * crouchPercentage;}


    private int wallJumps;

    private Coroutine slidingCoroutine;

    private void Start()
    {
        characterController.height = playerHeight;
        characterController.center = playerCenter;
    }


    private void FixedUpdate()
    {
        if(groundSensor.IsGrounded) wallJumps = 0;
    }

    public void Move(Vector2 moveVector)
    {
        Vector3 inputDirection = new Vector3(moveVector.x, 0.0f, moveVector.y).normalized;

        Vector3 targetDirection = CalculateDirection(inputDirection);
        
        currentHorizontalSpeed = (currentHorizontalSpeed - GetCurrentHorizontalSpeed() == currentHorizontalSpeed)? currentHorizontalSpeed : GetCurrentHorizontalSpeed();
        CalculateSpeed(inputDirection, targetDirection);        

        velocity = inputVector + verticalVelocity;

        velocity = ApplyVector(ref knockbackVector, velocity);
        velocity = ApplyVector(ref additionalMovementVector, velocity);
        velocity = ApplyFriction(velocity);

        if(velocity.magnitude > terminalVelocity)
        {
            velocity *= terminalVelocity/velocity.magnitude;
        }


        characterController.Move(Time.deltaTime * velocity);

        lastDirection = (velocity != Vector3.zero)?  velocity.normalized : targetDirection;

    }

    public void Jump(bool value)
    {
        if(!value) jumpSafeControl = false;

        if(value == isJumping || jumpSafeControl) return;

        if (value)
        {
            if (groundSensor.IsGrounded )
            {
                verticalVelocity.y = jumpForce;

            } else if (wallSensor.IsNextToWall && wallJumps < maxWallJumps)
            {
                verticalVelocity.y = jumpForce;
                
                if(wallSensor != null) additionalMovementVector = wallSensor.WallNormal * jumpForce;

                wallJumps++;
            }

            deltaGroundFrictionTimeout = groundFrictionTimeout;
        }
        

        isJumping = value;
        jumpSafeControl = true;

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
            frictionVector = new (0, wallFriction * verticalVelocity.y, 0);
        }else if(groundSensor.IsGrounded)
        {
            frictionVector = new (groundFriction * velocity.x, 0, groundFriction * velocity.z);

        }else frictionVector = Vector3.zero;

        return velocity - frictionVector;
    }

    private Vector3 ApplyVector(ref Vector3 appliedVector, Vector3 velocity)
    {
        velocity += (characterController.isGrounded)? appliedVector : appliedVector/(mass / 10);
        appliedVector = Vector3.zero;
        return velocity;
    }

    private Vector3 CalculateDirection(Vector3 inputDirection)
    {
        float _targetRotation = 0.0f;

        if (inputDirection != Vector3.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + transform.eulerAngles.y;
        }


        Vector3 targetDirection = (inputDirection == Vector3.zero) ? lastDirection : Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        return targetDirection.normalized;
    }


    private float GetCurrentHorizontalSpeed()
    {
        return new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;
    }

    private void CalculateSpeed(Vector3 inputDirection, Vector3 targetDirection)
    {
        if(inputDirection == Vector3.zero && !groundSensor.IsGrounded && !wallSensor.IsNextToWall)
        {
            inputVector = new (characterController.velocity.x, 0, characterController.velocity.z);
            return;
        }

        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;

        if(!characterController.isGrounded) inputDirection *= airSpeed;

        inputVector = Vector3.Lerp(inputVector, targetSpeed * inputDirection.magnitude * targetDirection, speedChangeRate * Time.deltaTime);
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

        isSprinting = value && groundSensor.IsGrounded;
    }

    public void ApplyGravity(float gravityForce)
    {
        gravityForce *= Time.timeScale;
        currentGravity = gravityForce;

        verticalVelocity += Time.fixedDeltaTime * gravityForce * ((wallSensor.IsNextToWall)? mass/2 :mass)  * transform.up;

        if(groundSensor.IsGrounded) verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, -2, 100);
        
    }


    public void SetKnockback(Vector3 knockback)
    {
         knockbackVector = knockback;
    }
}
