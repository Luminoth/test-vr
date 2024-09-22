using VrTest.Managers;
using VrTest.Player.Input;

namespace VrTest.Player;

public partial class XrToolsPlayer : XROrigin3D
{
    // NOTE: the XR tools will make this a top level node
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

        // TODO: local player hide
        // remote player show
        _head.Hide();
    }

    public override void _Process(double delta)
    {
        if(XrManager.Instance.IsXrInitialized) {
            // TODO:
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

    // TODO: this is fighting the XR tools PlayerBody
    // and I'm not sure atm how to turn that off
    /*public override void _PhysicsProcess(double delta)
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
    }*/

    #endregion
}
