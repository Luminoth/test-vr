using VrTest.Player.Input;

namespace VrTest.Player.Movement.NoXR;

// movement scripts need to be above the player character script
// in tree order so that they execute first
public partial class FpsMovement : Movement
{
    protected override bool IsXrMovement => false;

    [Export]
    private ControllerInput _input;

    // TODO: maybe this is a set of movements
    // that we can decide which to hand off to?
    [Export]
    private JetpackMovement _jetpackMovement;

    [Export]
    private float _tiltLowerLimit = Mathf.DegToRad(-90.0f);

    public float TiltLowerLimit => _tiltLowerLimit;

    [Export]
    private float _tiltUpperLimit = Mathf.DegToRad(90.0f);

    public float TiltUpperLimit => _tiltUpperLimit;

    // TODO: move to game settings
    [Export]
    private int _lookSensitivity = 4;

    public int LookSensitivity => _lookSensitivity;

    #region Godot Lifecycle

    public override void _Ready()
    {
        base._Ready();

        // TODO: this sort of assumes we don't start
        // in jetpack mode and that's probably not always correct
        if(_jetpackMovement != null) {
            GD.Print("Disabling Jetpack movement");
            _jetpackMovement.FpsMovement = this;
            _jetpackMovement.IsEnabled = false;
        }

        _input.JumpPressed += JumpPressedEventHandler;
    }

    public override void _ExitTree()
    {
        _input.JumpPressed -= JumpPressedEventHandler;
    }

    #endregion

    protected override void ApplyRotation(float delta)
    {
        var input = _input.LookState;

        Character.Player.Camera.RotateX(input.Y * LookSensitivity * delta);
        Character.Player.Camera.Rotation = Character.Player.Camera.Rotation with {
            X = Mathf.Clamp(Character.Player.Camera.Rotation.X, TiltLowerLimit, TiltUpperLimit)
        };

        Character.Player.RotateY(-input.X * LookSensitivity * delta);
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

    #region Event Handlers

    private void JumpPressedEventHandler(object sender, System.EventArgs e)
    {
        if(IsEnabled) {
            if(Character.IsOnFloor()) {
                Character.Jump();
            } else if(_jetpackMovement != null) {
                GD.Print("Switching to Jetpack movement");
                IsEnabled = false;
                _jetpackMovement.IsEnabled = true;
            }
        }
    }

    #endregion
}
