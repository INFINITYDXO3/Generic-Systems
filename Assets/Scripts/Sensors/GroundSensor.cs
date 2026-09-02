using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class GroundSensor : MonoBehaviour
{

    [SerializeField]
    private float groundedOffset = 0.25f;

    [SerializeField]
    private float groundedBoxSize = 1.3f;

    [SerializeField]
    private float groundedRayMaxDistance = 1.2f;

    [SerializeField]
    private LayerMask groundLayers = 1;

    private Vector3 groundedBoxPosition;
    private Vector3 groundedBoxScale;
    private Vector3 groundedRayDirection;
    private Vector3 groundDirection = Vector3.down;

    
    private bool isGrounded;
    private bool lockDownDirection;

    public bool IsGrounded {get => isGrounded;}

    private void FixedUpdate()
    {
        if (lockDownDirection)
        {
            groundDirection = -transform.up;
        }

        GroundedCheck();
    }


    private void GroundedCheck()
    {
        UpdateGroundedBoxValues();
        isGrounded = Physics.BoxCast(groundedBoxPosition, groundedBoxScale/2 , groundedRayDirection, Quaternion.identity, groundedRayMaxDistance, groundLayers);
    }

   
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    { 
        if(!EditorApplication.isPlaying) UpdateGroundedBoxValues();
        Gizmos.color = Color.aliceBlue;


        Gizmos.DrawCube(groundedBoxPosition, groundedBoxScale);
        Gizmos.DrawRay(Vector3.ProjectOnPlane(groundedBoxPosition, groundDirection) + (Vector3.Project(groundedBoxPosition, groundDirection) - Vector3.Project(groundedBoxScale, groundDirection)/2), groundedRayDirection * groundedRayMaxDistance);
    
    }
    #endif

    private void UpdateGroundedBoxValues()
    {
        // Debug.Log(transform.up);
        Vector3 offset = Vector3.Project(transform.position.normalized, groundDirection) * -groundedOffset;
        groundedBoxPosition = transform.position - offset;
        groundedBoxScale = (Vector3.ProjectOnPlane(Vector3.one, groundDirection) + Vector3.Project(Vector3.one, groundDirection) * 0.1f) * groundedBoxSize;
        groundedRayDirection = groundDirection;
    }

    public void ChangeGroundDirection(Vector3 direction)
    {
        if(direction != Vector3.zero)
        {
            groundDirection = direction;
            lockDownDirection = false;
        }
        else
        {
            groundDirection = -transform.up;
            lockDownDirection = true;
        }
    }
}
