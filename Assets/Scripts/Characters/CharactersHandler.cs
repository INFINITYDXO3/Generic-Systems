using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CharactersHandler : MonoBehaviour
{
    [Header("Character Components")]

    [SerializeField] protected MovementSystem movementSystem;
    [SerializeField] protected CameraSystem cameraSystem;
    [SerializeField] protected WeaponsHandler weaponsHandler;
    [SerializeField] protected AimController aimController;
    [SerializeField] protected Inventory inventory;


    protected virtual void OnEnable()
    {
        if(inventory != null) inventory.OnItemsChanged += CheckWeapons;
    }

    protected virtual void OnDisable()
    {
        if(inventory != null) inventory.OnItemsChanged -= CheckWeapons;
    }

    protected void CheckWeapons()
    {
        if(inventory == null) return;

        List<Weapon> weapons = inventory.InventoryItems.Select(item => item as Weapon).ToList();

        InitWeapons(weapons);
    }

    public void InitWeapons(List<Weapon> weapons)
    {
        if(weaponsHandler == null) return;

        weaponsHandler.InitWeapons(weapons);
    }

    public virtual void ProcessMove(Vector2 motion)
    {
        if(movementSystem == null) return;
        
        movementSystem.Move(motion);
    }

    public virtual void PerformJump(bool value)
    {
        if(movementSystem == null) return;
        
        movementSystem.Jump(value);
    }

    public virtual void ToggleSprint(bool value)
    {
        if(movementSystem == null) return;
        
        movementSystem.Sprint(value);
    }

    public virtual void ProcessLook(Vector2 lookInput)
    {
        if(cameraSystem == null) return;
        
        cameraSystem.RotateCamera(lookInput);
    }

    internal void ApplyCameraEffect(CameraEffectsType effectType, bool isOn)
    {
        if(cameraSystem == null) return;
        
        cameraSystem.ApplyCameraEffect(effectType, isOn);
    }

    public virtual void ToggleAttack(bool isAttacking)
    {        
        if(weaponsHandler == null) return;
        
        weaponsHandler.ToggleAttackStatus(isAttacking);
    }

    public void Reload()
    {
        if(weaponsHandler == null) return;
        
        weaponsHandler.ReloadCurrentWeapon();
    }

    public void NextWeapon()
    {
        if(weaponsHandler == null) return;
        
        weaponsHandler.NextWeapon();
    }

    public void SetAim(Aim aim)
    {
        if(aimController == null) return;

        aimController.SetAim(aim);
    }

}
