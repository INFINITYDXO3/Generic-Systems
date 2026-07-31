using System.Collections.Generic;
using UnityEngine;

public class WeaponsHandler : MonoBehaviour
{
    [SerializeField] private AimController aimController;

    [SerializeField] private RecoilEffect recoilEffect;

    private List<Weapon> weapons = new ();

    private Weapon _currentWeapon;
    public Weapon CurrentWeapon {get => _currentWeapon;}

    

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
            DeInitCurrentWeapon();
        }
        _currentWeapon = weapon;
        _currentWeapon.gameObject.SetActive(true);
        if(aimController != null) _currentWeapon.SetAimController(aimController);
        if(recoilEffect != null) _currentWeapon.onWeaponAttack.AddListener(OnCurrentWeaponAttack);
    }


    private void DeInitCurrentWeapon()
    {
        _currentWeapon.gameObject.SetActive(false);
        if(aimController != null) _currentWeapon.SetAimController(null);
        if(recoilEffect != null) _currentWeapon.onWeaponAttack.RemoveListener(OnCurrentWeaponAttack);
        _currentWeapon = null;
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
        InitCurrentWeapon(weapons[index]);
    }
}
