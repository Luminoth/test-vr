using VrTest.Managers;

namespace VrTest.Player;

// movement scripts need to be above the player character script
// in tree order so that they execute first
public partial class GorillaMovement : Node
{
    [Export]
    private bool _enabled = true;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = XrManager.Instance.IsXrInitialized && value;

            SetProcess(_enabled);
            SetPhysicsProcess(_enabled);
        }
    }

    [Export]
    private XrPlayerCharacter _character;

    private double _gravity;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();

        SetProcess(XrManager.Instance.IsXrInitialized && Enabled);
        SetPhysicsProcess(XrManager.Instance.IsXrInitialized && Enabled);
    }

    public override void _PhysicsProcess(double delta)
    {
        // rotation in the physics step
        // because hands will move from this
        ApplyRotation((float)delta);

        ApplyMovement((float)delta);
    }

    #endregion

    private void ApplyRotation(float delta)
    {
        // match the character Y rotation to the camera
        _character.GlobalRotation = _character.GlobalRotation with { Y = _character.Player.Camera.GlobalRotation.Y };
    }

    private void ApplyMovement(float delta)
    {
        // attempt to move the character to be under the camera on the X/Z plane
        var currentPosition = _character.GlobalPosition with { Y = 0.0f };
        var desiredPosition = _character.Player.Camera.GlobalPosition with { Y = 0.0f };
        var currentVelocity = _character.Velocity;
        _character.Velocity = (desiredPosition - currentPosition) / delta;
        _character.MoveAndSlide();
        _character.Velocity = currentVelocity;

        // move the origin back to match if we didn't make it
        var remaining = desiredPosition - _character.GlobalPosition with { Y = 0.0f };
        _character.Player.GlobalPosition -= remaining;
    }
}
