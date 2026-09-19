using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private PlayerHandler player;
    [SerializeField] private List<PlayerUIElement> playerUIElements;

    private Weapon ui_weapon;

    private HealthSystem ui_healthSystem;
    private float ui_speed;

    private Dictionary<ElementType, PlayerUIElement> _elements;

    private void Start()
    {
        _elements = new Dictionary<ElementType, PlayerUIElement>();
        foreach (var element in playerUIElements)
        {
            _elements[element.ElementType] = element;
        }
    }


    private void Update()
    {
        if(ui_healthSystem != player.HealthSystem)
        {
            ui_healthSystem = player.HealthSystem;
            UpdateValue(ElementType.Health, ui_healthSystem);
        }

        if(ui_weapon != player.CurrentWeapon)
        {
            ui_weapon = player.CurrentWeapon;
            UpdateValue(ElementType.Weapon, ui_weapon);
        }

        if(ui_speed != player.Speed)
        {
            ui_speed = player.Speed;
            UpdateValue(ElementType.Speed, ui_speed);
        }


    }

    private void UpdateValue<T>(ElementType elementType, T value )
    {
        if(_elements.TryGetValue(elementType, out PlayerUIElement element)) element.SetValue(value);
    }



}
