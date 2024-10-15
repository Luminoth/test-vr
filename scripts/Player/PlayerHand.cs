namespace VrTest.Player;

// this goes on the chararacter under the origin
// so that we can order scripts correctly
public partial class PlayerHand : Node3D
{
    [Signal]
    public delegate void collisionEventHandler(PlayerHand hand, Node3D body);

    [Export]
    private Node3D _model;

    public Node3D Model => _model;

    // https://forum.godotengine.org/t/rigid-bodies-as-hands/67646
    [Export]
    AnimatableBody3D _handBody;

    private XRController3D _controller;

    [CanBeNull]
    public XRController3D Controller
    {
        get => _controller;

        set
        {
            _controller = value;

            _previousControllerPosition = _controller.GlobalPosition;

            GlobalPosition = _controller.GlobalPosition;
            _handBody.Position = Vector3.Zero;
        }
    }

    [Export]
    private int _trackedVelocityCount = 5;

    private Vector3[] _trackedVelocities;

    private int _nextVelocityIdx = 0;

    [Export]
    private Vector3 _trackedVelocity;

    public Vector3 TrackedVelocity => _trackedVelocity;

    private Vector3 _previousControllerPosition;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _trackedVelocities = new Vector3[_trackedVelocityCount];

        _handBody.SyncToPhysics = false;
        _handBody.GlobalPosition = GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = (Controller.GlobalPosition - _previousControllerPosition) / (float)delta;
        _previousControllerPosition = Controller.GlobalPosition;

        UpdateTrackedVelocity(velocity);

        // try and move our virtual hands
        // https://docs.godotengine.org/en/stable/tutorials/physics/using_character_body_2d.html
        var handDistance = Controller.GlobalPosition - _handBody.GlobalPosition;
        var collision = _handBody.MoveAndCollide(handDistance);
        if(collision != null && collision.GetCollider() is Node3D body) {
            EmitSignal(SignalName.collision, this, body);

            // TODO: we don't want to do this if we hit an enemy
            ResetTrackedVelocity();
        }

        GlobalPosition = _handBody.GlobalPosition;
        _handBody.Position = Vector3.Zero;
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
