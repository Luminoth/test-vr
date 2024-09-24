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

    #region Godot Lifecycle

    public override void _Ready()
    {
        TopLevel = true;

        _head.Hide();
    }

    public override void _PhysicsProcess(double delta)
    {
        if(XrManager.Instance.IsXrInitialized) {
            // rotate character to match the camera rotation
            GlobalRotation = GlobalRotation with { Y = Player.Camera.GlobalRotation.Y };

            // move the origin so the camera is on the body
            // but just a little in front to match the eyes
            var eyeOffset = GlobalBasis * new Vector3(0.0f, 0.0f, -Player.EyeForwardOffset);
            Player.GlobalPosition = GlobalPosition - Player.Camera.Position + eyeOffset;

            // move the origin to fix the camera at the player height
            // minus a little bit to be at the eye position
            // (assuming Local reference space here, Local Floor and Stage shouldn't do this)
            Player.GlobalPosition = Player.GlobalPosition with { Y = GlobalPosition.Y + Player.Height - Player.Camera.Position.Y - Player.EyeHeightOffset };
        } else {
            // rotate the character to match the origin
            GlobalRotation = Player.GlobalRotation;

            // move the origin to match the body
            // but just a little in front to match the eyes
            Player.GlobalPosition = GlobalPosition + (GlobalBasis * new Vector3(0.0f, 0.0f, -Player.EyeForwardOffset));
        }
    }

    #endregion
}
