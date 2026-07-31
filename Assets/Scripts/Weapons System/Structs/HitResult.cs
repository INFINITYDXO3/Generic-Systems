using UnityEngine;
public struct WeaponHitResult
{
    public Vector3 hitPoint;
    public Vector3 hitNormal;
    public Collider hitCollider;
    public float distance;

    public WeaponHitResult(Vector3 point, Vector3 normal, Collider collider, float dist)
    {
        hitPoint = point;
        hitNormal = normal;
        hitCollider = collider;
        distance = dist;
    }
}