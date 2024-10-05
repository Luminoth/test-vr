using VrTest.Player.Input;

namespace VrTest.Player.Movement.NoXR;

// movement scripts need to be above the player character script
// in tree order so that they execute first
public partial class FpsMovement : Movement
{
    protected override bool IsXrMovement => false;

    [Export]
    private ControllerInput _input;

    [Export]
    private float _tiltLowerLimit = Mathf.DegToRad(-90.0f);

    [Export]
    private float _tiltUpperLimit = Mathf.DegToRad(90.0f);

    // TODO: move to game settings
    [Export]
    private int _lookSensitivity = 4;

    #region Godot Lifecycle

    public override void _PhysicsProcess(double delta)
    {
        // TODO: this sucks because we don't want to poll for this
        if(_input.IsJumpPressed()) {
            Character.Jump();
        }

        base._PhysicsProcess(delta);
    }

    #endregion

    protected override void ApplyRotation(float delta)
    {
        var input = _input.LookState;

        Character.Player.Camera.RotateX(input.Y * _lookSensitivity * delta);
        Character.Player.Camera.Rotation = Character.Player.Camera.Rotation with { X = Mathf.Clamp(Character.Player.Camera.Rotation.X, _tiltLowerLimit, _tiltUpperLimit) };

        Character.Player.RotateY(-input.X * _lookSensitivity * delta);
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
