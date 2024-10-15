using VrTest.NPCs;

namespace VrTest.Player.Movement.XR;

// movement scripts need to be above the player character script
// in tree order so that they execute first
// TODO: this still needs a lot of work and I'm not entirely sure where
public partial class GorillaMovement : Movement
{
    protected override bool IsXrMovement => true;

    #region Godot Lifecycle

    public override void _Ready()
    {
        base._Ready();

        Character.Model.LeftHand.collision += _on_left_hand_collision;
        Character.Model.RightHand.collision += _on_right_hand_collision;
    }

    public override void _ExitTree()
    {
        Character.Model.LeftHand.collision -= _on_left_hand_collision;
        Character.Model.RightHand.collision -= _on_right_hand_collision;
    }

    #endregion

    public override void ApplyRotation(float delta)
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
        var collisionVelocity = hand.TrackedVelocity - Character.Velocity;

        if(body is Enemy enemy) {
            // TODO: this doesn't belong in the movement
            // it should be part of the character
            // (so we can slap stuff without this movement)
            enemy.Shove(collisionVelocity);
        } else {
            Character.JumpWithVelocity(-collisionVelocity);
        }
    }

    #region Signal Handlers

    private void _on_left_hand_collision(PlayerHand hand, Node3D body)
    {
        if(IsEnabled) {
            HandleHandCollision(hand, body);
        }
    }

    private void _on_right_hand_collision(PlayerHand hand, Node3D body)
    {
        if(IsEnabled) {
            HandleHandCollision(hand, body);
        }
    }

    #endregion
}
