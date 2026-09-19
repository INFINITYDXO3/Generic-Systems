using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponElement : PlayerUIElement
{
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text maxAmmoText;
    [SerializeField] private TMP_Text weaponName;
    [SerializeField] private Image weaponIcon;

    private Weapon value;

    public override void SetValue<T>(T value)
    {
        if(value is Weapon weapon)
        {
            this.value = weapon;
            weaponIcon.sprite = this.value.Data.WeaponIcon;
        }
    }

    protected override void Awake()
    {
        SetElementType(ElementType.Weapon);
    }

    private void Update()
    {
        UpdateUI();
    }

    protected override void UpdateUI()
    {
        if(value == null) return;
        
        ammoText.text = $"{value.CurrentBulletsCount}";
        maxAmmoText.text = $"/ {value.Data.MagSize}";

        weaponName.text = value.gameObject.name;
    }
}
