using VrTest.Managers;

namespace VrTest.Player;

public partial class PlayerModel : Node3D
{
    [Export]
    private Node3D _headModel;

    [Export]
    private Node3D _leftArmModel;

    [Export]
    private PlayerHand _leftHand;

    public PlayerHand LeftHand => _leftHand;

    [Export]
    private Node3D _rightArmModel;

    [Export]
    private PlayerHand _rightHand;

    public PlayerHand RightHand => _rightHand;

    private static void TrackHand(PlayerHand hand, Node3D arm)
    {
        arm.LookAt(hand.GlobalPosition, Vector3.Up);
    }

    #region Godot Lifecycle

    public override void _Process(double delta)
    {
        TrackHand(_leftHand, _leftArmModel);
        TrackHand(_rightHand, _rightArmModel);
    }

    #endregion

    public void ShowHead(bool show)
    {
        if(show) {
            _headModel.Show();
        } else {
            _headModel.Hide();
        }
    }
}
