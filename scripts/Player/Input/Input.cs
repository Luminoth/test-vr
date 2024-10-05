using System;

namespace VrTest.Player.Input;

public abstract partial class Input : Node
{
    public event EventHandler JumpPressed;

    public event EventHandler JumpReleased;

    public event EventHandler CancelPressed;

    public abstract Vector2 MoveState { get; }

    public abstract Vector2 LookState { get; }

    protected void OnJumpPressed()
    {
        JumpPressed?.Invoke(this, EventArgs.Empty);
    }

    protected void OnJumpReleased()
    {
        JumpReleased?.Invoke(this, EventArgs.Empty);
    }

    public abstract bool IsJumpHeld();

    protected void OnCancelPressed()
    {
        CancelPressed?.Invoke(this, EventArgs.Empty);
    }
}
