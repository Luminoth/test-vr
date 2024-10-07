using VrTest.Managers;

namespace VrTest.Player.Movement;

// movement scripts need to be above the player character script
// in tree order so that they execute first
public abstract partial class Movement : Node
{
    protected abstract bool IsXrMovement { get; }

    [Export]
    private bool _enabled = true;

    public bool IsEnabled
    {
        get => _enabled;
        set
        {
            _enabled = IsXrEnabled && value;

            SetProcess(_enabled);
            SetPhysicsProcess(_enabled);
        }
    }

    private bool IsXrEnabled => IsXrMovement == XrManager.Instance.IsXrInitialized;

    [Export]
    private PlayerCharacter _character;

    protected PlayerCharacter Character => _character;

    private float _gravity;

    protected float Gravity => _gravity;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();

        // reset enabled to include the IsXrEnabled check
        IsEnabled = _enabled;

        SetProcess(IsEnabled);
        SetPhysicsProcess(IsEnabled);
    }

    public override void _PhysicsProcess(double delta)
    {
        ApplyRotation((float)delta);
        ApplyMovement((float)delta);
    }

    #endregion

    public abstract void ApplyRotation(float delta);

    protected abstract void ApplyMovement(float delta);

    protected void ApplyPhysicalRotation()
    {
        // match the character Y rotation to the camera
        Character.GlobalRotation = Character.GlobalRotation with { Y = Character.Player.Camera.GlobalRotation.Y };
    }

    protected void ApplyPhysicalMovement(float delta)
    {
        // attempt to move the character to be under the camera on the X/Z plane
        var currentPosition = Character.GlobalPosition with { Y = 0.0f };
        var desiredPosition = Character.Player.Camera.GlobalPosition with { Y = 0.0f };
        var currentVelocity = Character.Velocity;
        Character.Velocity = (desiredPosition - currentPosition) / delta;
        Character.MoveAndSlide();
        Character.Velocity = currentVelocity;

        // move the origin back to match if we didn't make it
        var remaining = desiredPosition - Character.GlobalPosition with { Y = 0.0f };
        Character.Player.GlobalPosition -= remaining;
    }

    protected void UpdateOrigin(Vector3 previousPosition)
    {
        // move the origin to match the character movement on the X/Z plane
        var distance = Character.GlobalPosition with { Y = 0.0f } - previousPosition with { Y = 0.0f };
        Character.Player.GlobalPosition += distance;
    }
}
