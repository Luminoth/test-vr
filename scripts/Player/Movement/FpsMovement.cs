using VrTest.Managers;
using VrTest.Player.Input;

namespace VrTest.Player;

// movement scripts need to be above the player script
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

    private void ApplyXrRotation(float delta)
    {
        var input = _xrInput.LookState;

        // rotate origin with snap turn
        // snap turn step accumulator from XRTools
        // TODO: this isn't working, I think because the camera
        // is rotating around the origin with this
        _snapTurnAccum -= Mathf.Abs(input.X) * delta;
        if(_snapTurnAccum <= 0.0f) {
            _character.Player.RotateY(_snapTurnAngle * -Mathf.Sign(input.X));
            _snapTurnAccum = _snapTurnDelay;
        }
    }

    private void ApplyPhysicalMovement(float delta)
    {
        var cameraBasis = _character.Player.Camera.GlobalBasis;
        cameraBasis.X = Vector3.Right;
        cameraBasis.Y = Vector3.Up;

        var eyeOffset = cameraBasis * new Vector3(0.0f, 0.0f, _character.Player.EyeForwardOffset);
        GD.Print($"eye offset: {eyeOffset}");
        if(eyeOffset.Y != 0.0f) {
            GD.PushWarning($"invalid eye offset: {eyeOffset}");
        }

        // try and move the character to match the camera, ignoring the Y axis
        var desiredPosition = _character.Player.Camera.GlobalPosition with { Y = _character.GlobalPosition.Y } + eyeOffset;
        var distanceSquared = (desiredPosition - _character.GlobalPosition).LengthSquared();
        GD.Print($"from {_character.GlobalPosition} to {desiredPosition}: {distanceSquared}");
        if(distanceSquared > 0.1f) {
            var currentVelocity = _character.Velocity;
            _character.Velocity = (desiredPosition - _character.GlobalPosition) / delta;
            _character.MoveAndSlide();
            _character.Velocity = currentVelocity;
        }

    }

    private void ApplyXrMovement(float delta)
    {
        ApplyPhysicalMovement(delta);
        ApplyInputMovement(_xrInput.MoveState, delta);
    }

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
}
