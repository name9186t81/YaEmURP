using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEditor.PackageManager;

using UnityEngine;

namespace YaEm
{
	public static class ColorTable
	{
		public static Color GetColor(int teamNumber)
		{
			switch (teamNumber)
			{
				case 1: return Color.green;
				case 2: return Color.red;
				case 3: return Color.blue;
				case 4: return Color.yellow;
				case 5: return Color.cyan;
				default: return Color.white;
			}
		}

		public static Gradient GetColoredGradient(Gradient gradient, int teamNumber)
		{
			Color col = ColorTable.GetColor(teamNumber);
			Color.RGBToHSV(col, out float offset, out _, out _);

			Gradient grad = new Gradient();
			GradientColorKey[] keys = new GradientColorKey[gradient.colorKeys.Length];
			for (int i = 0, length = gradient.colorKeys.Length; i < length; i++)
			{
				var current = gradient.colorKeys[i];
				Color.RGBToHSV(gradient.colorKeys[i].color, out float var, out float s, out float v);
				keys[i] = new GradientColorKey(Color.HSVToRGB(var + offset, s, v), current.time);
			}
			grad.SetKeys(keys, gradient.alphaKeys);
			return grad;
		} 
	}
}
