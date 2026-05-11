using Godot;
using System;

public partial class LevelUpManager : Control
{
    [Export]
    public int KillsToLevelUp { get; private set; } = 10;
    [Export(PropertyHint.NodeType, "Control")]
    private AbilitySelector abilitySelector;

    private int currentKills = 0;

    private ProgressBar progress;

    public override void _Ready()
    {
        progress = GetNode<ProgressBar>("ProgressBar");
        progress.MaxValue = KillsToLevelUp;
        SignalBus.Instance.EnemyKilled += EnemyKilled;
        base._Ready();
    }

    public void EnemyKilled(EnemyBase enemy, PlayerCharacter source)
    {
        currentKills++;
        progress.Value = currentKills;
        if (currentKills >= KillsToLevelUp)
        {
            currentKills = 0;
            abilitySelector.showSelector();
        }
    }
}
