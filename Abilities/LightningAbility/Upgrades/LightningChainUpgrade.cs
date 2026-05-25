using Godot;
using System.Collections.Generic;
using Chickensoft.UMLGenerator;

[ClassDiagram(UseVSCodePaths = true)]
public partial class LightningChainUpgrade : UpgradeBase
{
    [Export]
    private int chainCountIncrease;
    public override void ApplyAdditiveUpgrades(AbilityBase a)
    {
        if(a is LightningAbility)
        {
            ((LightningAbility)a).ChainAmount += chainCountIncrease;
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
