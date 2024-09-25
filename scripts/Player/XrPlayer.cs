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
    private Label _fpsLabel;

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
    private float _gravityModifier = 1.0f;

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
    }

    #endregion
}
