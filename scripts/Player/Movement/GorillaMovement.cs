using VrTest.NPCs;
using VrTest.Player.Input;

namespace VrTest.Player.Movement;

// movement scripts need to be above the player character script
// in tree order so that they execute first
public partial class GorillaMovement : Movement
{
    protected override bool IsXrMovement => true;

    [Export]
    private XrInput _input;

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

    private void HandleHandCollision(PlayerHand hand, Node3D body)
    {
        // remove character movement from the hand velocity
        var collisionVelocity = hand.Velocity - Character.Velocity;

        if(body is Enemy enemy) {
            enemy.Shove(collisionVelocity);
        } else {
            Character.JumpWithVelocity(-collisionVelocity);
        }
    }

    #region Signal Handlers

    private void _on_left_hand_collision(Node3D body)
    {
        HandleHandCollision(_input.LeftHand, body);
    }

    private void _on_right_hand_collision(Node3D body)
    {
        HandleHandCollision(_input.RightHand, body);
    }

    #endregion
}
