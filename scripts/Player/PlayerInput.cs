using VrTest.Managers;

namespace VrTest.Player;

// TODO: https://docs.godotengine.org/en/stable/tutorials/xr/xr_action_map.html
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
        if(!XrManager.Instance.IsXrInitialized) {
            // TODO: handle the XR controller actions
        }

        _moveDirection = Input.GetVector("move left", "move right", "move forward", "move back");
        _lookDirection = Input.GetVector("look left", "look right", "look up", "look down");
    }

    #endregion
}
