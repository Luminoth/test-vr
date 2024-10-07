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

}
