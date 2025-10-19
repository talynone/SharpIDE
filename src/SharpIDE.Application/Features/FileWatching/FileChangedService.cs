using Microsoft.VisualStudio.SolutionPersistence.Model;
using SharpIDE.Application.Features.Analysis;
using SharpIDE.Application.Features.Evaluation;
using SharpIDE.Application.Features.Events;
using SharpIDE.Application.Features.FilePersistence;
using SharpIDE.Application.Features.SolutionDiscovery;
using SharpIDE.Application.Features.SolutionDiscovery.VsPersistence;

namespace SharpIDE.Application.Features.FileWatching;

public enum FileChangeType
{
	IdeSaveToDisk, // Apply to disk
	IdeUnsavedChange, // Apply only in memory
	ExternalChange, // Apply to disk, as well as in memory
	CodeActionChange // Apply to disk, as well as in memory
}

public class FileChangedService(RoslynAnalysis roslynAnalysis, IdeOpenTabsFileManager openTabsFileManager)
{
	private readonly RoslynAnalysis _roslynAnalysis = roslynAnalysis;
	private readonly IdeOpenTabsFileManager _openTabsFileManager = openTabsFileManager;

	public SharpIdeSolutionModel SolutionModel { get; set; } = null!;

	// All file changes should go via this service
	public async Task SharpIdeFileChanged(SharpIdeFile file, string newContents, FileChangeType changeType)
	{
		if (changeType is FileChangeType.ExternalChange)
		{
			// Disk is already up to date
			// Update any open tabs
			// update in memory
			await _openTabsFileManager.UpdateFileTextInMemory(file, newContents);
			file.FileContentsChangedExternally.InvokeParallelFireAndForget();
		}
		else if (changeType is FileChangeType.CodeActionChange)
		{
			// update in memory, tabs and save to disk
			await _openTabsFileManager.UpdateInMemoryIfOpenAndSaveAsync(file, newContents);
			file.FileContentsChangedExternally.InvokeParallelFireAndForget();
		}
		else if (changeType is FileChangeType.IdeSaveToDisk)
		{
			// save to disk
			// We technically don't need to update in memory here. TODO review
			await _openTabsFileManager.UpdateInMemoryIfOpenAndSaveAsync(file, newContents);
		}
		else if (changeType is FileChangeType.IdeUnsavedChange)
		{
			// update in memory only
			await _openTabsFileManager.UpdateFileTextInMemory(file, newContents);
		}
		var afterSaveTask = (file, changeType) switch
		{
			({ IsRoslynWorkspaceFile: true }, _) => HandleWorkspaceFileChanged(file, newContents),
			({ IsCsprojFile: true }, FileChangeType.IdeSaveToDisk or FileChangeType.ExternalChange) => HandleCsprojChanged(file),
			({ IsCsprojFile: true }, _) => Task.CompletedTask,
			_ => throw new InvalidOperationException("Unknown file change type.")
		};
		await afterSaveTask;
	}

	private async Task HandleCsprojChanged(SharpIdeFile file)
	{
		var project = SolutionModel.AllProjects.SingleOrDefault(p => p.FilePath == file.Path);
		if (project is null) return;
		await ProjectEvaluation.ReloadProject(file.Path);
		await _roslynAnalysis.ReloadProject(project);
		GlobalEvents.Instance.SolutionAltered.InvokeParallelFireAndForget();
		await _roslynAnalysis.UpdateSolutionDiagnostics();
	}

	private async Task HandleWorkspaceFileChanged(SharpIdeFile file, string newContents)
	{
		await _roslynAnalysis.UpdateDocument(file, newContents);
		GlobalEvents.Instance.SolutionAltered.InvokeParallelFireAndForget();
		await _roslynAnalysis.UpdateSolutionDiagnostics();
	}
}
