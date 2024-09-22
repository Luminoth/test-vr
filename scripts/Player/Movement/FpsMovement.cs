using VrTest.Managers;

namespace VrTest.Player;

public partial class FpsMovement : Node
{
    [Export]
    private Player _player;

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
    }

    public override void _PhysicsProcess(double delta)
    {
        if(XrManager.Instance.IsXrInitialized) {
            var input = _xrInput.LookState;

            // snap turn step accumulator from XRTools
            _snapTurnAccum -= Mathf.Abs(input.X) * (float)delta;
            if(_snapTurnAccum <= 0.0f) {
                _player.Character.RotateY(_snapTurnAngle);
                _snapTurnAccum = _snapTurnDelay;
            }

            // TODO: try to move the character
        } else {
            var input = _controllerInput.LookState;

            _player.Camera.RotateX(input.Y * _lookSensitivity * (float)delta);
            _player.Camera.Rotation = _player.Camera.Rotation with { X = Mathf.Clamp(_player.Camera.Rotation.X, _tiltLowerLimit, _tiltUpperLimit) };

            _player.RotateY(-input.X * _lookSensitivity * (float)delta);

            var velocity = _player.Character.Velocity;

            input = _controllerInput.MoveState;
            var direction = _player.GlobalBasis * new Vector3(input.X, 0, input.Y);
            if(direction.LengthSquared() > 0.0f) {
                velocity.X = direction.X * _player.MoveSpeed;
                velocity.Z = direction.Z * _player.MoveSpeed;
            } else {
                velocity.X = velocity.Z = 0.0f;
            }

            // apply gravity
            velocity.Y -= (float)(_gravity * delta);

            _player.Character.Velocity = velocity;
            _player.Character.MoveAndSlide();
        }
    }

    #endregion
}
