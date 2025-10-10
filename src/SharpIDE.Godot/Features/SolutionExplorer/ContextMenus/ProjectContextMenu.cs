using Godot;
using SharpIDE.Application.Features.Build;
using SharpIDE.Application.Features.Events;
using SharpIDE.Application.Features.SolutionDiscovery.VsPersistence;
using SharpIDE.Godot.Features.BottomPanel;

namespace SharpIDE.Godot.Features.SolutionExplorer;

file enum ProjectContextMenuOptions
{
    Run = 0,
    Build = 1,
    Rebuild = 2,
    Clean = 3,
    Restore = 4
}

public partial class SolutionExplorerPanel
{
    private Texture2D _runIcon = ResourceLoader.Load<Texture2D>("uid://bkty6563cthj8");
    private void OpenContextMenuProject(SharpIdeProjectModel project)
    {
        var menu = new PopupMenu();
        AddChild(menu);
        menu.AddIconItem(_runIcon, "Run", (int)ProjectContextMenuOptions.Run);
        menu.SetItemIconMaxWidth((int)ProjectContextMenuOptions.Run, 20);
        menu.AddSeparator();
        menu.AddItem("Build", (int)ProjectContextMenuOptions.Build);
        menu.AddItem("Rebuild", (int)ProjectContextMenuOptions.Rebuild);
        menu.AddItem("Clean", (int)ProjectContextMenuOptions.Clean);
        menu.AddItem("Restore", (int)ProjectContextMenuOptions.Restore);
        menu.PopupHide += () => menu.QueueFree();
        menu.IdPressed += id =>
        {
            var actionId = (ProjectContextMenuOptions)id;
            if (actionId is ProjectContextMenuOptions.Run)
            {
                
            }
            if (actionId is ProjectContextMenuOptions.Build)
            {
                _ = Task.GodotRun(async () => await MsBuildProject(project, BuildType.Build));
            }
            else if (actionId is ProjectContextMenuOptions.Rebuild)
            {
                _ = Task.GodotRun(async () => await MsBuildProject(project, BuildType.Rebuild));
            }
            else if (actionId is ProjectContextMenuOptions.Clean)
            {
                _ = Task.GodotRun(async () => await MsBuildProject(project, BuildType.Clean));
            }
            else if (actionId is ProjectContextMenuOptions.Restore)
            {
                _ = Task.GodotRun(async () => await MsBuildProject(project, BuildType.Restore));
            }
        };
			
        var globalMousePosition = GetGlobalMousePosition();
        menu.Position = new Vector2I((int)globalMousePosition.X, (int)globalMousePosition.Y);
        menu.Popup();
    }
    private static async Task MsBuildProject(SharpIdeProjectModel project, BuildType buildType)
    {
        GodotGlobalEvents.Instance.BottomPanelTabExternallySelected.InvokeParallelFireAndForget(BottomPanelType.Build);
        await Singletons.BuildService.MsBuildAsync(project.FilePath, buildType);
    }
}
