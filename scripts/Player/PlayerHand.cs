using VrTest.NPCs;

namespace VrTest.Player;

// this goes on the chararacter under the origin
// so that we can order scripts correctly
public partial class PlayerHand : XRController3D
{
    [Export]
    private XrPlayerCharacter _character;

    [Export]
    private Vector3 _velocity;

    public Vector3 Velocity => _velocity;

    private Vector3 _previousPosition;

    #region Godot Lifecycle

    public override void _PhysicsProcess(double delta)
    {
        _velocity = (GlobalPosition - _previousPosition) / (float)delta;

        _previousPosition = GlobalPosition;
    }

    #endregion

    #region Signal Handlers

    private void _on_area_3d_body_entered(Node3D body)
    {
        if(body.GetParent() is Enemy enemy) {
            GD.Print($"{Name} collision with enemy {enemy.Name} at {_velocity}");
        } else {
            GD.Print($"{Name} collision with {body.GetParent().Name} at {_velocity}");

            _character.JumpWithVelocity(-_velocity);
        }
    }

    #endregion
}
