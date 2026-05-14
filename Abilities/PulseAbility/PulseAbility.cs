using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PulseAbility : AbilityBase
{
    [Export]
	public float baseRange { get; private set; } = 10;
	public float Range {
		get { return _range; }
		set {
            _range = value;
            Scale = new Vector3(_range, 1, _range);
        }
	}
	private float _range;

	[Export]
	public float baseDamage { get; private set; } = 10;
	public float Damage {
		get { return _damage; }
		set { _damage = value; }
	}
	private float _damage;

    private MeshInstance3D vfxMesh;
    private ShapeCast3D hitBox;
    private AnimationPlayer animPlayer;
    private PlayerCharacter source;
    private bool firing;

    public override void _Ready()
    {
        vfxMesh = GetNode<MeshInstance3D>("%VFX");
        vfxMesh.Scale = new Vector3(1, 1, 1);
        hitBox = GetNode<ShapeCast3D>("%HitBox");
        hitBox.Scale = new Vector3(1, 1, 1);
        animPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");

        base._Ready();
    }

    private List<EnemyBase> enemiesHit = new List<EnemyBase>();

    public override void _Process(double delta)
    {
        base._Process(delta);

        if(!firing) return; // only processes hitbox when firing
        for (int i = 0; i < hitBox.GetCollisionCount(); i++)
        {
            Node collider = (Node)hitBox.GetCollider(i);
            if (IsInstanceValid(collider) && collider.IsInGroup("Enemies"))
            {
                EnemyBase enemy = (EnemyBase)collider;
                if (!enemiesHit.Contains(enemy))
                {
                    enemy.Hit(Damage, source);
                    enemiesHit.Add(enemy);
                }
            }
        }
    }

    public override void Fire(PlayerCharacter source)
    {
        this.source = source;
        firing = true;
        animPlayer.Play("PulseAnimation");
        lastFired = Time.GetTicksMsec();
    }

    public void AttackFinished(StringName AnimName)
    {
        firing = false;
        // not all enemies might be valid since some of them could have died!!
        OnAbilityFired(enemiesHit, source);
    }

    protected override void ResetStats()
    {
        CooldownMS = baseCooldownMS;
		Range = baseRange;
		Damage = baseDamage;
    }
}
