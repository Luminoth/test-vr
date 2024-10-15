using VrTest.Managers;

namespace VrTest.Player;

public partial class XrPlayer : XROrigin3D
{
    [Export]
    private XRCamera3D _camera;

    public XRCamera3D Camera => _camera;

    [Export]
    private XRController3D _leftHand;

    public XRController3D LeftHand => _leftHand;

    [Export]
    private XRController3D _rightHand;

    public XRController3D RightHand => _rightHand;

    [Export]
    private OpenXRCompositionLayer _hud;

    public OpenXRCompositionLayer Hud => _hud;

    #region Godot Lifecycle

    public override void _Ready()
    {
        XrManager.Instance.XrPlayer = this;
    }

    #endregion

    // moves the origin to fix the camera at the player height
    // minus a little bit to be at the eye position
    // (assuming Local reference space here, Local Floor and Stage shouldn't do this)
    public void ResetHeight(float eyeHeight)
    {
        GlobalPosition = GlobalPosition with { Y = GlobalPosition.Y + eyeHeight - Camera.Position.Y };
    }

    // from XRTools, rotates the origin around the camera
    public void Rotate(float angle)
    {
        var t1 = Transform3D.Identity;
        var t2 = Transform3D.Identity;
        var rot = Transform3D.Identity;

        t1.Origin = -Camera.Position;
        t2.Origin = Camera.Position;
        rot = rot.Rotated(Vector3.Down, angle);

        Transform = (Transform * t2 * rot * t1).Orthonormalized();
    }
}
