using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public Vector2 move;
    public Vector2 look;
    public bool sprint;
    public bool walk;
    public bool aim;
    public bool shoot;
    public bool reload;
    public bool jump;

    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        LookInput(value.Get<Vector2>());
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void OnWalk(InputValue value)
    {
        walk = !walk;
    }

    public void OnAim(InputValue value)
    {
        AimInput(value.isPressed);
    }

    public void OnShoot(InputValue value)
    {
        ShootInput(value.isPressed);
    }

    public void OnReload(InputValue value)
    {
        ReloadInput(value.isPressed);
    }
    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }


    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }

    public void AimInput(bool newAimState)
    {
        aim = newAimState;
    }

    public void ShootInput(bool newShootState)
    {
        shoot = newShootState;
    }
    public void ReloadInput(bool newReloadState)
    {
        reload = newReloadState;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}