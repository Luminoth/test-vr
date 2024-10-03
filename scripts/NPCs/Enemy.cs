namespace VrTest.NPCs;

public partial class Enemy : CharacterBody3D
{
    [Export]
    private float _moveSpeed = 5.0f;

    public float MoveSpeed => _moveSpeed;

    [Export]
    private float _jumpSpeed = 5.0f;

    public float JumpSpeed => _jumpSpeed;

    [Export]
    private float _gravityModifier = 1.0f;

    public float GravityModifier => _gravityModifier;

    [Export]
    private float _terminalVelocity = 100.0f;

    public float TermainalVelocity => _terminalVelocity;

    [Export]
    private float _maxShoveVelocity = 8.0f;

    private float _gravity;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;

        // apply gravity
        velocity = velocity with {
            Y = Mathf.Clamp(
                Velocity.Y - (float)(_gravity * GravityModifier * delta),
                -TermainalVelocity,
                TermainalVelocity
            )
        };

        if(IsOnFloor()) {
            velocity.X = Mathf.MoveToward(velocity.X, 0.0f, MoveSpeed * 10.0f * (float)delta);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0.0f, MoveSpeed * 10.0f * (float)delta);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    #endregion

    public void Shove(Vector3 velocity)
    {
        velocity = velocity.LimitLength(_maxShoveVelocity);

        Velocity += velocity;
    }
}
