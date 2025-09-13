using Godot;
using Microsoft.CodeAnalysis;
using SharpIDE.Application.Features.SolutionDiscovery.VsPersistence;

namespace SharpIDE.Godot.Features.Problems;

public partial class DiagnosticMetadataContainer(Diagnostic diagnostic) : GodotObject
{
    public Diagnostic Diagnostic { get; } = diagnostic;
}

public partial class ProjectContainer(SharpIdeProjectModel project) : GodotObject
{
    public SharpIdeProjectModel Project { get; } = project;
}