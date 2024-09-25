using VrTest.Managers;

namespace VrTest.Player.Input;

public partial class XrInput : Node
{
    [Export]
    private PlayerHand _leftHand;

    public PlayerHand LeftHand => _leftHand;

    [Export]
    private PlayerHand _rightHand;

    public PlayerHand RightHand => _rightHand;

    [Export]
    private float _moveDeadzone = 0.5f;

    [Export]
    private float _lookDeadzone = 0.5f;

    [Export]
    private Vector2 _moveState;

    public Vector2 MoveState => _moveState;

    [Export]
    private Vector2 _lookState;

    public Vector2 LookState => _lookState;

    #region Godot Lifecycle

    public override void _Ready()
    {
        if(!XrManager.Instance.IsXrInitialized) {
            GD.Print("Disabling XrInput");
            SetProcess(false);
        }
    }

    public override void _Process(double delta)
    {
        _moveState = _leftHand.GetVector2("primary");
        _moveState.X = Mathf.Abs(_moveState.X) < _moveDeadzone ? 0.0f : _moveState.X;
        _moveState.Y = Mathf.Abs(_moveState.Y) < _moveDeadzone ? 0.0f : -_moveState.Y;

        _lookState = _rightHand.GetVector2("primary");
        _lookState.X = Mathf.Abs(_lookState.X) < _lookDeadzone ? 0.0f : _lookState.X;
        _lookState.Y = Mathf.Abs(_lookState.Y) < _lookDeadzone ? 0.0f : _lookState.Y;
    }

    #endregion
}
