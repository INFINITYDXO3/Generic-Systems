using UnityEngine;

public class GravityVolume : MonoBehaviour
{
    [SerializeField]
    private Vector3 gravityForce;

    void OnTriggerStay(Collider other)
    {
        if(other.TryGetComponent(out IGravityAffected gravityAffected))
        {
            gravityAffected.SetGravity(gravityForce);
        }
    }
}
