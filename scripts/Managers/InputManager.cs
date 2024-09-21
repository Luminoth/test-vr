using VrTest.Util;

namespace VrTest.Managers;

public partial class InputManager : SingletonNode<InputManager>
{
    #region Godot Lifecycle

    public override void _Ready()
    {
        if(!XrManager.Instance.IsXrInitialized) {
            var joypads = Input.GetConnectedJoypads();
            GD.Print($"Detected {joypads.Count} joypads:");
            foreach(var joypad in joypads) {
                GD.Print($"  {Input.GetJoyName(joypad)}");
            }
        }
    }

    #endregion
}
