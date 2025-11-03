using Godot;
using SharpIDE.Application.Features.SolutionDiscovery.VsPersistence;

namespace SharpIDE.Godot.Features.Nuget;

public partial class PackageDetailsProjectEntry : MarginContainer
{
    private Label _projectNameLabel = null!;
    private Label _installedVersionLabel = null!;
    
    public SharpIdeProjectModel ProjectModel { get; set; } = null!;
    public string InstalledVersion { get; set; } = string.Empty;
    public override void _Ready()
    {
        _projectNameLabel = GetNode<Label>("%ProjectNameLabel");;
        _installedVersionLabel = GetNode<Label>("%InstalledVersionLabel");
        SetValues();
    }

    private void SetValues()
    {
        if (ProjectModel == null) return;
        _projectNameLabel.Text = ProjectModel.Name;
        _installedVersionLabel.Text = InstalledVersion;
    }
}