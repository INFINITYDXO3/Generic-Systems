using UnityEngine;
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour, IGravityAffected
{
    [SerializeField] LayerMask exclusionMasks;

    [Header("Movement")]
    [SerializeField] private float stepHeight = 0.4f;
    [SerializeField] private float slopeLimit = 45f;
    [SerializeField] private float skinWidth = 0.05f;

    [Header("Sensors")]
    [SerializeField] private GroundSensor groundSensor;
    [SerializeField] private WallSensor wallSensor;

    private LayerMask includedMasks;
    private readonly Collider[] overlapedColliders = new Collider[20];

    private Rigidbody rb;
    private CapsuleCollider col;
    private Vector3 velocity;
    private Vector3 gravity;
    private Vector3 input;
    private bool _physicalIsGrounded;

    public bool PhysicalIsGrounded {get => _physicalIsGrounded;}
    public bool RaycastIsGrounded {get => groundSensor.IsGrounded;}

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        
        rb.isKinematic = true;

        includedMasks = ~exclusionMasks;
    }

    public void ApplyImpulse(Vector3 impulse) { if (impulse.sqrMagnitude > 0.001f) velocity += impulse; }

    private void FixedUpdate()
    {
        GroundCheck();
        Move();
    }

    private void Move()
    {
        GetCapsulePoints(out Vector3 pointBottom, out Vector3 pointTop);

        if(Physics.OverlapCapsuleNonAlloc(pointBottom, pointTop, col.radius, overlapedColliders, includedMasks) != 0)
        {
            
            foreach(Collider otherCollider in overlapedColliders)
            {
                if(otherCollider == null || otherCollider.isTrigger) continue;
                Debug.Log("here " + otherCollider.name);

                    // 1. Predictive sweep (replaces CC's built-in collision)
                if (Physics.ComputePenetration(col, transform.position, transform.rotation,
                    otherCollider, otherCollider.gameObject.transform.position, otherCollider.gameObject.transform.rotation, out Vector3 hitDir, out float hitDist))
                {
                    // 4. Collision response (push out + slide)
                    Vector3 pushOut = hitDir * (hitDist + skinWidth);
                    rb.MovePosition(rb.position + pushOut);
                    velocity = Vector3.ProjectOnPlane(velocity, hitDir); // Slide along surface

                }
            }
        }else
        {

            velocity = rb.position + input + gravity * Time.deltaTime;
            Debug.Log(velocity.magnitude);
            // 3. Apply movement
            rb.MovePosition(velocity);
        }
            
       
    }

    public void Move(Vector3 input)
    {
        this.input = input;
    }

    private void GroundCheck()
    {
        GetCapsulePoints(out Vector3 pointBottom, out Vector3 pointTop);

        _physicalIsGrounded = Physics.CheckSphere(pointBottom, 0.1f);
    }

    public void SetGravity(Vector3 gravity)
    {
        this.gravity = gravity;
    }

    // Expose state for other systems
    public bool IsGrounded => _physicalIsGrounded;
    public Vector3 Velocity => velocity;


    private void GetCapsulePoints(out Vector3 pointBottom, out Vector3 pointTop)
    {
        // Account for custom center offset set in the inspector
        Vector3 localCenter = col.center;
        
        // Calculate half the distance between the sphere centers
        // Formula: (Height / 2) - Radius
        // For height 2, radius 0.5: (2 / 2) - 0.5 = 0.5
        float halfCylinderHeight = Mathf.Max(0f, (col.height * 0.5f) - col.radius);

        Vector3 localBottom = localCenter;
        Vector3 localTop = localCenter;

        // Determine the capsule's orientation axis (0 = X, 1 = Y, 2 = Z)
        // Standard Unity capsules use the Y-Axis (1)
        if (col.direction == 1) 
        {
            localBottom.y -= halfCylinderHeight;
            localTop.y += halfCylinderHeight;
        }
        else if (col.direction == 0) // X-Axis
        {
            localBottom.x -= halfCylinderHeight;
            localTop.x += halfCylinderHeight;
        }
        else // Z-Axis
        {
            localBottom.z -= halfCylinderHeight;
            localTop.z += halfCylinderHeight;
        }

        // Transform the local points into absolute World Space (handles object rotation/position)
        pointBottom = transform.TransformPoint(localBottom);
        pointTop = transform.TransformPoint(localTop);
    }

    void OnDrawGizmos()
    {
        if(col == null) col = GetComponent<CapsuleCollider>();
        GetCapsulePoints(out Vector3 pointBottom, out Vector3 pointTop);

        Gizmos.DrawSphere(pointBottom, 0.1f);
        Gizmos.DrawSphere(pointTop, 0.1f);
    }


}