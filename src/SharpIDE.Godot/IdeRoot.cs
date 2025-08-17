using Godot;

namespace SharpIDE.Godot;

public partial class IdeRoot : Control
{
	private Button _openSlnButton = null!;
	private FileDialog _fileDialog = null!;
	public override void _Ready()
	{
		_openSlnButton = GetNode<Button>("%OpenSlnButton");
		_fileDialog = GetNode<FileDialog>("%OpenSolutionDialog");
		_fileDialog.FileSelected += OnFileSelected;
	}

	private void OnFileSelected(string path)
	{
		GD.Print($"Selected: {path}");
	}
}