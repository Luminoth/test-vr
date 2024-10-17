using VrTest.Managers;

namespace VrTest.UI;

// put this node under the object to follow
// the OpenXRCompositionLayer goes under the XROrigin3D
// the MeshInstance3D goes under this node
// the MeshInstance3D Mesh and Mesh Material must be marked as "Local to Scene"
public partial class XrUIHelper : Node3D
{
    [Export]
    private SubViewport _viewport;

    public SubViewport Viewport => _viewport;

    [Export]
    private MeshInstance3D _noXrUI;

    private OpenXRCompositionLayer _xrUI;

    public OpenXRCompositionLayer XrUI
    {
        get => _xrUI;

        set
        {
            _xrUI = value;

            if(XrManager.Instance.IsXrInitialized) {
                _xrUI.Show();
            } else {
                _xrUI.Hide();
            }
        }
    }

    private Vector3 _offset;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _offset = Position;

        if(XrManager.Instance.IsXrInitialized) {
            _noXrUI.Hide();
        } else {
            _noXrUI.Show();
        }
    }

    public override void _Process(double delta)
    {
        var hudTarget = XrManager.Instance.XrPlayer.HudTarget;

        GlobalPosition = hudTarget.GlobalPosition;
        GlobalRotation = hudTarget.GlobalRotation;

        _xrUI.GlobalPosition = GlobalPosition;
        _xrUI.GlobalRotation = GlobalRotation;
    }

    #endregion
}
