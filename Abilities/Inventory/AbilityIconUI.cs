using Godot;
using System;

public partial class AbilityIconUI : ProgressBar
{
	private AbilityBase ability;

	public AbilityIconUI(AbilityBase ability)
	{
		this.ability = ability;
		MaxValue = ability.CooldownMS;
		MinValue = 0;

		// set the visuals of the ability icon
		CustomMinimumSize = new Vector2(30, 30);
		FillMode = (int)FillModeEnum.BottomToTop;
		ShowPercentage = false;
		StyleBoxFlat fillStyle = new StyleBoxFlat()
		{
			BgColor = new Color(0.5f, 0.5f, 0.5f, 0.5f),
		};
		AddThemeStyleboxOverride("fill", fillStyle);

		// if the ability has an icon texture assigned, set it as the progress bar background, else set a base color
		if(ability.IconTexture != null)
		{
            StyleBoxTexture backgroundStyle = new StyleBoxTexture
            {
                Texture = ability.IconTexture
            };
            AddThemeStyleboxOverride("background", backgroundStyle);
		}
		else
		{
			StyleBoxFlat backgroundStyle = new StyleBoxFlat()
			{
				BgColor = new Color(0.1f, 0.1f, 0.1f, 0.75f),
			};
			AddThemeStyleboxOverride("background", backgroundStyle);
		}
	}

	private void CooldownChanged()
	{
		MaxValue = ability.CooldownMS;
	}
}
