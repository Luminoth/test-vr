using VrTest.Player.Input;

namespace VrTest.Player;

public partial class XrPlayer : XROrigin3D
{
    [Export]
    private XRCamera3D _camera;

    public XRCamera3D Camera => _camera;

    [Export]
    private XrPlayerCharacter _character;

    [Export]
    private XrInput _xrInput;

    [Export]
    private CollisionShape3D _collider;

    public float Height => ((CapsuleShape3D)_collider.Shape).Height;

    public float Radius => ((CapsuleShape3D)_collider.Shape).Radius;

    [Export]
    private Label _fpsLabel;

    [Export]
    private Label _isGroundedLabel;

    [Export]
    private Label _velocityLabel;

    [Export]
    private Label _leftHandVelocityLabel;

    [Export]
    private Label _rightHandVelocityLabel;

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
    private float _terminalVelocity = 100.0f;

    public float TermainalVelocity => _terminalVelocity;

    #region Godot Lifecycle

    public override void _Ready()
    {
        GD.Print($"Player height: {Height}");
    }

    public override void _Process(double delta)
    {
        _fpsLabel.Text = $"FPS: {Engine.GetFramesPerSecond()}";
        _isGroundedLabel.Text = $"IsGrounded: {_character.IsGrounded}";
        _velocityLabel.Text = $"Velocity: {_character.Velocity}";
        _leftHandVelocityLabel.Text = $"Left Hand Velocity: {_xrInput.LeftHand.Velocity}";
        _rightHandVelocityLabel.Text = $"Right Hand Velocity: {_xrInput.RightHand.Velocity}";
    }

    #endregion
}
