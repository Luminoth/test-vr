using System;

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
            ProcessXrRotation((float)delta);

            ProcessXrMovement((float)delta);
        } else {
            // rotation in the physics step
            // because hands will move from this
            ProcessNoXrRotation((float)delta);

            ProcessNoXrMovement((float)delta);
        }
    }

    #endregion

    private void ProcessXrRotation(float delta)
    {
        var input = _xrInput.LookState;

        // rotate character with snap turn
        // snap turn step accumulator from XRTools
        _snapTurnAccum -= Mathf.Abs(input.X) * delta;
        if(_snapTurnAccum <= 0.0f) {
            _character.Player.RotateY(_snapTurnAngle * -MathF.Sign(input.X));
            _snapTurnAccum = _snapTurnDelay;
        }
    }

    private void ProcessXrMovement(float delta)
    {
        // TODO:
    }

    private void ProcessNoXrRotation(float delta)
    {
        var input = _controllerInput.LookState;

        _character.Player.Camera.RotateX(input.Y * _lookSensitivity * delta);
        _character.Player.Camera.Rotation = _character.Player.Camera.Rotation with { X = Mathf.Clamp(_character.Player.Camera.Rotation.X, _tiltLowerLimit, _tiltUpperLimit) };

        _character.Player.RotateY(-input.X * _lookSensitivity * delta);
    }

    private void ProcessNoXrMovement(float delta)
    {
        var velocity = _character.Velocity;

        var input = _controllerInput.MoveState;
        var direction = _character.Player.GlobalBasis * new Vector3(input.X, 0, input.Y);
        if(direction.LengthSquared() > 0.0f) {
            velocity.X = direction.X * _character.Player.MoveSpeed;
            velocity.Z = direction.Z * _character.Player.MoveSpeed;
        } else {
            velocity.X = velocity.Z = 0.0f;
        }

        // apply gravity
        velocity.Y -= (float)(_gravity * delta);

        _character.Velocity = velocity;
        _character.MoveAndSlide();
    }
}
