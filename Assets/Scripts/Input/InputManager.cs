using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	private Vector2 move;
	private Vector2 look;
	private bool jump;
	private bool sprint;
	private bool crouch;
	private bool attacking;


	public Vector2 Move {get => move;}
	public Vector2 Look {get => look;}
	public bool Jump {get => jump;}
	public bool Sprint {get => sprint;}
	public bool Crouch {get => crouch;}
	public bool Attacking {get => attacking;}

	public event Action OnReloadStarted;
	public event Action OnNextWeaponSwitched;

	public void OnMove(InputValue value)
	{
		move = value.Get<Vector2>();
	}

	public void OnLook(InputValue value)
	{
		look = value.Get<Vector2>();
		
	}

	public void OnJump(InputValue value)
	{
		jump = value.isPressed;
	}

	public void OnSprint(InputValue value)
	{
		sprint = value.isPressed;
	}

    public void OnCrouch(InputValue value)
    {
        crouch = value.isPressed;
    }

    public void OnMousePress_Left(InputValue value)
	{
		attacking = value.isPressed;
	}


	private void OnApplicationFocus(bool hasFocus)
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void OnReload(InputValue value)
	{
		OnReloadStarted?.Invoke();
	}

	public void OnNextWeapon(InputValue value)
	{
		OnNextWeaponSwitched?.Invoke();
	}


}
	
