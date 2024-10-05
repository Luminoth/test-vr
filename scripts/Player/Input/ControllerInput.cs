using VrTest.Managers;

namespace VrTest.Player.Input;

public partial class ControllerInput : Node
{
    [Export]
    private Vector2 _moveState;

    public Vector2 MoveState => _moveState;

    [Export]
    private Vector2 _lookState;

    public Vector2 LookState => _lookState;

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

        _lookState = Godot.Input.GetVector("look left", "look right", "look up", "look down");
        _lookState.Y *= _invertVerticalLook ? 1.0f : -1.0f;
    }

    #endregion

    // TODO: this sucks because we don't want to poll for this
    public bool IsJumpPressed()
    {
        return Godot.Input.IsActionJustPressed("jump");
    }

    // TODO: this sucks because we don't want to poll for this
    public bool IsJumpReleased()
    {
        return Godot.Input.IsActionJustReleased("jump");
    }
}
