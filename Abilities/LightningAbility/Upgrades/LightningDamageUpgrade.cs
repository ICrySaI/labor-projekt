using Godot;
using System;
using System.Collections.Generic;

public partial class LightningDamageUpgrade : UpgradeBase
{
    [Export]
    private float damageIncrease;

    public override void ApplyAdditiveUpgrades(AbilityBase a)
    {
        if(a is LightningAbility)
        {
            ((LightningAbility)a).Damage += damageIncrease;
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
