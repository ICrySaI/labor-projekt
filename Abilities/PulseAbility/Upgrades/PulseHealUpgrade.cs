using Godot;
using System;
using System.Collections.Generic;

public partial class PulseHealUpgrade : UpgradeBase
{
    [Export]
    public float healValue = 10;

    public override void ApplyAdditiveUpgrades(AbilityBase a)
    {
        return;
    }

    public override void ApplyMultiplicativeUpgrades(AbilityBase a)
    {
        return;
    }

    public override void OnAbilityFired(List<EnemyBase> enemiesHit, PlayerCharacter source)
    {
        // not all enemies might be valid instances since some of them could have died!! (still get healing from enemies that were killed by the attack)
        source.CurrentHealth += enemiesHit.Count * healValue;
    }

}
