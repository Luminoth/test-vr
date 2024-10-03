namespace VrTest.Player;

public partial class PlayerModel : Node3D
{
    [Export]
    private Node3D _head;

    [Export]
    private Node3D _leftArm;

    public Node3D LeftArm => _leftArm;

    [Export]
    private Node3D _rightArm;

    public Node3D RightArm => _rightArm;

    public void ShowHead(bool show)
    {
        if(show) {
            _head.Show();
        } else {
            _head.Hide();
        }
    }
}
