using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public abstract class CharactersHandler : NetworkBehaviour, IGravityAffected
{
    [Header("Character Components")]

    [SerializeField] protected MovementSystem movementSystem;
    
    [SerializeField] protected WeaponsHandler weaponsHandler;
    [SerializeField] protected AimController aimController;
    [SerializeField] protected Inventory inventory;
    [SerializeField] protected HealthSystem healthSystem;

    [Header("Other")]
    [SerializeField] protected Transform hand;


    public bool IsDead {get; private set;} = false; 

    protected virtual void OnEnable()
    {
        if(inventory != null && !IsDead) inventory.OnItemsChanged += CheckWeapons;
    }

    protected virtual void OnDisable()
    {
        if(inventory != null && !IsDead) inventory.OnItemsChanged -= CheckWeapons;
    }

    protected virtual void Update()
    {
        IsDead = healthSystem.IsDead;
    }

    protected void CheckWeapons()
    {
        if(inventory == null || IsDead) return;

        List<Weapon> weapons = inventory.InventoryItems.Select(item => item as Weapon).ToList();

        InitWeapons(weapons);
    }

    public void InitWeapons(List<Weapon> weapons)
    {
        if(weaponsHandler == null || IsDead) return;

        weaponsHandler.InitWeapons(weapons);
    }

    public virtual void ProcessMove(Vector2 motion)
    {
        if(movementSystem == null || IsDead) return;
        
        movementSystem.SetInput(motion);
    }

    public virtual void PerformJump()
    {
        if(movementSystem == null || IsDead) return;
        
        movementSystem.Jump();
    }

    public virtual void ToggleSprint(bool value)
    {
        if(movementSystem == null || IsDead) return;
        
        movementSystem.ToggleSprint(value);
    }

    public virtual void ToggleCrouch(bool value)
    {
        if(movementSystem == null || IsDead) return;
        
        movementSystem.ToggleCrouch(value);
    }

    public void TakeKnockback(Vector3 knockback)
    {
        if(movementSystem == null || IsDead) return;
        
        movementSystem.SetKnockback(knockback);
    }

    

    public virtual void ToggleAttack(bool isAttacking)
    {        
        if(weaponsHandler == null || IsDead) return;
        
        weaponsHandler.ToggleAttackStatus(isAttacking);
    }

    public void Reload()
    {
        if(weaponsHandler == null || IsDead) return;
        
        weaponsHandler.ReloadCurrentWeapon();
    }

    public void NextWeapon()
    {
        if(weaponsHandler == null || IsDead) return;
        
        weaponsHandler.NextWeapon();
    }

    public void SetAim(Aim aim)
    {
        if(aimController == null || IsDead) return;

        aimController.SetAim(aim);

        if(hand != null)
        {
            Vector3 direction = aim.Direction;
            Quaternion rotation = Quaternion.LookRotation(direction);
            hand.rotation = rotation;
            hand.localEulerAngles = new Vector3(hand.localEulerAngles.x, 0, 0);
        }

    }

    public void SetGravity(Vector3 gravity)
    {
        if(movementSystem == null || IsDead) return;

        movementSystem.SetGravity(gravity);
    }


    public void Damage(float damage)
    {
        if(healthSystem == null || IsDead) return;

        healthSystem.Damage(damage);
    }

    public void Heal(float heal)
    {
        if(healthSystem == null || IsDead) return;
        
        healthSystem.Heal(heal);
    }

    [ContextMenu("Damage")]
    public void Damage()
    {
        Damage(10);
    }


    public HealthSystem HealthSystem => healthSystem != null ? healthSystem : null;

    public float Speed => movementSystem != null ? movementSystem.HorizontalSpeed : 0f;

    public Weapon CurrentWeapon => (weaponsHandler != null && weaponsHandler.CurrentWeapon != null)? weaponsHandler.CurrentWeapon : null;

}
