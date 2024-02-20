using YaEm.Weapons;

public static class SpreadProviderFactory
{
	public static ISpreadProvider GetSpread(RangedWeapon.SpreadType spreadType, int iterations, float angle, int offset = 0)
	{
		if (spreadType == RangedWeapon.SpreadType.Fixed) return new FixedSpread();
		return new AngleSpread(iterations, angle, offset);
	}
}
