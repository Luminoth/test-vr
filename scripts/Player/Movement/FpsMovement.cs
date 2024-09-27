using VrTest.Managers;
using VrTest.Player.Input;

namespace VrTest.Player.Movement;

// movement scripts need to be above the player character script
// in tree order so that they execute first
public partial class FpsMovement : Node
{
    [Export]
    private bool _enabled = true;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = XrManager.Instance.IsXrInitialized && value;

            SetProcess(_enabled);
            SetPhysicsProcess(_enabled);
        }
    }

    [Export]
    private XrPlayerCharacter _character;

    [Export]
    private XrInput _input;

    [Export]
    private float _snapTurnDelay = 0.2f;

    [Export]
    private float _snapTurnAngle = Mathf.DegToRad(20.0f);

    private float _snapTurnAccum;

    private float _gravity;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();

        SetProcess(XrManager.Instance.IsXrInitialized && Enabled);
        SetPhysicsProcess(XrManager.Instance.IsXrInitialized && Enabled);
    }

    public override void _PhysicsProcess(double delta)
    {
        // TODO: this sucks because we don't want to poll for this
        if(_input.IsJumpPressed()) {
            _character.Jump();
        }

        // rotation in the physics step
        // because hands will move from this
        ApplyRotation((float)delta);

        ApplyMovement((float)delta);
    }

    #endregion

    private void ApplyPhysicalRotation()
    {
        // match the character Y rotation to the camera
        _character.GlobalRotation = _character.GlobalRotation with { Y = _character.Player.Camera.GlobalRotation.Y };
    }

    private void ApplyInputRotation(float delta)
    {
        var input = _input.LookState;

        // from XRTools, rotate origin with snap turn
        _snapTurnAccum -= Mathf.Abs(input.X) * delta;
        if(_snapTurnAccum <= 0.0f) {
            _character.RotatePlayer(_snapTurnAngle * Mathf.Sign(input.X));
            _snapTurnAccum = _snapTurnDelay;
        }
    }

    private void ApplyRotation(float delta)
    {
        ApplyPhysicalRotation();
        ApplyInputRotation(delta);
    }

    private void ApplyPhysicalMovement(float delta)
    {
        // attempt to move the character to be under the camera on the X/Z plane
        var currentPosition = _character.GlobalPosition with { Y = 0.0f };
        var desiredPosition = _character.Player.Camera.GlobalPosition with { Y = 0.0f };
        var currentVelocity = _character.Velocity;
        _character.Velocity = (desiredPosition - currentPosition) / delta;
        _character.MoveAndSlide();
        _character.Velocity = currentVelocity;

        // move the origin back to match if we didn't make it
        var remaining = desiredPosition - _character.GlobalPosition with { Y = 0.0f };
        _character.Player.GlobalPosition -= remaining;
    }

    private void ApplyInputMovement(Vector2 input, float delta)
    {
        _character.ApplyGravity(_gravity, delta);

        var velocity = _character.Velocity;

        if(_character.IsGrounded || _character.Player.AllowAirControl) {
            var direction = _character.GlobalBasis * new Vector3(input.X, 0, input.Y);
            if(direction.LengthSquared() > 0.0f) {
                velocity.X = direction.X * _character.Player.MoveSpeed;
                velocity.Z = direction.Z * _character.Player.MoveSpeed;
            } else {
                velocity.X = velocity.Z = 0.0f;
            }
        }

        _character.Velocity = velocity;
        _character.MoveAndSlide();
    }

    private void ApplyMovement(float delta)
    {
        ApplyPhysicalMovement(delta);

        var currentPosition = _character.GlobalPosition;
        ApplyInputMovement(_input.MoveState, delta);

        // move the origin to match the character movement on the X/Z plane
        var distance = _character.GlobalPosition with { Y = 0.0f } - currentPosition with { Y = 0.0f };
        _character.Player.GlobalPosition += distance;
    }
}
