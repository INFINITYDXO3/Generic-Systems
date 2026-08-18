using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName ="Weapons System/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [HideInInspector]
    public WeaponsTypes WeaponType;
    
    [HideInInspector]
    public float Damage;

    [HideInInspector]
    public RecoilData RecoilData;

    [HideInInspector]
    public float Knockback;
    
    [HideInInspector]
    public float SpreadAngle;
    
    [HideInInspector]
    public float Range;
    
    [HideInInspector]
    public float FireRate;
    
    [HideInInspector]
    public float ReloadTime;
    
    [HideInInspector]
    public BulletType BulletType;
    
    [HideInInspector]
    public int MagSize;
}
