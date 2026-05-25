using Godot;
using System.Collections.Generic;
using Chickensoft.UMLGenerator;

[ClassDiagram(UseVSCodePaths = true)]
public partial class MainMenu : Control
{
	VBoxContainer scoresContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scoresContainer = GetNode<VBoxContainer>("%ScoresContainer");
		List<int> scores = Globals.saveData.GetScores();
		foreach (int score in scores)
		{
			Label newLabel = new Label()
			{
				Theme = GD.Load<Theme>("res://GUI/GUITheme.tres"),
				ThemeTypeVariation = "HeaderSmall",
				HorizontalAlignment = HorizontalAlignment.Center,
				Text = score.ToString()
			};
			scoresContainer.AddChild(newLabel);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartGame()
	{
		GetTree().ChangeSceneToFile("res://Scenes/GameplayScene/GameInstance.tscn");
	}

	public void QuitGame()
	{
		GetTree().Quit();
	}

	public void ClearScores()
	{
		Globals.saveData.Clear();
		Globals.saveData.save();
		foreach (Node n in scoresContainer.GetChildren())
		{
			n.QueueFree();
		}
	}
}
