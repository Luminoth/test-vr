namespace VrTest.Player.Movement;

// movement scripts need to be above the player character script
// in tree order so that they execute first
public partial class GorillaMovement : Movement
{
    protected override bool IsXrMovement => true;

    protected override void ApplyRotation(float delta)
    {
        ApplyPhysicalRotation();
    }

    protected override void ApplyMovement(float delta)
    {
        ApplyPhysicalMovement(delta);

        var currentPosition = Character.GlobalPosition;

        Character.ApplyGravity(Gravity, delta);

        var didCollide = Character.MoveAndSlide();

        // prevent sliding on the ground
        if(Character.IsOnFloor() && didCollide) {
            Character.Velocity = Character.Velocity with { X = 0.0f, Z = 0.0f };
        }

        UpdateOrigin(currentPosition);
    }
}
