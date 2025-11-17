using Godot;

namespace SharpIDE.Godot.Features.Settings;

public partial class SettingsWindow : Window
{
    public override void _Ready()
    {
        CloseRequested += Hide;        
    }
}