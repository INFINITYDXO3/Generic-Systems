using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponsHandler : NetworkBehaviour
{
    [SerializeField] private AimController aimController;

    [SerializeField] private RecoilEffect recoilEffect;

    
    private List<Weapon> weapons = new ();

    [SyncVar(hook = nameof(OnWeaponChanged))]
    private Weapon _currentWeapon;
    public Weapon CurrentWeapon {get => _currentWeapon;}

    

    private void OnWeaponChanged(Weapon oldWeapon, Weapon newWeapon)
    {
        if(isLocalPlayer) return;
        
        if(oldWeapon != null) DeInitCurrentWeapon(oldWeapon);
        if(newWeapon != null) InitCurrentWeapon(newWeapon);
    }

    public void InitWeapons(List<Weapon> weapons)
    {
        if (weapons.Count == 0) return;

        this.weapons = weapons;
        foreach(var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }
        
        InitCurrentWeapon(this.weapons[0]);
    }

    private void OnCurrentWeaponAttack()
    {
        recoilEffect.ApplyRecoil(_currentWeapon.Data.RecoilData);
    }

    private void InitCurrentWeapon(Weapon weapon)
    {
        if(_currentWeapon != null)
        {
            DeInitCurrentWeapon(_currentWeapon);
        }
        
        _currentWeapon = weapon;
        _currentWeapon.gameObject.SetActive(true);
        if(aimController != null) _currentWeapon.SetAimController(aimController);
        if(recoilEffect != null) _currentWeapon.onWeaponAttack.AddListener(OnCurrentWeaponAttack);
    }


    private void DeInitCurrentWeapon(Weapon weapon)
    {
        weapon.gameObject.SetActive(false);
        if(aimController != null) weapon.SetAimController(null);
        if(recoilEffect != null) weapon.onWeaponAttack.RemoveListener(OnCurrentWeaponAttack);
    }



    public void ToggleAttackStatus(bool isAttacking)
    {
        if(_currentWeapon == null) return;
        
        _currentWeapon.ToggleAttackStatus(isAttacking);
    }

    internal void ReloadCurrentWeapon()
    {
        _currentWeapon.OnReloadStarted();
    }

    [ContextMenu("NextWeapon")]
    public void NextWeapon()
    {
        int index = weapons.IndexOf(_currentWeapon);
        if(index == weapons.Count - 1) index = 0;
        else index++;
        
        SelectWeapon(index);
    }

    [ContextMenu("PreviousWeapon")]
    public void PreviousWeapon()
    {
        int index = weapons.IndexOf(_currentWeapon);
        if(index == 0) index = weapons.Count - 1;
        else index--;
        
        SelectWeapon(index);
    }

    public void SelectWeapon(int index)
    {
        Debug.Log(transform);
        InitCurrentWeapon(weapons[index]);
    }
}
