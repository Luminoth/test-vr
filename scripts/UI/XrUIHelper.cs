using VrTest.Managers;

namespace VrTest.UI;

// put this node under the object to track against
// the OpenXRCompositionLayer goes under the XROrigin3D
// the MeshInstance3D goes under this node
// the MeshInstance3D Mesh and Mesh Material must be marked as "Local to Scene"
public partial class XrUIHelper : Node3D
{
    [Export]
    private OpenXRCompositionLayer _xrNode;

    [Export]
    private MeshInstance3D _noXrNode;

    #region Godot Lifecycle

    public override void _Ready()
    {
        if(XrManager.Instance.IsXrInitialized) {
            _xrNode.Show();
            _noXrNode.Hide();
        } else {
            _xrNode.Hide();
            _noXrNode.Show();
        }
    }

    public override void _Process(double delta)
    {
        _xrNode.GlobalPosition = GlobalPosition;
        _xrNode.GlobalRotation = GlobalRotation;
    }

    #endregion
}
