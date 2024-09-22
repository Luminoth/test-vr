using VrTest.Managers;

namespace VrTest.Player;

// this goes on the chararacter under the origin
// so that we can order scripts correctly
public partial class PlayerCharacter : CharacterBody3D
{
    [Export]
    private XROrigin3D _origin;

    public XROrigin3D Origin => _origin;

    [Export]
    private XRCamera3D _camera;

    public XRCamera3D Camera => _camera;

    [Export]
    private Node3D _head;

    [Export]
    private Label _fpsLabel;

    [Export]
    private float _playerHeight = 1.8f;

    public float PlayerHeight => _playerHeight;

    [Export]
    private float _moveSpeed = 5.0f;

    public float MoveSpeed => _moveSpeed;

    #region Godot Lifecycle

    public override void _Ready()
    {
        TopLevel = true;

        // TODO: local player hide
        // remote player show
        _head.Hide();
    }

    public override void _Process(double delta)
    {
        _fpsLabel.Text = $"FPS: {Engine.GetFramesPerSecond()}";
    }

    public override void _PhysicsProcess(double delta)
    {
        if(XrManager.Instance.IsXrInitialized) {
            // rotate character to match the camera rotation
            GlobalRotation = GlobalRotation with { Y = Camera.GlobalRotation.Y };

            // move the origin to match the body
            // but just a little in front to match the eyes
            Origin.GlobalPosition = GlobalPosition + (GlobalBasis * new Vector3(0.0f, 0.0f, -0.5f));

            // move the origin to fix the camera at the player height
            // minus a little bit to be at the eye position
            // (assuming Local reference space here, Local Floor and Stage shouldn't do this)
            Origin.GlobalPosition = Origin.GlobalPosition with { Y = GlobalPosition.Y + PlayerHeight - Camera.Position.Y - 0.1f };
        } else {
            // rotate the character to match the origin
            GlobalRotation = Origin.GlobalRotation;

            // move the origin to match the body
            // but just a little in front to match the eyes
            Origin.GlobalPosition = GlobalPosition + (GlobalBasis * new Vector3(0.0f, 0.0f, -0.5f));
        }
    }

    #endregion
}
