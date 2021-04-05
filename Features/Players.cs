﻿using EFT.Trainer.Configuration;
using EFT.Trainer.Extensions;
using UnityEngine;

namespace EFT.Trainer.Features
{
	public class Players : ToggleMonoBehaviour
	{
		[ConfigurationProperty]
		public Color BearColor { get; set; } = Color.blue;

		[ConfigurationProperty]
		public Color UsecColor { get; set; } = Color.green;

		[ConfigurationProperty]
		public Color ScavColor { get; set; } = Color.yellow;
		
		[ConfigurationProperty]
		public Color BossColor { get; set; } = Color.red;

		protected override void UpdateWhenEnabled()
		{
			var hostiles = GameState.Current?.Hostiles;
			if (hostiles == null)
				return;

			foreach (var ennemy in hostiles)
			{
				if (!ennemy.IsValid())
					continue;

				var color = GetPlayerColor(ennemy);
				SetShaders(ennemy, GameState.OutlineShader, color);
			}
		}

		private Color GetPlayerColor(Player player)
		{
			var info = player.Profile?.Info;
			if (info == null)
				return ScavColor;

			var settings = info.Settings;
			if (settings != null && settings.IsBoss())
				return BossColor;

			// it can still be a bot in sptarkov but let's use the pmc color
			return info.Side switch
			{
				EPlayerSide.Bear => BearColor,
				EPlayerSide.Usec => UsecColor,
				_ => ScavColor
			};
		}

		private static void SetShaders(Player player, Shader shader, Color color)
		{
			var skins = player.PlayerBody.BodySkins;
			foreach (var skin in skins.Values)
			{
				if (skin == null)
					continue;

				foreach (var renderer in skin.GetRenderers())
				{
					if (renderer == null)
						continue;

					var material = renderer.material;
					if (material == null)
						continue;

					if (material.shader != null && material.shader == shader)
						continue;

					material.shader = shader;

					material.SetColor("_FirstOutlineColor", Color.red);
					material.SetFloat("_FirstOutlineWidth", 0.02f);
					material.SetColor("_SecondOutlineColor", color);
					material.SetFloat("_SecondOutlineWidth", 0.0025f);
				}
			}
		}
	}
}
