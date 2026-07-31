using UnityEngine;

public class RaycastManager
{
    public static bool PerformRaycast(Aim aim, out RaycastHit raycastHit , float maxDistance, int layerMask, bool debugRay = false)
    {
        Ray ray = new (aim.OriginPoint, aim.Direction);
        
        if(debugRay) Debug.DrawRay(aim.OriginPoint, aim.Direction, Color.red, 5);

        return Physics.Raycast(ray, out raycastHit, maxDistance, layerMask);
    }
}
