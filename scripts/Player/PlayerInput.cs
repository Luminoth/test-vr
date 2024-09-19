using VrTest.Managers;

namespace VrTest.Player;

// TODO: https://docs.godotengine.org/en/stable/tutorials/xr/xr_action_map.html
public partial class PlayerInput : Node
{
    [Export]
    private XRController3D _leftHand;

    [Export]
    private XRController3D _rightHand;

    [Export]
    private Vector2 _moveState;

    public Vector2 MoveState => _moveState;

    [Export]
    private Vector2 _lookState;

    public Vector2 LookState => _lookState;

    // TODO: move to game settings
    [Export]
    private bool _invertVerticalLook;

    // TODO: move to game settings
    [Export]
    private int _lookSensitivity = 4;

    #region Godot Lifecycle

    public override void _Process(double delta)
    {
        if(XrManager.Instance.IsXrInitialized) {
            _moveState = _leftHand.GetVector2("move");
            _moveState.Y *= -1.0f;
        } else {
            _moveState = Input.GetVector("move left", "move right", "move forward", "move back");

            _lookState = Input.GetVector("look left", "look right", "look up", "look down") * _lookSensitivity;
            _lookState.Y *= _invertVerticalLook ? 1.0f : -1.0f;
        }
    }

    #endregion
}
