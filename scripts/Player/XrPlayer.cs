namespace VrTest.Player;

public partial class XrPlayer : XROrigin3D
{
    [Export]
    private XRCamera3D _camera;

    public XRCamera3D Camera => _camera;

    [Export]
    private CollisionShape3D _collider;

    public float Height => ((CapsuleShape3D)_collider.Shape).Height;

    public float Radius => ((CapsuleShape3D)_collider.Shape).Radius;

    [Export]
    private float _eyeForwardOffset = 0.5f;

    public float EyeForwardOffset => _eyeForwardOffset;

    [Export]
    private float _eyeHeightOffset = 0.1f;

    public float EyeHeightOffset => _eyeHeightOffset;

    [Export]
    private float _moveSpeed = 5.0f;

    public float MoveSpeed => _moveSpeed;

    [Export]
    private float _jumpSpeed = 5.0f;

    public float JumpSpeed => _jumpSpeed;

    [Export]
    private bool _allowAirControl;

    public bool AllowAirControl => _allowAirControl;

    [Export]
    private float _gravityModifier = 2.0f;

    public float GravityModifier => _gravityModifier;

    [Export]
    private float _terminalSpeed = 100.0f;

    public float TerminalSpeed => _terminalSpeed;

    #region Godot Lifecycle

    public override void _Ready()
    {
        GD.Print($"Player height: {Height}");
    }

    #endregion
}
