using GDExtensionBindgen;
using Godot;
using SharpIDE.Application.Features.SolutionDiscovery.VsPersistence;

namespace SharpIDE.Godot.Features.Debug_.Tab;

public partial class DebugPanelTab : Control
{
    private Terminal _terminal = null!;
    private Task _writeTask = Task.CompletedTask;
    
    public SharpIdeProjectModel Project { get; set; } = null!;
    public int TabBarTab { get; set; }

    public override void _Ready()
    {
        var terminalControl = GetNode<Control>("%Terminal");
        _terminal = new Terminal(terminalControl);
    }
    
    public void StartWritingFromProjectOutput()
    {
        if (_writeTask.IsCompleted is not true)
        {
            GD.PrintErr("Attempted to start writing from project output, but a write task is already running.");
            return;
        }
        _writeTask = GodotTask.Run(async () =>
        {
            await foreach (var array in Project.RunningOutputChannel!.Reader.ReadAllAsync().ConfigureAwait(false))
            {
                //_terminal.Write(array);
                //await this.InvokeAsync(() => _terminal.Write(array));
                var str = System.Text.Encoding.UTF8.GetString(array);
                await this.InvokeAsync(() => _terminal.Write(str));
            }
        });
    }
    
    public void ClearTerminal()
    {
        _terminal.Clear();
    }
}