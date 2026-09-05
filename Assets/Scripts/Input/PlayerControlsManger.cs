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
        input.OnJumpPerformed += player.PerformJump;
    }

    private void Update()
    {
        player.ProcessMove(input.Move);
        CheckSprint(input.Sprint);
        player.ToggleCrouch(input.Crouch);
        

        player.ToggleAttack(input.Attacking);
    }

    private void LateUpdate()
    {
        player.ProcessLook(input.Look);
    }
    
    private void CheckSprint(bool isSprinting)
    {
        player.ToggleSprint(isSprinting);
        player.ApplyCameraEffect(CameraEffectsType.SprintEffect, isSprinting);
    }

}
