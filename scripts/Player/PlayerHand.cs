namespace VrTest.Player;

// this goes on the chararacter under the origin
// so that we can order scripts correctly
public partial class PlayerHand : XRController3D
{
    [Signal]
    public delegate void collisionEventHandler(Node3D body);

    [Export]
    private PlayerCharacter _character;

    // https://forum.godotengine.org/t/rigid-bodies-as-hands/67646
    [Export]
    AnimatableBody3D _handBody;

    [Export]
    private int _trackedVelocityCount = 5;

    private Vector3[] _trackedVelocities;

    private int _nextVelocityIdx = 0;

    [Export]
    private Vector3 _trackedVelocity;

    public Vector3 TrackedVelocity => _trackedVelocity;

    private Vector3 _previousPosition;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _trackedVelocities = new Vector3[_trackedVelocityCount];

        _previousPosition = GlobalPosition;

        _handBody.SyncToPhysics = false;
        _handBody.TopLevel = true;
        _handBody.GlobalPosition = GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = (GlobalPosition - _previousPosition) / (float)delta;
        _previousPosition = GlobalPosition;

        UpdateTrackedVelocity(velocity);

        // try and move our virtual hands
        // https://docs.godotengine.org/en/stable/tutorials/physics/using_character_body_2d.html
        var handDistance = GlobalPosition - _handBody.GlobalPosition;
        var collision = _handBody.MoveAndCollide(handDistance);
        if(collision != null && collision.GetCollider() is Node3D body) {
            EmitSignal(SignalName.collision, body);

            // TODO: we don't want to do this if we hit an enemy
            ResetTrackedVelocity();
        }
    }

    #endregion

    private void UpdateTrackedVelocity(Vector3 velocity)
    {
        _trackedVelocities[_nextVelocityIdx] = velocity;
        _nextVelocityIdx = (_nextVelocityIdx + 1) % _trackedVelocities.Length;

        var sum = Vector3.Zero;
        for(int i = 0; i < _trackedVelocities.Length; ++i) {
            sum += _trackedVelocities[i];
        }

        _trackedVelocity = sum / _trackedVelocities.Length;
    }

    private void ResetTrackedVelocity()
    {
        System.Array.Clear(_trackedVelocities);
        _trackedVelocity = Vector3.Zero;
    }
}
