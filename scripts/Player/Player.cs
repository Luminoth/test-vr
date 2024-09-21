using VrTest.Managers;

namespace VrTest.Player;

// NOTE: disable all of the XRTools scripts before using this!!!
public partial class Player : XROrigin3D
{
    [Export]
    private CharacterBody3D _character;

    [Export]
    private XRCamera3D _camera;

    [Export]
    private Node3D _head;

    [Export]
    private ControllerInput _controllerInput;

    [Export]
    private XrInput _xrInput;

    [Export]
    private float _playerHeight = 1.8f;

    [Export]
    private float _tiltLowerLimit = Mathf.DegToRad(-90.0f);

    [Export]
    private float _tiltUpperLimit = Mathf.DegToRad(90.0f);

    // TODO: this needs to be sync'd to the XR Tools movement
    [Export]
    private float _moveSpeed = 5.0f;

    // TODO: move to game settings
    [Export]
    private int _lookSensitivity = 4;

    [Export]
    private Label _fpsLabel;

    private double _gravity;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();

        _character.TopLevel = true;

        // TODO: local player hide
        // remote player show
        _head.Hide();
    }

    public override void _Process(double delta)
    {
        if(XrManager.Instance.IsXrInitialized) {
            // TODO:

            // move the origin to fix the camera at the player height
            // minus a little bit to be at the eye position
            // (assuming Local reference space here, Local Floor and Stage shouldn't do this)
            GlobalPosition = GlobalPosition with { Y = _playerHeight - _camera.Position.Y - 0.1f };
        } else {
            var input = _controllerInput.LookState;

            _camera.RotateX(input.Y * _lookSensitivity * (float)delta);
            _camera.Rotation = _camera.Rotation with { X = Mathf.Clamp(_camera.Rotation.X, _tiltLowerLimit, _tiltUpperLimit) };

            RotateY(-input.X * _lookSensitivity * (float)delta);

            // rotate the character to match the origin
            _character.Rotation = Rotation;
        }

        _fpsLabel.Text = $"FPS: {Engine.GetFramesPerSecond()}";
    }

    public override void _PhysicsProcess(double delta)
    {
        if(XrManager.Instance.IsXrInitialized) {
            // TODO:
        } else {
            var velocity = _character.Velocity;

            var input = _controllerInput.MoveState;
            var direction = GlobalBasis * new Vector3(input.X, 0, input.Y);
            if(direction.LengthSquared() > 0.0f) {
                velocity.X = direction.X * _moveSpeed;
                velocity.Z = direction.Z * _moveSpeed;
            } else {
                velocity.X = velocity.Z = 0.0f;
            }

            // apply gravity
            velocity.Y -= (float)(_gravity * delta);

            _character.Velocity = velocity;
            _character.MoveAndSlide();

            // move the origin to match the body
            GlobalPosition = _character.GlobalPosition;
        }
    }

    #endregion
}
