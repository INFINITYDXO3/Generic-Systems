using UnityEngine;

public class BulletHolesPoolingManger : GlobalPoolingManager
{
    public static BulletHolesPoolingManger Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }else if(Instance != this)
        {
            Destroy(gameObject);
        }        
    }

    public void SetBulletHole(Vector3 position, Vector3 normal)
    {
        GameObject bulletHole = BulletHolesPoolingManger.Instance.GetObject();
        bulletHole.transform.SetPositionAndRotation(position, Quaternion.LookRotation(normal));
    }
}
