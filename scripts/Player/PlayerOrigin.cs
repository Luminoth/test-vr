namespace VrTest.Player;

// origin-centric player
// https://docs.godotengine.org/en/stable/tutorials/xr/xr_room_scale.html
// TODO: https://github.com/godotengine/godot-demo-projects/blob/master/xr/openxr_origin_centric_movement/player.gd is more complete than this
// TODO: this is untested
public partial class PlayerOrigin : XROrigin3D
{
    [Export]
    private CharacterBody3D _character;

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
        var newCameraTransform = _character.GlobalTransform;

        // height at neck join
        newCameraTransform.Origin.Y = _neck.GlobalPosition.Y;

        // apply the transform
        newCameraTransform *= _neck.Transform.Inverse();

        // remove tilt
        var cameraTransform = _camera.Transform;
        var forward = _camera.Basis.Z;
        forward.Y = 0.0f;
        cameraTransform = cameraTransform.LookingAt(cameraTransform.Origin + forward.Normalized(), Vector3.Up, true);

        // update XR location
        GlobalTransform = newCameraTransform * cameraTransform.Inverse();

        // recenter character
        _character.Transform = Transform3D.Identity;
    }

    private bool ProcessOnPhysicalMovement(double delta)
    {
        var currentVelocity = _character.Velocity;
        var characterPosition = _character.GlobalPosition;

        // where should we end up
        var bodyLocation = _camera.Transform * _neck.Position;
        bodyLocation.Y = 0.0f;
        bodyLocation = GlobalTransform * bodyLocation;

        // move
        _character.Velocity = (bodyLocation - characterPosition) / (float)delta;
        _character.MoveAndSlide();

        // reset velocity
        _character.Velocity = currentVelocity;

        // check if we collided (ignoring height change)
        var movementRemaining = bodyLocation - _character.GlobalPosition;
        movementRemaining.Y = 0.0f;
        return movementRemaining.LengthSquared() > 0.01;
    }

    private void CopyPlayerRotationToCharacter()
    {
        // only copy forward direction, ignoring tilt
        var cameraForward = -_camera.GlobalBasis.Z;
        var bodyForward = new Vector3(cameraForward.X, 0.0f, cameraForward.Z);

        _character.GlobalBasis = Basis.LookingAt(bodyForward, Vector3.Up);
    }

    private void ProcessRotationOnInput(double delta)
    {
        var t1 = Transform3D.Identity;
        var t2 = Transform3D.Identity;
        var rot = Transform3D.Identity;

        // rotate origin around the player
        var position = _character.GlobalPosition - GlobalPosition;

        t1.Origin = -position;
        t2.Origin = position;
        // TODO:
        //rot = rot.Rotated(Vector3.Up, _input.LookDirection * (float)delta);
        GlobalTransform = (GlobalTransform * t2 * rot * t1).Orthonormalized();

        // face body the correct way
        CopyPlayerRotationToCharacter();
    }

    private void ProcessMovementOnInput(double delta)
    {
        var characterPosition = _character.GlobalPosition;

        // apply gravity
        var velocity = _character.Velocity;
        velocity.Y -= (float)(_gravity * delta);

        // move
        var input = _input.MoveDirection;
        var direction = _character.GlobalBasis * new Vector3(input.X, 0.0f, input.Y);
        if(direction.LengthSquared() > 0.0f) {
            velocity.X = direction.X * _moveSpeed;
            velocity.Z = direction.Z * _moveSpeed;
        } else {
            // slow down
            velocity.X = Mathf.MoveToward(velocity.X, 0.0f, (float)delta);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0.0f, (float)delta);
        }
        _character.Velocity = velocity;
        _character.MoveAndSlide();

        // apply movement to origin
        GlobalPosition += _character.GlobalPosition - characterPosition;
    }
}
