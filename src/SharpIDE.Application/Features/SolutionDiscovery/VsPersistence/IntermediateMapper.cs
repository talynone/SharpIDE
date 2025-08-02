using Ardalis.GuardClauses;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;

namespace SharpIDE.Application.Features.SolutionDiscovery.VsPersistence;

public static class IntermediateMapper
{
	public static async Task<IntermediateSolutionModel> GetIntermediateModel(string solutionFilePath,
		CancellationToken cancellationToken = default)
	{
		var serializer = SolutionSerializers.GetSerializerByMoniker(solutionFilePath);
		Guard.Against.Null(serializer, nameof(serializer));
		var vsSolution = await serializer.OpenAsync(solutionFilePath, cancellationToken);

		var rootFolders = vsSolution.SolutionFolders
			.Where(f => f.Parent is null)
			.Select(f => BuildFolderTree(f, solutionFilePath, vsSolution.SolutionFolders, vsSolution.SolutionProjects))
			.ToList();

		var solutionModel = new IntermediateSolutionModel
		{
			Name = Path.GetFileName(solutionFilePath),
			FilePath = solutionFilePath,
			Projects = vsSolution.SolutionProjects.Where(p => p.Parent is null).Select(s => new IntermediateProjectModel
			{
				Model = s,
				Id = s.Id,
				FullFilePath = new DirectoryInfo(Path.Join(Path.GetDirectoryName(solutionFilePath), s.FilePath)).FullName
			}).ToList(),
			SolutionFolders = rootFolders
		};
		return solutionModel;
	}

	private static IntermediateSlnFolderModel BuildFolderTree(SolutionFolderModel folder, string solutionFilePath,
		IReadOnlyList<SolutionFolderModel> allSolutionFolders, IReadOnlyList<SolutionProjectModel> allSolutionProjects)
	{
		var childFolders = allSolutionFolders
			.Where(f => f.Parent == folder)
			.Select(f => BuildFolderTree(f, solutionFilePath, allSolutionFolders, allSolutionProjects))
			.ToList();

		var projectsInFolder = allSolutionProjects
			.Where(p => p.Parent == folder)
			.Select(s => new IntermediateProjectModel
			{
				Model = s,
				Id = s.Id,
				FullFilePath = new DirectoryInfo(Path.Join(Path.GetDirectoryName(solutionFilePath), s.FilePath)).FullName
			})
			.ToList();

		var filesInFolder = folder.Files?
			.Select(f => new IntermediateSlnFolderFileModel
			{
				Name = Path.GetFileName(f),
				FullPath = new FileInfo(Path.Join(Path.GetDirectoryName(solutionFilePath), f)).FullName
			})
			.ToList() ?? [];

		return new IntermediateSlnFolderModel
		{
			Model = folder,
			Folders = childFolders,
			Projects = projectsInFolder,
			Files = filesInFolder
		};
	}
}
