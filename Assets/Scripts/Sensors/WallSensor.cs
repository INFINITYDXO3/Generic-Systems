using UnityEngine;

public class WallSensor : MonoBehaviour
{
    [SerializeField]
    private float wallCheckOffset = 0;
    
    [SerializeField]
    private float wallCheckRadius = 0.6f;
    
    [SerializeField]
    private LayerMask wallLayers = 1; 

    private readonly RaycastHit[] wallHits = new RaycastHit[8];
    private readonly Vector3[] _castDirections = new[] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
    
    private Vector3 wallSpherePosition;
    private Vector3 wallNormal;

    private bool isNextToWall;

    public Vector3 WallNormal {get => wallNormal;}

    public bool IsNextToWall {get => isNextToWall;}
    
    void FixedUpdate()
    {
        WallCheck();    
    }


    private void WallCheck()
    {
        wallSpherePosition = new Vector3(
            transform.position.x, 
            transform.position.y - wallCheckOffset, 
            transform.position.z
        );

        isNextToWall = false;
        wallNormal = Vector3.up;
        float closestDist = float.MaxValue;

        for (int i = 0; i < _castDirections.Length; i++)
        {
            int hitCount = Physics.SphereCastNonAlloc(
                wallSpherePosition,
                wallCheckRadius * 0.5f,        
                _castDirections[i],
                wallHits,
                wallCheckRadius,
                wallLayers
            );

            for (int j = 0; j < hitCount; j++)
            {
                RaycastHit hit = wallHits[j];
                if (hit.collider == null || hit.collider.isTrigger) continue;

                // Filter out floors (<45°) and ceilings (>135°)
                float angleToUp = Vector3.Angle(hit.normal, Vector3.up);
                if (angleToUp < 45f || angleToUp > 135f) continue;

                // Keep the closest valid wall
                if (hit.distance < closestDist)
                {
                    closestDist = hit.distance;
                    wallNormal = hit.normal;
                    isNextToWall = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    { 
        if(wallSpherePosition == Vector3.zero) wallSpherePosition = new Vector3(transform.position.x, transform.position.y - wallCheckOffset, transform.position.z);
        Gizmos.color = Color.aliceBlue;
        
        Gizmos.color = Color.red;
        Gizmos.DrawCube(wallSpherePosition, Vector3.one * wallCheckRadius);
        Gizmos.DrawRay(wallSpherePosition + (new Vector3(0, 0, 1f) * wallCheckRadius / 2), new Vector3(0, 0, wallCheckRadius));

        
        //Gizmos.DrawSphere(wallSpherePosition, wallCheckRadius);

    }
}
