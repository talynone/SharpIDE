using System.Collections.Generic;
using GDExtensionBindgen;
using Godot;
using SharpIDE.Application.Features.SolutionDiscovery.VsPersistence;

namespace SharpIDE.Godot.Features.Run;

public partial class RunPanel : Control
{
	private Terminal _terminal = null!;
	private TabBar _tabBar = null!;
	private Panel _tabsPanel = null!;
	public override void _Ready()
	{
		_tabBar = GetNode<TabBar>("%TabBar");
		_tabsPanel = GetNode<Panel>("%TabsPanel");
		var test = GetNode<Control>("VBoxContainer/TabsPanel/Terminal");
		_terminal = new Terminal(test);
		_terminal.Write("Hello from SharpIDE.Godot!\n");
	}

	public override void _Process(double delta)
	{
		//_terminal.Write("a");
	}

	public void NewRunStarted(SharpIdeProjectModel projectModel)
	{
		var terminal = new Terminal();
		_tabBar.AddTab(projectModel.Name);
		_tabsPanel.AddChild(terminal);
	}
}