using SharpIDE.Application.Features.Events;

namespace SharpIDE.Godot;

public static class GodotGlobalEvents
{
    public static event Func<BottomPanelType, Task> BottomPanelTabExternallySelected = _ => Task.CompletedTask;
    public static void InvokeBottomPanelTabExternallySelected(BottomPanelType type) => BottomPanelTabExternallySelected.InvokeParallelFireAndForget(type);
    
    public static event Func<BottomPanelType?, Task> BottomPanelTabSelected = _ => Task.CompletedTask;
    public static void InvokeBottomPanelTabSelected(BottomPanelType? type) => BottomPanelTabSelected.InvokeParallelFireAndForget(type);
    
    public static event Func<bool, Task> BottomPanelVisibilityChangeRequested = _ => Task.CompletedTask;
    public static void InvokeBottomPanelVisibilityChangeRequested(bool show) => BottomPanelVisibilityChangeRequested.InvokeParallelFireAndForget(show);
}

public enum BottomPanelType
{
    Run,
    Build,
    Problems
}