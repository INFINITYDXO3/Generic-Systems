using UnityEngine;

public class PlayerControlsManger : MonoBehaviour
{
    [SerializeField] private PlayerHandler player;
    [SerializeField] private InputManager input;


    void Start()
    {
        InitInputEvents();
    }

    private void InitInputEvents()
    {
        input.OnReloadStarted += player.Reload;
        input.OnNextWeaponSwitched += player.NextWeapon;
    }

    private void Update()
    {
        player.ProcessLook(input.Look);
        player.ProcessMove(input.Move);
        CheckSprint(input.Sprint);
        player.ToggleCrouch(input.Crouch);
        player.PerformJump(input.Jump);
        

        player.ToggleAttack(input.Attacking);
    }

    private void CheckSprint(bool isSprinting)
    {
        player.ToggleSprint(isSprinting);
        player.ApplyCameraEffect(CameraEffectsType.SprintEffect, isSprinting);
    }

}
