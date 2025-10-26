using System.Collections.Immutable;
using Godot;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using SharpIDE.Application.Features.SolutionDiscovery;

namespace SharpIDE.Godot.Features.SymbolLookup;

public partial class SymbolLookupPopup : PopupPanel
{
    private Label _symbolNameLabel = null!;
    private VBoxContainer _usagesContainer = null!;
    public ImmutableArray<ReferenceLocation> Locations { get; set; }
    public ImmutableArray<(ReferenceLocation location, SharpIdeFile file)> LocationsAndFiles { get; set; }
    public ISymbol Symbol { get; set; } = null!;
	private readonly PackedScene _symbolUsageScene = ResourceLoader.Load<PackedScene>("uid://dokm0dyac2enh");
    
    public override void _Ready()
    {
        _symbolNameLabel = GetNode<Label>("%SymbolNameLabel");
        _symbolNameLabel.Text = "";
        _usagesContainer = GetNode<VBoxContainer>("%UsagesVBoxContainer");
        _usagesContainer.GetChildren().ToList().ForEach(s => s.QueueFree());
        AboutToPopup += OnAboutToPopup;
        
        _usagesContainer.GetChildren().ToList().ForEach(s => s.QueueFree());
        foreach (var (location, file) in LocationsAndFiles)
        {
            var resultNode = _symbolUsageScene.Instantiate<SymbolUsageComponent>();
            resultNode.Location = location;
            resultNode.File = file;
            resultNode.ParentSearchWindow = this;
            _usagesContainer.AddChild(resultNode);
        }
        _symbolNameLabel.Text = $"'{Symbol.Name}'";
    }

    private void OnAboutToPopup()
    {
        
    }
}