using Godot;
using Chickensoft.UMLGenerator;

[ClassDiagram(UseVSCodePaths = true)]
public partial class GameInstance : Node3D
{
	[Export(PropertyHint.NodeType, "Label")]
	private ScoreBoard scoreBoard;
	[Export(PropertyHint.NodeType, "Control")]
	private AbilitySelector abilitySelector;
	[Export(PropertyHint.NodeType, "Control")]
	private Control gameOverScreen;
	[Export(PropertyHint.NodeType, "RigidBody3D")]
	private PlayerCharacter playerCharacter;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Globals.CaptureMouse();
		SignalBus.Instance.EnemyKilled += EnemyKilled;
		playerCharacter.Died += GameOver;
		abilitySelector.showSelector();
	}

	public void EnemyKilled(EnemyBase enemy, PlayerCharacter killedBy)
	{
		scoreBoard.Score += enemy.Level * 10;
		enemy.QueueFree();
	}

	public void GameOver()
	{
		gameOverScreen.Visible = true;
		GetTree().Paused = true;
		Globals.ReleaseMouse();
	}

	public void QuitToMenu()
	{
		// save score
		Globals.saveData.AddScore(scoreBoard.Score);
		Globals.saveData.save();
		// make sure game is unpaused
		GetTree().Paused = false;
		// change scene to main menu
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}

    public override void _ExitTree()
    {
		// make sure to disconnect the signal when exiting the scene tree
		SignalBus.Instance.EnemyKilled -= EnemyKilled;
        base._ExitTree();
    }
}
