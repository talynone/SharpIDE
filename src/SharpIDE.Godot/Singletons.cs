using SharpIDE.Application.Features.Run;

namespace SharpIDE.Godot;

public static class Singletons
{
    public static RunService RunService { get; } = new RunService();
}