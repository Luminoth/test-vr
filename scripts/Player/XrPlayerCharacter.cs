using VrTest.Managers;

namespace VrTest.Player;

// this goes on the chararacter under the origin
// so that we can order scripts correctly
public partial class XrPlayerCharacter : CharacterBody3D
{
    [Export]
    private XrPlayer _player;

    public XrPlayer Player => _player;

    [Export]
    private Node3D _head;

    private float EyeHeight => Player.Height - Player.EyeHeightOffset;

    #region Godot Lifecycle

    public override void _Ready()
    {
        TopLevel = true;

        _head.Hide();

        GD.Print($"Player eye height: {EyeHeight}");
    }

    public override void _PhysicsProcess(double delta)
    {
        if(XrManager.Instance.IsXrInitialized) {
            // offset the character so the camera is at the eye
            // maybe this could move the origin instead
            // but this actually keeps the camera "in bounds" so is probably correct
            var eyeOffset = GlobalBasis * new Vector3(0.0f, 0.0f, -Player.EyeForwardOffset);
            GlobalPosition -= eyeOffset;

            // move the origin to fix the camera at the player height
            // minus a little bit to be at the eye position
            // (assuming Local reference space here, Local Floor and Stage shouldn't do this)
            Player.GlobalPosition = Player.GlobalPosition with { Y = GlobalPosition.Y + EyeHeight - Player.Camera.Position.Y };
        } else {
            // rotate the character to match the origin
            GlobalRotation = Player.GlobalRotation;

            // move the origin to match the body
            // but just a little in front to match the eyes
            Player.GlobalPosition = GlobalPosition + (GlobalBasis * new Vector3(0.0f, 0.0f, -Player.EyeForwardOffset));
        }
    }

    #endregion

    public void Jump()
    {
        Velocity += Vector3.Up * Player.JumpSpeed;
    }

    public void JumpWithVelocity(Vector3 velocity)
    {
        Velocity += velocity * Player.JumpSpeed;
        GD.Print($"jump updated velocity: {Velocity}");
    }

    // from XRToools, rotates the origin around the camera
    public void RotatePlayer(float angle)
    {
        var t1 = Transform3D.Identity;
        var t2 = Transform3D.Identity;
        var rot = Transform3D.Identity;

        t1.Origin = -Player.Camera.Position;
        t2.Origin = Player.Camera.Position;
        rot = rot.Rotated(Vector3.Down, angle);

        Player.Transform = (Player.Transform * t2 * rot * t1).Orthonormalized();
    }
}
