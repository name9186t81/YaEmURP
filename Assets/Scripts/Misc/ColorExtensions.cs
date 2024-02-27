using UnityEngine;

public static class ColorExtensions
{
	public static Color WhiteOut(this in Color color, float amount)
	{
		Color.RGBToHSV(color, out var h, out var s, out var v);
		return Color.HSVToRGB(h, 1 - Mathf.Clamp(amount * amount * amount, 0f, 0.99f), v);
	}

	public static Color Saturate(this in Color color, float amount)
	{
		Color.RGBToHSV(color, out var h, out var s, out var v);
		return Color.HSVToRGB(h, 1, v);
	}
}