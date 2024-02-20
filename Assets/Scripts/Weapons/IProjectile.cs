using YaEm.Core;
using YaEm.Health;
using System;
using UnityEngine;

namespace YaEm.Weapons
{
	public interface IProjectile
	{
		float Speed { get; }
		IActor Source { get; }
		Vector2 Direction { get; }
		Vector2 Position { get; }
		Vector2 StartPosition { get; }
		ProjectileFlags ProjectileFlags { get; }
		event Action<IActor> OnParry;
		event Action OnInit;
		bool TryChangeDirection(in Vector2 newDirection);
		void Init(Pool<IProjectile> pool, DamageArgs args, int teamNumber, in Vector2 startPos, in Vector2 direction, IActor owner, float speedModifier);
		void Parry(IActor source, in Vector2 parryDirection);
		void Destroy();
	}

	[Flags]
	public enum ProjectileFlags
	{
		None = 0,
		Parriable = 1,
		NotMutable = 2,
		NoSource = 4,
		HaveTeam = 8,
		Frozen = 16,
		GoThroughTargets = 32
	}
}
