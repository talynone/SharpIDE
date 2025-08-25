using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Microsoft.Build.Locator;
using SharpIDE.Application.Features.Analysis;
using SharpIDE.Application.Features.SolutionDiscovery.VsPersistence;
using SharpIDE.Godot.Features.Run;

namespace SharpIDE.Godot;

public partial class IdeRoot : Control
{
	private Button _openSlnButton = null!;
	private FileDialog _fileDialog = null!;
	private SharpIdeCodeEdit _sharpIdeCodeEdit = null!;
	private SolutionExplorerPanel _solutionExplorerPanel = null!;
	private RunPanel _runPanel = null!;
	private MenuButton _runMenuButton = null!;
	public override void _Ready()
	{
		MSBuildLocator.RegisterDefaults();
		
		_openSlnButton = GetNode<Button>("%OpenSlnButton");
		_runMenuButton = GetNode<MenuButton>("%RunMenuButton");
		var popup = _runMenuButton.GetPopup();
		popup.HideOnItemSelection = false;
		
		_sharpIdeCodeEdit = GetNode<SharpIdeCodeEdit>("%SharpIdeCodeEdit");
		_fileDialog = GetNode<FileDialog>("%OpenSolutionDialog");
		_solutionExplorerPanel = GetNode<SolutionExplorerPanel>("%SolutionExplorerPanel");
		_fileDialog.FileSelected += OnFileSelected;
		_runPanel = GetNode<RunPanel>("%RunPanel");
		_openSlnButton.Pressed += () => _fileDialog.Visible = true;
		//_fileDialog.Visible = true;
		OnFileSelected(@"C:\Users\Matthew\Documents\Git\BlazorCodeBreaker\BlazorCodeBreaker.slnx");
	}

	private void OnFileSelected(string path)
	{
		_ = Task.Run(async () =>
		{
			try
			{
				GD.Print($"Selected: {path}");
				var solutionModel = await VsPersistenceMapper.GetSolutionModel(path);
				_solutionExplorerPanel.SolutionModel = solutionModel;
				Callable.From(_solutionExplorerPanel.RepopulateTree).CallDeferred();
				RoslynAnalysis.StartSolutionAnalysis(path);
				var infraProject = solutionModel.AllProjects.Single(s => s.Name == "Infrastructure");
				var diFile = infraProject.Files.Single(s => s.Name == "DependencyInjection.cs");
				await this.InvokeAsync(async () => await _sharpIdeCodeEdit.SetSharpIdeFile(diFile));
				
				var tasks = solutionModel.AllProjects.Select(p => p.MsBuildEvaluationProjectTask).ToList();
				await Task.WhenAll(tasks).ConfigureAwait(false);
				var runnableProjects = solutionModel.AllProjects.Where(p => p.IsRunnable).ToList();
				await this.InvokeAsync(() =>
				{
					var popup = _runMenuButton.GetPopup();
					foreach (var project in runnableProjects)
					{
						popup.AddItem(project.Name);
					}
					_runMenuButton.Disabled = false;
				});
				//var runnableProject = solutionModel.AllProjects.First(s => s.IsRunnable);
				//await this.InvokeAsync(() => _runPanel.NewRunStarted(runnableProject));
			}
			catch (Exception e)
			{
				GD.PrintErr($"Error loading solution: {e.Message}");
				GD.PrintErr(e.StackTrace);
			}
		});
	}
}