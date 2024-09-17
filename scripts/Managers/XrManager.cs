using VrTest.Util;

namespace VrTest.Managers;

public partial class XrManager : SingletonNode<XrManager>
{
    private XRInterface _xrInterface;

    public bool IsXrInitialized => _xrInterface != null && _xrInterface.IsInitialized();

    #region Godot Lifecycle

    public override void _Ready()
    {
        // PC not supported by OpenXR
        if(OS.GetName() != "Android") {
            GD.PushWarning($"Skipping OpenXR init on unsupported platform {OS.GetName()}");
            return;
        }

        GD.Print("Initializing OpenXR ...");

        _xrInterface = XRServer.FindInterface("OpenXR");
        if(IsXrInitialized) {
            GD.Print("OpenXR initialized successfully");
            var vp = GetViewport();

            // Enable XR on our viewport
            vp.UseXR = true;

            // Make sure v-sync is off, v-sync is handled by OpenXR
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);

            // Enable VRS
            if(RenderingServer.GetRenderingDevice() != null) {
                vp.VrsMode = Viewport.VrsModeEnum.XR;
            } else if((int)ProjectSettings.GetSetting("xr/openxr/foveation_level") == 0) {
                GD.PushWarning("OpenXR: Recommend setting Foveation level to High in Project Settings");
            }

            // TODO: Also note that by default the physics engine runs at 60Hz as well and this can result in choppy physics.
            // You should set Engine.physics_ticks_per_second to a higher value.

            // Connect the OpenXR events
            /*_xrInterface.SessionBegun += OnOpenXRSessionBegun;
            _xrInterface.SessionVisible += OnOpenXRVisibleState;
            _xrInterface.SessionFocussed += OnOpenXRFocusedState;
            _xrInterface.SessionStopping += OnOpenXRStopping;
            _xrInterface.PoseRecentered += OnOpenXRPoseRecentered;*/
        } else {
            GD.PushError("OpenXR not initialized, please check if your headset is connected");
            GetTree().Quit();
        }
    }

    #endregion
}
