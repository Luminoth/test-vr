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

    // TODO: move to game settings
    [Export]
    private bool _jetpackHoldInput = true;

    #region Godot Lifecycle

    public override void _Ready()
    {
        if(FpsMovement == null) {
            GD.Print("Jetpack movement missing FpsMovement, disabling");
            IsEnabled = false;
        }
    }

    public override void _Process(double delta)
    {
        // TODO: this sucks because we don't want to poll for this
        if(_input.IsJumpReleased()) {
            GD.Print("Switching to FPS movement");
            IsEnabled = false;
            FpsMovement.IsEnabled = true;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if(Character.IsOnFloor()) {
            GD.Print("Grounded, switching to FPS movement");
            IsEnabled = false;
            FpsMovement.IsEnabled = true;
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
        Character.ApplyGravity(Gravity, delta);

        var velocity = Character.Velocity;

        if(Character.IsOnFloor() || Character.Player.AllowAirControl) {
            var input = _input.MoveState;
            var direction = Character.GlobalBasis * new Vector3(input.X, 0, input.Y);
            if(direction.LengthSquared() > 0.0f) {
                velocity.X = direction.X * Character.Player.MoveSpeed;
                velocity.Z = direction.Z * Character.Player.MoveSpeed;
            } else {
                velocity.X = velocity.Z = 0.0f;
            }
        }

        Character.Velocity = velocity;
        Character.MoveAndSlide();
    }
}
