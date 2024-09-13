using VrTest.Util;

namespace VrTest.Managers;

public partial class XrManager : SingletonNode<XrManager>
{
    private XRInterface _xrInterface;

    #region Godot Lifecycle

    public override void _Ready()
    {
        _xrInterface = XRServer.FindInterface("OpenXR");
        if(_xrInterface != null && _xrInterface.IsInitialized()) {
            GD.Print("OpenXR initialized successfully");

            // Turn off v-sync!
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);

            // Change our main viewport to output to the HMD
            GetViewport().UseXR = true;

            // TODO: Also note that by default the physics engine runs at 60Hz as well and this can result in choppy physics.
            // You should set Engine.physics_ticks_per_second to a higher value.
        } else {
            GD.PushError("OpenXR not initialized, please check if your headset is connected");
        }
    }

    #endregion
}
