using Godot;
using System;

public partial class AbilityIcon : ProgressBar
{
	private AbilityBase ability;

	public AbilityIcon(AbilityBase ability)
	{
		this.ability = ability;
		MaxValue = ability.CooldownMS;
		ability.CooldownChanged += CooldownChanged;
		MinValue = 0;

		CustomMinimumSize = new Vector2(30, 30);
		FillMode = (int)FillModeEnum.BottomToTop;
		ShowPercentage = false;
	}

	private void CooldownChanged()
	{
		MaxValue = ability.CooldownMS;
	}
}
