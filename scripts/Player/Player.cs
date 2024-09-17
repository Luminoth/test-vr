using VrTest.Managers;

namespace VrTest.Player;

public partial class Player : CharacterBody3D
{
    [Export]
    private XROrigin3D _origin;

    [Export]
    private PlayerInput _input;

    [Export]
    private float _moveSpeed = 5.0f;

    public override void _PhysicsProcess(double delta)
    {
        if(!XrManager.Instance.IsXrInitialized) {
            // TODO: handle right stick look
        }

        var moveInput = new Vector3(_input.MoveDirection.X, 0, _input.MoveDirection.Y);
        var velocity = (GlobalTransform.Basis * moveInput).Normalized();
        if(moveInput.IsZeroApprox()) {
            Velocity = Velocity with { X = 0.0f, Z = 0.0f };
        } else {
            Velocity = Velocity with { X = velocity.X * _moveSpeed, Z = velocity.Z * _moveSpeed };
        }

        MoveAndSlide();
    }
}
