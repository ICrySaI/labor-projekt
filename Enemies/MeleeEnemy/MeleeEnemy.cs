using Godot;
using Chickensoft.UMLGenerator;

[ClassDiagram(UseVSCodePaths = true)]
public partial class MeleeEnemy : EnemyBase
{
	[Export(PropertyHint.Range, "1, 20")]
	private float attackRange = 5;

    protected override bool TryAttack()
	{
		// check if we have a target
		if(currentTarget == null) return false;
		// check if we're in range to attack
		if(GlobalPosition.DistanceTo(currentTarget.GlobalPosition) > attackRange) return false;

		// execute attack
		AddChild(new MeleeEnemyAttack(this, attackRange, AttackDamage));
		return true;
	}
}
