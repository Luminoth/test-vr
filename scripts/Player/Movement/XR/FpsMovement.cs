using VrTest.Player.Input;

namespace VrTest.Player.Movement.XR;

// movement scripts need to be above the player character script
// in tree order so that they execute first
public partial class FpsMovement : Movement
{
    protected override bool IsXrMovement => true;

    [Export]
    private XrInput _input;

    // TODO: maybe this is a set of movements
    // that we can decide which to hand off to?
    [Export]
    private JetpackMovement _jetpackMovement;

    [Export]
    private float _snapTurnDelay = 0.2f;

    [Export]
    private float _snapTurnAngle = Mathf.DegToRad(20.0f);

    private float _snapTurnAccum;

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

    private void ApplyInputRotation(float delta)
    {
        var input = _input.LookState;

        // from XRTools, rotate origin with snap turn
        _snapTurnAccum -= Mathf.Abs(input.X) * delta;
        if(_snapTurnAccum <= 0.0f) {
            Character.RotatePlayer(_snapTurnAngle * Mathf.Sign(input.X));
            _snapTurnAccum = _snapTurnDelay;
        }
    }

    public override void ApplyRotation(float delta)
    {
        ApplyPhysicalRotation();
        ApplyInputRotation(delta);
    }

    private void ApplyInputMovement(Vector2 input, float delta)
    {
        var velocity = Character.Velocity;

        if(Character.IsOnFloor() || Character.Player.AllowAirControl) {
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

    protected override void ApplyMovement(float delta)
    {
        ApplyPhysicalMovement(delta);

        var currentPosition = Character.GlobalPosition;

        Character.ApplyGravity(Gravity, delta);

        ApplyInputMovement(_input.MoveState, delta);

        UpdateOrigin(currentPosition);
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
