using VrTest.Managers;

namespace VrTest.Player.Input;

public partial class XrInput : Input
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

    private XRController3D _leftController;

    public XRController3D LeftController => _leftController;

    private XRController3D _rightController;

    public XRController3D RightController => _rightController;

    #region Godot Lifecycle

    public override void _Ready()
    {
        var origin = XrManager.Instance.XrPlayer;
        _leftController = origin.LeftHand;
        _rightController = origin.RightHand;

        if(!XrManager.Instance.IsXrInitialized) {
            GD.Print("Disabling XrInput");
            SetProcess(false);
        } else {
            _rightController.ButtonPressed += RightHandButtonPressedEventHandler;
        }
    }

    public override void _ExitTree()
    {
        _rightController.ButtonPressed -= RightHandButtonPressedEventHandler;
    }

    public override void _Process(double delta)
    {
        _moveState = _leftController.GetVector2("primary");
        _moveState.X = Mathf.Abs(_moveState.X) < _moveDeadzone ? 0.0f : _moveState.X;
        _moveState.Y = Mathf.Abs(_moveState.Y) < _moveDeadzone ? 0.0f : -_moveState.Y;

        _lookState = _rightController.GetVector2("primary");
        _lookState.X = Mathf.Abs(_lookState.X) < _lookDeadzone ? 0.0f : _lookState.X;
        _lookState.Y = Mathf.Abs(_lookState.Y) < _lookDeadzone ? 0.0f : _lookState.Y;
        _lookState.Y *= _invertVerticalLook ? 1.0f : -1.0f;
    }

    #endregion

    public override bool IsJumpHeld()
    {
        return _rightController.IsButtonPressed("ax_button");
    }

    #region Event Handlers

    private void RightHandButtonPressedEventHandler(string name)
    {
        switch(name) {
        case "ax_button":
            OnJumpPressed();
            break;
        case "by_button":
            OnCancelPressed();
            break;
        }
    }

    private void RightHandButtonReleasedEventHandler(string name)
    {
        switch(name) {
        case "ax_button":
            OnJumpReleased();
            break;
        }
    }

    #endregion
}
