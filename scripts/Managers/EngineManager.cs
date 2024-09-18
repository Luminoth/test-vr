using VrTest.Util;

namespace VrTest.Managers;

public partial class EngineManager : SingletonNode<EngineManager>
{
    #region Godot Lifecycle

    public override void _Ready()
    {
        var joypads = Input.GetConnectedJoypads();
        GD.Print($"Detected {joypads.Count} joypads:");
        foreach(var joypad in joypads) {
            GD.Print(Input.GetJoyName(joypad));
        }
    }

    #endregion
}
