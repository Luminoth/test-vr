using VrTest.Player.Input;

namespace VrTest.Player;

public partial class PlayerModel : Node3D
{
    [Export]
    private XrInput _input;

    [Export]
    private Node3D _head;

    [Export]
    private Node3D _leftArm;

    public Node3D LeftArm => _leftArm;

    [Export]
    private Node3D _rightArm;

    public Node3D RightArm => _rightArm;

    #region Godot Lifecycle

    public override void _Process(double delta)
    {
        // TODO: this is null on the remote player right now
        // but that entire thing should go away when the
        // XROrigin is moved to its own thing for multiplayer
        if(_input != null) {
            TrackHand(_input.LeftHand, LeftArm);
            TrackHand(_input.RightHand, RightArm);
        }
    }

    #endregion

    public void ShowHead(bool show)
    {
        if(show) {
            _head.Show();
        } else {
            _head.Hide();
        }
    }

    private void TrackHand(PlayerHand hand, Node3D arm)
    {
        arm.LookAt(hand.GlobalPosition, Vector3.Up);
    }
}
