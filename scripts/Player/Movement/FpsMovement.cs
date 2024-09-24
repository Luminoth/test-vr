using VrTest.Managers;
using VrTest.Player.Input;

namespace VrTest.Player;

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
            SetProcess(_enabled);
            SetPhysicsProcess(_enabled);
        }
    }

    [Export]
    private XrPlayerCharacter _character;

    [Export]
    private ControllerInput _controllerInput;

    [Export]
    private XrInput _xrInput;

    [Export]
    private float _tiltLowerLimit = Mathf.DegToRad(-90.0f);

    [Export]
    private float _tiltUpperLimit = Mathf.DegToRad(90.0f);

    // TODO: move to game settings
    [Export]
    private int _lookSensitivity = 4;

    [Export]
    private float _snapTurnDelay = 0.2f;

    [Export]
    private float _snapTurnAngle = Mathf.DegToRad(20.0f);

    private float _snapTurnAccum;

    private double _gravity;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();

        SetProcess(Enabled);
        SetPhysicsProcess(Enabled);
    }

    public override void _PhysicsProcess(double delta)
    {
        if(XrManager.Instance.IsXrInitialized) {
            // rotation in the physics step
            // because hands will move from this
            ApplyXrRotation((float)delta);

            ApplyXrMovement((float)delta);
        } else {
            // rotation in the physics step
            // because hands will move from this
            ApplyNoXrRotation((float)delta);

            ApplyNoXrMovement((float)delta);
        }
    }

    #endregion

    private void ApplyInputMovement(Vector2 input, float delta)
    {
        var velocity = _character.Velocity;

        // apply gravity
        velocity.Y = Mathf.Clamp(
            velocity.Y - (float)(_gravity * _character.Player.GravityModifier * delta),
            -_character.Player.TermainalVelocity,
            _character.Player.TermainalVelocity
        );

        var direction = _character.GlobalBasis * new Vector3(input.X, 0, input.Y);
        if(direction.LengthSquared() > 0.0f) {
            velocity.X = direction.X * _character.Player.MoveSpeed;
            velocity.Z = direction.Z * _character.Player.MoveSpeed;
        } else {
            velocity.X = velocity.Z = 0.0f;
        }

        _character.Velocity = velocity;
        _character.MoveAndSlide();
    }

    #region XR

    private void ApplyXrPhysicalRotation()
    {
        // match the character Y rotation to the camera
        _character.GlobalRotation = _character.GlobalRotation with { Y = _character.Player.Camera.GlobalRotation.Y };
    }

    private void ApplyXrInputRotation(float delta)
    {
        var input = _xrInput.LookState;

        // from XRTools, rotate origin with snap turn
        _snapTurnAccum -= Mathf.Abs(input.X) * delta;
        if(_snapTurnAccum <= 0.0f) {
            _character.RotatePlayer(_snapTurnAngle * Mathf.Sign(input.X));
            _snapTurnAccum = _snapTurnDelay;
        }
    }

    private void ApplyXrRotation(float delta)
    {
        ApplyXrPhysicalRotation();
        ApplyXrInputRotation(delta);
    }

    private void ApplyXrPhysicalMovement(float delta)
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

    private void ApplyXrMovement(float delta)
    {
        ApplyXrPhysicalMovement(delta);

        var currentPosition = _character.GlobalPosition;
        ApplyInputMovement(_xrInput.MoveState, delta);

        // move the origin to match the character movement on the X/Z plane
        var distance = _character.GlobalPosition with { Y = 0.0f } - currentPosition with { Y = 0.0f };
        _character.Player.GlobalPosition += distance;
    }

    #endregion

    #region No XR

    private void ApplyNoXrRotation(float delta)
    {
        var input = _controllerInput.LookState;

        _character.Player.Camera.RotateX(input.Y * _lookSensitivity * delta);
        _character.Player.Camera.Rotation = _character.Player.Camera.Rotation with { X = Mathf.Clamp(_character.Player.Camera.Rotation.X, _tiltLowerLimit, _tiltUpperLimit) };

        _character.Player.RotateY(-input.X * _lookSensitivity * delta);
    }

    private void ApplyNoXrMovement(float delta)
    {
        ApplyInputMovement(_controllerInput.MoveState, delta);
    }

    #endregion
}
