using VrTest.Managers;
using VrTest.Player.Input;

namespace VrTest.Player.Movement;

// movement scripts need to be above the player character script
// in tree order so that they execute first
public partial class ControllerMovement : Node
{
    [Export]
    private bool _enabled = true;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = !XrManager.Instance.IsXrInitialized && value;

            SetProcess(_enabled);
            SetPhysicsProcess(_enabled);
        }
    }

    [Export]
    private XrPlayerCharacter _character;

    [Export]
    private ControllerInput _input;

    [Export]
    private float _tiltLowerLimit = Mathf.DegToRad(-90.0f);

    [Export]
    private float _tiltUpperLimit = Mathf.DegToRad(90.0f);

    // TODO: move to game settings
    [Export]
    private int _lookSensitivity = 4;

    private float _gravity;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();

        SetProcess(!XrManager.Instance.IsXrInitialized && Enabled);
        SetPhysicsProcess(!XrManager.Instance.IsXrInitialized && Enabled);
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

    private void ApplyRotation(float delta)
    {
        var input = _input.LookState;

        _character.Player.Camera.RotateX(input.Y * _lookSensitivity * delta);
        _character.Player.Camera.Rotation = _character.Player.Camera.Rotation with { X = Mathf.Clamp(_character.Player.Camera.Rotation.X, _tiltLowerLimit, _tiltUpperLimit) };

        _character.Player.RotateY(-input.X * _lookSensitivity * delta);
    }

    private void ApplyMovement(float delta)
    {
        _character.ApplyGravity(_gravity, delta);

        var velocity = _character.Velocity;

        if(_character.IsOnFloor() || _character.Player.AllowAirControl) {
            var input = _input.MoveState;
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
}
