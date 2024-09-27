using VrTest.Player;
using VrTest.Player.Input;

namespace VrTest.UI;

public partial class HUD : Control
{
    [Export]
    private XrPlayerCharacter _character;

    [Export]
    private XrInput _xrInput;

    [Export]
    private Label _fpsLabel;

    [Export]
    private Label _isOnFloorLabel;

    [Export]
    private Label _velocityLabel;

    [Export]
    private Label _leftHandVelocityLabel;

    [Export]
    private Label _rightHandVelocityLabel;

    #region Godot Lifecycle

    public override void _Process(double delta)
    {
        _fpsLabel.Text = $"FPS: {Engine.GetFramesPerSecond()}";

        _isOnFloorLabel.Text = $"IsOnFloor: {_character.IsOnFloor()}";
        _velocityLabel.Text = $"Velocity: {_character.Velocity}";
        _leftHandVelocityLabel.Text = $"Left Hand Velocity: {_xrInput.LeftHand.Velocity}";
        _rightHandVelocityLabel.Text = $"Right Hand Velocity: {_xrInput.RightHand.Velocity}";
    }

    #endregion
}
