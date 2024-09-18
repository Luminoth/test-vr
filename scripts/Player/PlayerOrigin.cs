namespace VrTest.Player;

// origin-centric player
// https://docs.godotengine.org/en/stable/tutorials/xr/xr_room_scale.html
// TODO: https://github.com/godotengine/godot-demo-projects/blob/master/xr/openxr_origin_centric_movement/player.gd is more complete than this
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
        var t1 = new Transform3D();
        var t2 = new Transform3D();
        var rot = new Transform3D();

        // rotate origin around the player
        var position = _character.GlobalPosition - GlobalPosition;

        t1.Origin = -position;
        t2.Origin = position;
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
        var movement = _character.GlobalBasis * new Vector3(input.X, 0.0f, input.Y);
        velocity.X = movement.X * _moveSpeed;
        velocity.Y = movement.Y;
        _character.Velocity = velocity * _moveSpeed;
        _character.MoveAndSlide();

        // apply movement to origin
        GlobalPosition += _character.GlobalPosition - characterPosition;
    }
}
