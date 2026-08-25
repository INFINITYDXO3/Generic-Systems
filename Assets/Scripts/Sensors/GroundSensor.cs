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

    
    private bool isGrounded;

    public bool IsGrounded {get => isGrounded;}
    
    private void FixedUpdate()
    {
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
        Gizmos.DrawRay(Vector3.ProjectOnPlane(groundedBoxPosition, -transform.up) + (Vector3.Project(groundedBoxPosition, -transform.up) - Vector3.Project(groundedBoxScale, -transform.up)/2), groundedRayDirection * groundedRayMaxDistance);
    
    }
    #endif

    private void UpdateGroundedBoxValues()
    {
        // Debug.Log(transform.up);
        Vector3 offset = Vector3.Project(transform.position.normalized, -transform.up) * -groundedOffset;
        groundedBoxPosition = transform.position - offset;
        groundedBoxScale = (Vector3.ProjectOnPlane(Vector3.one, -transform.up) + Vector3.Project(Vector3.one, -transform.up) * 0.1f) * groundedBoxSize;
        groundedRayDirection = -transform.up;
    }
}
