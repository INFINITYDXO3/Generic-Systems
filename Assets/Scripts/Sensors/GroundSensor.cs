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
        isGrounded = Physics.BoxCast(groundedSpherePosition, ((transform.position - 0.9f * -transform.up) * groundedBoxSize) / 2, -transform.up, Quaternion.identity, groundedRayMaxDistance, groundLayers);
    }

   

    private void OnDrawGizmos()
    { 
        groundedSpherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        Gizmos.color = Color.aliceBlue;


        Gizmos.DrawCube(groundedSpherePosition, (transform.position.normalized - 0.9f * -transform.up) * groundedBoxSize);
        Gizmos.DrawLine(groundedSpherePosition + ((transform.position - 0.9f * -transform.up) * groundedBoxSize) / 2, ((transform.position - 0.9f * -transform.up) * groundedBoxSize) / 2 - (-transform.up * groundedRayMaxDistance));
    
    }
}
