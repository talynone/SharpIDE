using SharpIDE.Application.Features.Events;
using SharpIDE.Application.Features.SolutionDiscovery.VsPersistence;

namespace SharpIDE.Application.Features.FileWatching;

public class IdeFileExternalChangeHandler
{
	private readonly FileChangedService _fileChangedService;
	public SharpIdeSolutionModel SolutionModel { get; set; } = null!;
	public IdeFileExternalChangeHandler(FileChangedService fileChangedService)
	{
		_fileChangedService = fileChangedService;
		GlobalEvents.Instance.FileSystemWatcherInternal.FileChanged.Subscribe(OnFileChanged);
	}

	private async Task OnFileChanged(string filePath)
	{
		var sharpIdeFile = SolutionModel.AllFiles.SingleOrDefault(f => f.Path == filePath);
		if (sharpIdeFile is null) return;
		if (sharpIdeFile.SuppressDiskChangeEvents is true) return;
		if (sharpIdeFile.LastIdeWriteTime is not null)
		{
			var now = DateTimeOffset.Now;
			if (now - sharpIdeFile.LastIdeWriteTime.Value < TimeSpan.FromMilliseconds(300))
			{
				Console.WriteLine($"IdeFileExternalChangeHandler: Ignored - {filePath}");
				return;
			}
		}
		Console.WriteLine($"IdeFileExternalChangeHandler: Changed - {filePath}");
		var file = SolutionModel.AllFiles.SingleOrDefault(f => f.Path == filePath);
		if (file is not null)
		{
			await _fileChangedService.SharpIdeFileChanged(file, await File.ReadAllTextAsync(file.Path), FileChangeType.ExternalChange);
		}
	}
}
