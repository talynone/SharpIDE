using System.Diagnostics;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace SharpIDE.Application.Features.Build;

public enum BuildType
{
	Build,
	Rebuild,
	Clean,
	Restore
}
public class BuildService
{
	public async Task MsBuildSolutionAsync(string solutionFilePath, BuildType buildType = BuildType.Build)
	{
		var buildParameters = new BuildParameters
		{
			Loggers =
			[
				//new BinaryLogger { Parameters = "msbuild.binlog" },
				new ConsoleLogger(LoggerVerbosity.Quiet),
			],
		};
		string[] targetsToBuild = buildType switch
		{
			BuildType.Build => ["Restore", "Build"],
			BuildType.Rebuild => ["Restore", "Rebuild"],
			BuildType.Clean => ["Clean"],
			BuildType.Restore => ["Restore"],
			_ => throw new ArgumentOutOfRangeException(nameof(buildType), buildType, null)
		};
		var buildRequest = new BuildRequestData(
			projectFullPath : solutionFilePath,
			globalProperties: new Dictionary<string, string?>(),
			toolsVersion: null,
			targetsToBuild: targetsToBuild,
			hostServices: null,
			flags: BuildRequestDataFlags.None);

		await Task.Run(async () =>
		{
			var buildCompleteTcs = new TaskCompletionSource<BuildResult>();
			BuildManager.DefaultBuildManager.BeginBuild(buildParameters);
			var buildResult2 = BuildManager.DefaultBuildManager.PendBuildRequest(buildRequest);
			var timer = Stopwatch.StartNew();
			buildResult2.ExecuteAsync((BuildSubmission test) =>
			{
				buildCompleteTcs.SetResult(test.BuildResult!);
			}, null);
			//var buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequest); // This is a convenience to essentially do the same thing.
			var buildResult = await buildCompleteTcs.Task.ConfigureAwait(false);
			timer.Stop();
			BuildManager.DefaultBuildManager.EndBuild();
			Console.WriteLine($"Build result: {buildResult.OverallResult} in {timer.ElapsedMilliseconds}ms");
		}).ConfigureAwait(false);
	}
}

// To build a single project
// var solutionFile = GetNodesInSolution.ParseSolutionFileFromPath(_solutionFilePath);
// ArgumentNullException.ThrowIfNull(solutionFile);
// var projects = GetNodesInSolution.GetCSharpProjectObjectsFromSolutionFile(solutionFile);
// var projectRoot = projects.First();
// var buildRequest = new BuildRequestData(
// 	ProjectInstance.FromProjectRootElement(projectRoot, new ProjectOptions()),
// 	targetsToBuild: ["Restore", "Build"]);
