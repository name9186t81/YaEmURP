using YaEm.Effects;
using YaEm.Core;
using System;

namespace YaEm.Weapons
{
	public interface IWeapon : IActorComponent, IEffector<IWeapon>
	{
		bool CanAttack { get; }
		float DeltaBeforeAttack { get; }
		float EffectiveRange { get; }
		float UseRange { get; }
		WeaponFlags Flags { get; }
		bool ParryState { get; }
		float DamageMultiplier { get; set; }
		float ReloadMultiplier { get; set; }
		float SpeedMultiplier { get; set; }
		bool IsInited{get; }
		event Action OnInit;
		event Action OnAttack;
		event Action OnAttackEnded;
	}

	[Flags]
	public enum WeaponFlags
	{
		None = 0,
		CanParry = 1,
		Melee = 2,
		Ranged = 4,
		Mixed = 8,
		PreAim = 16
	}
}
