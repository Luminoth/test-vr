using VrTest.Managers;

namespace VrTest.Player;

public partial class XrInput : Node
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

    //[Export]
    private Vector2 _previousLookState;

    public Vector2 PreviousLookState => _previousLookState;

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
        _previousLookState = _lookState;

        _moveState = _leftHand.GetVector2("primary");
        _moveState.Y *= -1.0f;
        _lookState = _rightHand.GetVector2("primary");
    }

    #endregion
}
