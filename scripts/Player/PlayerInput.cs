namespace VrTest.Player;

public partial class PlayerInput : Node
{
    [Export]
    private Vector2 _moveDirection;

    public Vector2 MoveDirection => _moveDirection;

    [Export]
    private Vector2 _lookDirection;

    public Vector2 LookDirection => _lookDirection;

    #region Godot Lifecycle

    public override void _Process(double delta)
    {
        _moveDirection = Input.GetVector("move left", "move right", "move forward", "move back");
        _lookDirection = Input.GetVector("look left", "look right", "look forward", "look back");
    }

    #endregion
}
