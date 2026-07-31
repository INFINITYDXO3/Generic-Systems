using UnityEngine;
public interface IRaycastMechanic
{
    void PerformRaycast(Transform origin, Vector3 direction, float range, System.Action<WeaponHitResult> onHit);
}