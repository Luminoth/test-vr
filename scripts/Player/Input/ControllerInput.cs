using System;

using VrTest.Managers;

namespace VrTest.Player.Input;

public partial class ControllerInput : Input
{
    [Export]
    private Vector2 _moveState;

    public override Vector2 MoveState => _moveState;

    [Export]
    private Vector2 _lookState;

    public override Vector2 LookState => _lookState;

    // TODO: move to game settings
    [Export]
    private float _moveDeadzone = 0.1f;

    // TODO: move to game settings
    [Export]
    private float _lookDeadzone = 0.1f;

    // TODO: move to game settings
    [Export]
    private bool _invertVerticalLook;

    #region Godot Lifecycle

    public override void _Ready()
    {
        if(XrManager.Instance.IsXrInitialized) {
            GD.Print("Disabling ControllerInput");
            SetProcess(false);
        }
    }

    public override void _Process(double delta)
    {
        _moveState = Godot.Input.GetVector("move left", "move right", "move forward", "move back");
        _moveState.X = Mathf.Abs(_moveState.X) < _moveDeadzone ? 0.0f : _moveState.X;
        _moveState.Y = Mathf.Abs(_moveState.Y) < _moveDeadzone ? 0.0f : _moveState.Y;

        _lookState = Godot.Input.GetVector("look left", "look right", "look up", "look down");
        _lookState.X = Mathf.Abs(_lookState.X) < _lookDeadzone ? 0.0f : _lookState.X;
        _lookState.Y = Mathf.Abs(_lookState.Y) < _lookDeadzone ? 0.0f : _lookState.Y;
        _lookState.Y *= _invertVerticalLook ? 1.0f : -1.0f;

        if(Godot.Input.IsActionJustPressed("jump")) {
            OnJumpPressed();
        } else if(Godot.Input.IsActionJustReleased("jump")) {
            OnJumpReleased();
        }

        if(Godot.Input.IsActionJustPressed("cancel")) {
            OnCancelPressed();
        }
    }

    #endregion

    public override bool IsJumpHeld()
    {
        return Godot.Input.IsActionPressed("jump");
    }
}
