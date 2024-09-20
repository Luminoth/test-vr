using VrTest.Managers;

namespace VrTest.Player;

// body-centric player
// https://docs.godotengine.org/en/stable/tutorials/xr/xr_room_scale.html
// TODO: https://github.com/godotengine/godot-demo-projects/blob/master/xr/openxr_character_centric_movement/player.gd is more complete than the docs
public partial class PlayerCharacter : CharacterBody3D
{
    [Export]
    private XROrigin3D _origin;

    [Export]
    private XRCamera3D _camera;

    [Export]
    private Node3D _neck;

    [Export]
    private PlayerInput _input;

    [Export]
    private float _tiltLowerLimit = Mathf.DegToRad(-90.0f);

    [Export]
    private float _tiltUpperLimit = Mathf.DegToRad(90.0f);

    [Export]
    private float _moveSpeed = 5.0f;

    // TODO: move to game settings
    [Export]
    private int _lookSensitivity = 4;

    private double _gravity;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();
    }

    public override void _PhysicsProcess(double delta)
    {
        bool isColliding = ProcessOnPhysicalMovement(delta);
        if(!isColliding) {
            ProcessRotationOnInput(delta);
            ProcessMovementOnInput(delta);
        }
    }

    #endregion

    private bool ProcessOnPhysicalMovement(double delta)
    {
        var currentVelocity = Velocity;

        // rotate the origin to match the irl player
        var cameraBasis = _origin.Basis * _camera.Basis;
        var forward = new Vector2(cameraBasis.Z.X, cameraBasis.Z.Z);
        var angle = forward.AngleTo(new Vector2(0.0f, 1.0f));

        // rotate the body
        Basis = Basis.Rotated(Vector3.Up, angle);

        // reverse the rotation on the origin
        _origin.Transform = Transform3D.Identity.Rotated(Vector3.Up, -angle) * _origin.Transform;

        // move
        var characterPosition = GlobalPosition;
        var bodyLocation = _origin.Transform * _camera.Transform * _neck.Position;
        bodyLocation.Y = 0.0f;
        bodyLocation = GlobalTransform * bodyLocation;

        // move
        Velocity = (bodyLocation - characterPosition) / (float)delta;
        MoveAndSlide();

        // move the origin back
        var deltaMovement = GlobalPosition - characterPosition;
        _origin.GlobalPosition -= deltaMovement;

        // negate height changes
        _origin.Position = _origin.Position with { Y = 0.0f };

        // reset velocity
        Velocity = currentVelocity;

        var offset = (bodyLocation - GlobalPosition).LengthSquared();
        return offset > 0.01;
    }

    private void ProcessRotationOnInput(double delta)
    {
        if(XrManager.Instance.IsXrInitialized) {
            var previousInput = _input.PreviousLookState;
            var input = _input.LookState;

            // "click" style look
            if(previousInput.X < 0.8 && input.X > 0.8) {
                RotateY(Mathf.DegToRad(-30.0f));
            } else if(previousInput.X > -0.8 && input.X < -0.8) {
                RotateY(Mathf.DegToRad(30.0f));
            }
        } else {
            var input = _input.LookState;

            _camera.RotateX(input.Y * _lookSensitivity * (float)delta);
            _camera.Rotation = _camera.Rotation with { X = Mathf.Clamp(_camera.Rotation.X, _tiltLowerLimit, _tiltUpperLimit) };

            RotateY(-input.X * _lookSensitivity * (float)delta);
        }
    }

    private void ProcessMovementOnInput(double delta)
    {
        // apply gravity
        var velocity = Velocity;
        velocity.Y -= (float)(_gravity * delta);

        // move
        var input = _input.MoveState;
        var direction = GlobalBasis * new Vector3(input.X, 0, input.Y);
        if(direction.LengthSquared() > 0.0f) {
            velocity.X = direction.X * _moveSpeed;
            velocity.Z = direction.Z * _moveSpeed;
        } else {
            velocity.X = velocity.Z = 0.0f;
        }
        Velocity = velocity;
        MoveAndSlide();
    }
}
