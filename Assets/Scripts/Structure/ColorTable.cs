using UnityEngine;
using System;

namespace YaEm
{
	//public static class ColorTable
	//{
	//	public static Color GetColor(int teamNumber)
	//	{
	//		switch (teamNumber)
	//		{
	//			case 1: return Color.cyan;
	//			case 2: return Color.red;
	//			case 3: return new Color(1, 0.5f, 0); //orange
	//			case 4: return Color.green;
	//			case 5: return Color.blue;
	//			default: return Color.white;
	//		}
	//	}

	//	public static Gradient GetColoredGradient(Gradient gradient, int teamNumber)
	//	{
	//		Color col = ColorTable.GetColor(teamNumber);
	//		Color.RGBToHSV(col, out float offset, out _, out _);

	//		Gradient grad = new Gradient();
	//		GradientColorKey[] keys = new GradientColorKey[gradient.colorKeys.Length];
	//		for (int i = 0, length = gradient.colorKeys.Length; i < length; i++)
	//		{
	//			var current = gradient.colorKeys[i];
	//			Color.RGBToHSV(gradient.colorKeys[i].color, out float var, out float s, out float v);
	//			keys[i] = new GradientColorKey(Color.HSVToRGB(var + offset, s, v), current.time);
	//		}
	//		grad.SetKeys(keys, gradient.alphaKeys);
	//		return grad;
	//	}
	//}

	public sealed class ColorTable : IService
	{
		public static readonly Color[] DefaultColors = new Color[6]
		{
			Color.white, //1 is always reserved for white color
			Color.cyan,
			Color.red,
			new Color(1, 0.5f, 0),
			Color.green,
			Color.blue
		};

		private Color[] _colors;

		public ColorTable()
		{
			_colors = new Color[6];
			Array.Copy(DefaultColors, _colors, DefaultColors.Length);
		}

		public static Color GetDefaultColor(int teamNumber)
		{
			if (teamNumber >= 1 && teamNumber < DefaultColors.Length)
			{
				return DefaultColors[teamNumber];
			}
			return DefaultColors[0];
		}

		public Color GetColor(int teamNumber)
		{
			if(teamNumber == -41)
			{
				return new Color(0.41f, 0, 0.34f, 1f);
			}
			if(teamNumber >= 1 && teamNumber < _colors.Length)
			{
				return _colors[teamNumber];
			}
			return _colors[0];
		}

		public Gradient GetColoredGradient(Gradient gradient, int teamNumber)
		{
			Color col = GetColor(teamNumber);
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

		public void SetColor(int teamNumber, in Color color)
		{
			if(teamNumber >= 1 && teamNumber < _colors.Length)
			{
				_colors[teamNumber] = color;
				return;
			}
			throw new ArgumentException();
		}
	}
}
