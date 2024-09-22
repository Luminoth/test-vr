using VrTest.Managers;

namespace VrTest.Player;

public partial class Player : XROrigin3D
{
    [Export]
    private CharacterBody3D _character;

    public CharacterBody3D Character => _character;

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
        Character.TopLevel = true;

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
            Character.GlobalRotation = Character.GlobalRotation with { Y = Camera.GlobalRotation.Y };

            // move the origin to match the body
            // but just a little in front to match the eyes
            GlobalPosition = Character.GlobalPosition + (GlobalBasis * new Vector3(0.0f, 0.0f, -0.5f));

            // move the origin to fix the camera at the player height
            // minus a little bit to be at the eye position
            // (assuming Local reference space here, Local Floor and Stage shouldn't do this)
            GlobalPosition = GlobalPosition with { Y = GlobalPosition.Y + PlayerHeight - Camera.Position.Y - 0.1f };
        } else {
            // rotate the character to match the origin
            Character.GlobalRotation = GlobalRotation;

            // move the origin to match the body
            // but just a little in front to match the eyes
            GlobalPosition = Character.GlobalPosition + (GlobalBasis * new Vector3(0.0f, 0.0f, -0.5f));
        }
    }

    #endregion
}
