using Godot;
using Chickensoft.UMLGenerator;

[ClassDiagram(UseVSCodePaths = true)]
public partial class SignalBus : Node
{
    public static SignalBus Instance { get; private set; }

    [Signal]
    public delegate void EnemyKilledEventHandler(EnemyBase enemy, PlayerCharacter killedBy);

    public override void _Ready()
    {
        Instance = this;
        base._Ready();
    }
}
