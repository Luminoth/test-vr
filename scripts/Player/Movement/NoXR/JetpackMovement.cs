using VrTest.Player.Input;

namespace VrTest.Player.Movement.NoXR;

// movement scripts need to be above the player character script
// in tree order so that they execute first
public partial class JetpackMovement : Movement
{
    protected override bool IsXrMovement => false;

    [Export]
    private ControllerInput _input;

    public FpsMovement FpsMovement { private get; set; }

    [Export]
    private float _verticalSpeed = 5.0f;

    // TODO: move to game settings
    [Export]
    private bool _jetpackHoldInput = false;

    #region Godot Lifecycle

    public override void _Ready()
    {
        if(FpsMovement == null) {
            GD.Print("Jetpack movement missing FpsMovement, disabling");
            IsEnabled = false;
        }

        _input.JumpReleased += JumpReleasedEventHandler;
        _input.CancelPressed += CancelPressedEventHandler;
    }

    public override void _ExitTree()
    {
        _input.JumpReleased -= JumpReleasedEventHandler;
        _input.CancelPressed -= CancelPressedEventHandler;
    }

    public override void _PhysicsProcess(double delta)
    {
        if(Character.IsOnFloor()) {
            GD.Print("Grounded, switching to FPS movement");
            DisableJetpack();
        }

        base._PhysicsProcess(delta);
    }

    #endregion

    protected override void ApplyRotation(float delta)
    {
        var input = _input.LookState;

        Character.Player.Camera.RotateX(input.Y * FpsMovement.LookSensitivity * delta);
        Character.Player.Camera.Rotation = Character.Player.Camera.Rotation with {
            X = Mathf.Clamp(Character.Player.Camera.Rotation.X, FpsMovement.TiltLowerLimit, FpsMovement.TiltUpperLimit)
        };

        Character.Player.RotateY(-input.X * FpsMovement.LookSensitivity * delta);
    }

    protected override void ApplyMovement(float delta)
    {
        var velocity = Character.Velocity;

        // apply thrust
        velocity.Y = _input.IsJumpHeld() ? _verticalSpeed : 0.0f;

        var input = _input.MoveState;
        var direction = Character.GlobalBasis * new Vector3(input.X, 0, input.Y);
        if(direction.LengthSquared() > 0.0f) {
            velocity.X = direction.X * Character.Player.MoveSpeed;
            velocity.Z = direction.Z * Character.Player.MoveSpeed;
        } else {
            velocity.X = velocity.Z = 0.0f;
        }

        Character.Velocity = velocity;
        Character.MoveAndSlide();
    }

    private void DisableJetpack()
    {
        IsEnabled = false;
        FpsMovement.IsEnabled = true;
    }

    #region Event Handlers

    private void JumpReleasedEventHandler(object sender, System.EventArgs e)
    {
        if(IsEnabled && _jetpackHoldInput) {
            GD.Print("Switching to FPS movement");
            DisableJetpack();
        }
    }

    private void CancelPressedEventHandler(object sender, System.EventArgs e)
    {
        if(IsEnabled && !_jetpackHoldInput) {
            GD.Print("Switching to FPS movement");
            DisableJetpack();
        }
    }

    #endregion
}
