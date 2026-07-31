using UnityEngine;

public class GravityVolume : MonoBehaviour
{
    [SerializeField]
    private float gravityForce = -9.8f;

    void OnTriggerStay(Collider other)
    {
        if(other.TryGetComponent(out MovementSystem movementSystem))
        {
            movementSystem.ApplyGravity(gravityForce / DebugInfo.GetDetailedCurrentFPS());
        }
    }
}
