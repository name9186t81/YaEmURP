using YaEm.Core;
using System;

namespace YaEm.Weapons
{
	public interface IParriable
	{
		ParriableFlags Flags { get; }
		void Parry(IActor source, IWeapon weapon);
	}

	[Flags]
	public enum ParriableFlags
	{
		None = 0,
		Weapon = 1,
		Projectile = 2,
		Misc = 4,
		NeverParried = 8
	}
}
