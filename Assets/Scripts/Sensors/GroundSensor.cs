using UnityEngine;

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

    private Vector3 groundedSpherePosition;

    
    private bool isGrounded;

    public bool IsGrounded {get => isGrounded;}
    
    private void FixedUpdate()
    {
        GroundedCheck();
    }


    private void GroundedCheck()
    {
        groundedSpherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        isGrounded = Physics.BoxCast(groundedSpherePosition, (new Vector3(1, 0.1f, 1) * groundedBoxSize) / 2, Vector3.down, Quaternion.identity, groundedRayMaxDistance, groundLayers);
    }

   

    private void OnDrawGizmos()
    { 
        groundedSpherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        Gizmos.color = Color.aliceBlue;


        Gizmos.DrawCube(groundedSpherePosition, new Vector3(1, 0.1f, 1) * groundedBoxSize);
        Gizmos.DrawLine(groundedSpherePosition + (new Vector3(0, 0.1f, 0) * groundedBoxSize)/2, groundedSpherePosition + (new Vector3(0, 0.1f, 0)* groundedBoxSize)/2 - new Vector3(0, groundedRayMaxDistance, 0));
    
    }
}
