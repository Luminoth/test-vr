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
    private float _rotationSpeed = 1.0f;

    [Export]
    private float _moveSpeed = 5.0f;

    private double _gravity;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();

        Recenter();
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

    public void Recenter()
    {
        GD.Print("Recentering ...");

        // where should the camera be
        var newCameraTransform = GlobalTransform;

        // height at neck join
        newCameraTransform.Origin.Y = _neck.GlobalPosition.Y;

        // apply the transform
        newCameraTransform = newCameraTransform * _neck.Transform.Inverse();

        // remove tilt
        var cameraTransform = _camera.Transform;
        var forward = _camera.Basis.Z;
        forward.Y = 0.0f;
        cameraTransform = cameraTransform.LookingAt(cameraTransform.Origin + forward.Normalized(), Vector3.Up, true);

        // update XR location
        _origin.GlobalTransform = newCameraTransform * cameraTransform.Inverse();
    }

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
        Velocity = (bodyLocation - characterPosition) / (float)delta;
        MoveAndSlide();

        // move the origin back
        var deltaMovement = GlobalPosition - characterPosition;
        _origin.GlobalPosition -= deltaMovement;

        // negate height changes
        var position = _origin.Position;
        position.Y = 0.0f;
        _origin.Position = position;

        // reset velocity
        Velocity = currentVelocity;

        var offset = (bodyLocation - GlobalPosition).LengthSquared();
        return offset > 0.01;
    }

    private void ProcessRotationOnInput(double delta)
    {
        var rot = Rotation;
        // TODO:
        //rot.Y += _input.LookDirection * (float)delta;
        Rotation = rot;
    }

    private void ProcessMovementOnInput(double delta)
    {
        var input = _input.MoveDirection;
        var direction = GlobalBasis * new Vector3(input.X, 0, input.Y);
        var velocity = Velocity;
        if(direction.LengthSquared() > 0.0f) {
            velocity.X = direction.X * _moveSpeed;
            velocity.Z = direction.Z * _moveSpeed;
        } else {
            // slow down
            velocity.X = Mathf.MoveToward(velocity.X, 0.0f, (float)delta);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0.0f, (float)delta);
        }
        Velocity = velocity;

        MoveAndSlide();
    }
}
