using Godot;
using System.Collections.Generic;
using Chickensoft.UMLGenerator;

[ClassDiagram(UseVSCodePaths = true)]
public partial class PulseRangeUpgrade : UpgradeBase
{
    [Export]
    private float rangeIncrease;

    public override void ApplyAdditiveUpgrades(AbilityBase a)
    {
        if (a is PulseAbility)
        {
            ((PulseAbility)a).Range += rangeIncrease;
        }
    }

    public override void ApplyMultiplicativeUpgrades(AbilityBase a)
    {
        return;
    }

    public override void OnAbilityFired(List<EnemyBase> enemiesHit, PlayerCharacter source)
    {
        return;
    }

}
