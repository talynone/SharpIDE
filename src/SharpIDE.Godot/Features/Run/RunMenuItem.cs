using Godot;
using SharpIDE.Application.Features.SolutionDiscovery.VsPersistence;

namespace SharpIDE.Godot.Features.Run;

public partial class RunMenuItem : HBoxContainer
{
    public SharpIdeProjectModel Project { get; set; } = null!;
    private Label _label = null!;
    private Button _runButton = null!;
    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _runButton = GetNode<Button>("RunButton");
        _runButton.Pressed += OnRunButtonPressed;
        _label.Text = Project.Name;
    }

    private async void OnRunButtonPressed()
    {
        await Singletons.RunService.RunProject(Project).ConfigureAwait(false);
    }
}