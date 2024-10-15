using VrTest.Player;
using VrTest.Util;

namespace VrTest.Managers;

public partial class XrManager : SingletonNode<XrManager>
{
    [Export]
    private int _maximumRefreshRate = 120;

    [CanBeNull]
    private OpenXRInterface _xrInterface;

    public bool IsXrInitialized => _xrInterface != null && _xrInterface.IsInitialized();

    public bool XrIsFocused { get; private set; }

    [CanBeNull]
    public XrPlayer XrPlayer { get; set; }

    #region Godot Lifecycle

    public override void _Ready()
    {
        // PC not supported by OpenXR
        if(OS.GetName() != "Android") {
            GD.PushWarning($"Skipping OpenXR init on unsupported platform {OS.GetName()}");
            return;
        }

        GD.Print("Initializing OpenXR ...");

        _xrInterface = XRServer.FindInterface("OpenXR") as OpenXRInterface;
        if(!IsXrInitialized) {
            GD.PushError("OpenXR not initialized, please check if your headset is connected");
            GetTree().Quit();
            return;
        }

        GD.Print("OpenXR initialized successfully");

        var viewport = GetViewport();

        // Enable XR on our viewport
        viewport.UseXR = true;

        // Make sure v-sync is off, v-sync is handled by OpenXR
        DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);

        // Enable VRS
        if(RenderingServer.GetRenderingDevice() != null) {
            viewport.VrsMode = Viewport.VrsModeEnum.XR;
        } else if(ProjectSettings.GetSetting("xr/openxr/foveation_level").AsInt32() == 0) {
            GD.PushWarning("OpenXR: Recommend setting Foveation level to High in Project Settings");
        }

        // Connect the OpenXR events
        _xrInterface.SessionBegun += OnOpenXRSessionBegun;
        _xrInterface.SessionVisible += OnOpenXRVisibleState;
        _xrInterface.SessionFocussed += OnOpenXRFocusedState;
        _xrInterface.SessionStopping += OnOpenXRStopping;
        _xrInterface.PoseRecentered += OnOpenXRPoseRecentered;
    }

    #endregion

    #region Signal Handlers

    private void OnOpenXRSessionBegun()
    {
        // Get the reported refresh rate
        var currentRefreshRate = _xrInterface.DisplayRefreshRate;
        GD.Print(currentRefreshRate > 0
            ? $"OpenXR: Refresh rate reported as {currentRefreshRate}"
            : "OpenXR: No refresh rate given by XR runtime");

        // See if we have a better refresh rate available
        var newRate = currentRefreshRate;
        var availableRates = _xrInterface.GetAvailableDisplayRefreshRates();
        if(availableRates.Count == 0) {
            GD.Print("OpenXR: Target does not support refresh rate extension");
        } else if(availableRates.Count == 1) {
            // Only one available, so use it
            newRate = (float)availableRates[0];
        } else {
            GD.Print("OpenXR: Available refresh rates: ", availableRates);
            foreach(float rate in availableRates) {
                if(rate > newRate && rate <= _maximumRefreshRate) {
                    newRate = rate;
                }
            }
        }

        // Did we find a better rate?
        if(currentRefreshRate != newRate) {
            GD.Print($"OpenXR: Setting refresh rate to {newRate}");
            _xrInterface.DisplayRefreshRate = newRate;
            currentRefreshRate = newRate;
        }

        // Now match our physics rate
        if(currentRefreshRate > 0) {
            GD.Print($"OpenXR: Setting physics tick rate to {currentRefreshRate}");
            Engine.PhysicsTicksPerSecond = (int)currentRefreshRate;
        }
    }

    private void OnOpenXRVisibleState()
    {
        // We always pass this state at startup,
        // but the second time we get this it means our player took off their headset
        if(XrIsFocused) {
            GD.Print("OpenXR lost focus");

            XrIsFocused = false;

            // Pause our game
            GetTree().Paused = true;

            //EmitSignal(SignalName.FocusLost);
        }
    }

    private void OnOpenXRFocusedState()
    {
        GD.Print("OpenXR gained focus");
        XrIsFocused = true;

        // Un-pause our game
        GetTree().Paused = false;

        //EmitSignal(SignalName.FocusGained);
    }

    private void OnOpenXRStopping()
    {
        // Our session is being stopped.
        GD.Print("OpenXR is stopping");
    }

    private void OnOpenXRPoseRecentered()
    {
        // User recentered view, we have to react to this by recentering the view.
        // This is game implementation dependent.
        //EmitSignal(SignalName.PoseRecentered);
    }

    #endregion
}
