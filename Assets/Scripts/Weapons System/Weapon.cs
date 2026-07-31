using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : Item
{
    [Header("Weapon's Data")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private Transform muzzle;

    
    [Header("Weapon's Effects")]
    [SerializeField] private ParticleSystem attackParticles;
    [SerializeField] private WeaponAnimation weaponAnimation;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] emptyMagSounds;


    [Header("Other")]
    [Tooltip("The actions that trigger on attack like camera shake")] public UnityEvent onWeaponAttack;


    public WeaponData Data {get => weaponData;}

    private Coroutine attackCoroutine;
    private AimController aimController;

    private bool isFiring = false;
    private bool isReloading = false;

    private int currentBulletsCount;


    protected virtual void Start()
    {
        InitWeapon();
    }

    protected virtual void InitWeapon()
    {
        currentBulletsCount = weaponData.MagSize;
    }

    protected virtual void OnEnable()
    {
        if(attackCoroutine != null) attackCoroutine =  StartCoroutine(AttackC());
        if(weaponAnimation != null) weaponAnimation.PlayAnimation(WeaponAnimation.IDLE);
        
        if (isReloading)
        {
            isReloading = false;
        }
    }

    protected virtual void OnDisable()
    {
        if(attackCoroutine != null) 
        {
            StopCoroutine(AttackC());
            attackCoroutine = null;
        }
    }

    public void ToggleAttackStatus(bool isAttacking)
    {
        if(isReloading) return;
        
        isFiring = isAttacking;
            
        if (isFiring) Attack();
    }

    public void OnReloadStarted()
    {
        if(!CanReload()) return;
        
        isReloading = true;
        if(weaponAnimation != null) weaponAnimation.PlayAnimation(WeaponAnimation.Reload);
        else OnReloadFinished();
        
    }

    public void OnReloadFinished()
    {
        currentBulletsCount = weaponData.MagSize;
        isReloading = false;
    }

    private void Attack()
    {
        if(attackCoroutine == null) attackCoroutine = StartCoroutine(AttackC());         
    }

    private IEnumerator AttackC()
    {
        float time = 1 / weaponData.FireRate;

        if(weaponData.WeaponType == WeaponsTypes.Melee)
        {
            Hit();
        }else if(CanAttack()) 
        {
            Shoot();
        }else OnWeaponEmpty();

        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        if(weaponData.WeaponType != WeaponsTypes.Full_Auto) yield return new WaitUntil(() => !isFiring);
        attackCoroutine = null;
    }

    public bool CanAttack()
    {
        return currentBulletsCount > 0 && !isReloading;
    }

    public bool CanReload()
    {
        return !isFiring && currentBulletsCount != weaponData.MagSize && !isReloading;
    }
    
    private void Hit()
    {
        
    }

    private void Shoot()
    {
        Aim aim;
        if(aimController != null && aimController.GetAim() != Aim.zero)
        {
            aim = new (aimController.GetAim().OriginPoint, GetSpreadDirection(aimController.GetAim().Direction));
        }
        else if(muzzle != null)
        {
            aim = new (muzzle.transform.position, GetSpreadDirection(muzzle.transform.forward));
        }else return;
         
        
        AttackEffects();
        currentBulletsCount = Mathf.Clamp(currentBulletsCount - 1, 0, weaponData.MagSize);

        if (RaycastManager.PerformRaycast(aim, out RaycastHit hit, weaponData.Range, targetMask, true))
        {
            OnTargetHit(new WeaponHitResult(hit.point, hit.normal, hit.collider, hit.distance));
        }
    }


    private void AttackEffects()
    {
        onWeaponAttack?.Invoke();
        PlayRandomAudio(attackSounds);
        if(weaponAnimation != null) weaponAnimation.PlayAnimation(WeaponAnimation.ATTACK);

        if (attackParticles)
        {
            attackParticles.Play();
        }
    }

    private void OnWeaponEmpty()
    {
        PlayRandomAudio(emptyMagSounds);
    }

    private void PlayRandomAudio(AudioClip[] sounds)
    {
        if (audioSource && attackSounds != null && attackSounds.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, sounds.Length);
            AudioClip randomClip = sounds[randomIndex];

            audioSource.PlayOneShot(randomClip);
        }
    }

    private void OnTargetHit(WeaponHitResult hitResult)
    {
        if(hitResult.hitCollider == null ) return;
        if(BulletHolesPoolingManger.Instance != null)
        { 
            BulletHolesPoolingManger.Instance.SetBulletHole(hitResult.hitPoint, hitResult.hitNormal);
        }
    }

    private Vector3 GetSpreadDirection(Vector3 direction)
    {
        return Quaternion.Euler(UnityEngine.Random.Range(-weaponData.SpreadAngle, weaponData.SpreadAngle),
         UnityEngine.Random.Range(-weaponData.SpreadAngle, weaponData.SpreadAngle), 0f) * direction;
    }

    public void SetAimController(AimController aimController)
    {
        this.aimController = aimController;
    }

    public override void UseItem()
    {
        throw new NotImplementedException();
    }
}
