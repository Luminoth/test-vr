using VrTest.Managers;
using VrTest.UI;

namespace VrTest.Player;

public partial class PlayerCharacter : CharacterBody3D
{
    [Export]
    private PlayerModel _model;

    public PlayerModel Model => _model;

    [Export]
    private XrUIHelper _hud;

    [Export]
    private CollisionShape3D _collider;

    private float Height => ((CapsuleShape3D)_collider.Shape).Height;

    private float Radius => ((CapsuleShape3D)_collider.Shape).Radius;

    [Export]
    private float _eyeForwardOffset = 0.5f;

    //public float EyeForwardOffset => _eyeForwardOffset;

    [Export]
    private float _eyeHeightOffset = 0.1f;

    //public float EyeHeightOffset => _eyeHeightOffset;

    private float EyeHeight => Height - _eyeHeightOffset;

    [Export]
    private float _moveSpeed = 5.0f;

    public float MoveSpeed => _moveSpeed;

    [Export]
    private float _jumpSpeed = 5.0f;

    //public float JumpSpeed => _jumpSpeed;

    [Export]
    private bool _allowAirControl;

    public bool AllowAirControl => _allowAirControl;

    [Export]
    private float _gravityModifier = 2.0f;

    //public float GravityModifier => _gravityModifier;

    [Export]
    private float _terminalSpeed = 100.0f;

    //public float TerminalSpeed => _terminalSpeed;

    #region Godot Lifecycle

    public override void _Ready()
    {
        var origin = XrManager.Instance.XrPlayer;
        Model.LeftHand.Controller = origin.LeftHand;
        Model.RightHand.Controller = origin.RightHand;

        Model.ShowHead(false);

        origin.InitHud(_hud);

        GD.Print($"Player height: {Height}");
        GD.Print($"Player eye height: {EyeHeight}");
    }

    public override void _PhysicsProcess(double delta)
    {
        var eyeOffset = GlobalBasis * new Vector3(0.0f, 0.0f, -_eyeForwardOffset);
        var origin = XrManager.Instance.XrPlayer;

        if(XrManager.Instance.IsXrInitialized) {
            // offset the character so the camera is at the eye
            // maybe this could move the origin instead
            // but this actually keeps the camera "in bounds" so is probably correct
            var eyeOffset = GlobalBasis * new Vector3(0.0f, 0.0f, -_eyeForwardOffset);
            GlobalPosition -= eyeOffset;

            origin.ResetHeight(GlobalPosition.Y + EyeHeight);
        } else {
            var origin = XrManager.Instance.XrPlayer;

            // rotate the character to match the origin
            GlobalRotation = origin.GlobalRotation;

            // move the origin to match the body
            // but just a little in front to match the eyes
            origin.GlobalPosition = GlobalPosition + eyeOffset;
        }
    }

    #endregion

    public void ApplyGravity(float gravity, float delta)
    {
        Velocity = Velocity with {
            Y = Mathf.Clamp(
                Velocity.Y - (float)(gravity * _gravityModifier * delta),
                -_terminalSpeed,
                _terminalSpeed
            )
        };
    }

    public void Jump()
    {
        if(!IsOnFloor()) {
            return;
        }

        var velocity = Vector3.Up * _jumpSpeed;
        Velocity += velocity;
    }

    public void JumpWithVelocity(Vector3 velocity)
    {
        if(!IsOnFloor()) {
            return;
        }

        // clamp the velocity we add
        var verticalVelocity = new Vector3(0.0f, Mathf.Clamp(velocity.Y, 0.0f, _jumpSpeed), 0.0f);

        // NOTE: controller movement will reset this to match actual input
        var horizontalVelocity = new Vector3(velocity.X, 0.0f, velocity.Z).LimitLength(MoveSpeed);

        Velocity += verticalVelocity + horizontalVelocity;
    }
}
