using VrTest.NPCs;

namespace VrTest.Player;

// this goes on the chararacter under the origin
// so that we can order scripts correctly
public partial class PlayerHand : XRController3D
{
    [Export]
    private XrPlayerCharacter _character;

    // https://forum.godotengine.org/t/rigid-bodies-as-hands/67646
    [Export]
    AnimatableBody3D _handBody;

    private Vector3 _velocity;

    public Vector3 LastVelocity => _velocity;

    private Vector3 _previousPosition;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _previousPosition = GlobalPosition;

        _handBody.SyncToPhysics = false;
        _handBody.TopLevel = true;
        _handBody.GlobalPosition = GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        _velocity = (GlobalPosition - _previousPosition) / (float)delta;
        _previousPosition = GlobalPosition;

        // try and move our virtual hands
        // https://docs.godotengine.org/en/stable/tutorials/physics/using_character_body_2d.html
        var handDistance = GlobalPosition - _handBody.GlobalPosition;
        var collision = _handBody.MoveAndCollide(handDistance);
        if(collision != null && collision.GetCollider() is Node3D body) {
            if(body.GetParent() is Enemy enemy) {
                // TODO:
            } else {
                _character.JumpWithVelocity(-_velocity);
            }
        }
    }

    #endregion
}
