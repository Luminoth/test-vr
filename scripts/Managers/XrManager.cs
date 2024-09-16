using VrTest.Util;

namespace VrTest.Managers;

public partial class XrManager : SingletonNode<XrManager>
{
    private XRInterface _xrInterface;

    public bool IsXrInitialized => _xrInterface != null && _xrInterface.IsInitialized();

    #region Godot Lifecycle

    public override void _Ready()
    {
        GD.Print($"XR Platform: {OS.GetName()}");

        _xrInterface = XRServer.FindInterface("OpenXR");
        if(IsXrInitialized) {
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
