using System;

using UnityEngine;

using YaEm.Core;
using YaEm.Weapons;

namespace YaEm.Ability
{
	public sealed class Deflector : IAbility
	{
		private static readonly Collider2D[] _cachedColliders = new Collider2D[32];
		private readonly float _radius;
		private readonly float _cooldown;
		private readonly bool _deflectDeniesCooldown;
		private readonly LayerMask _projectilesMask;
		private DeflectUsage _instruction;
		private float _elapsed;
		private IActor _actor;

		public Deflector(float radius, float cooldown, float aiCheckTime, bool deflectDeniesCooldown, LayerMask projectilesMask)
		{
			_radius = radius;
			_cooldown = cooldown;
			_deflectDeniesCooldown = deflectDeniesCooldown;
			_projectilesMask = projectilesMask;
			_instruction = new DeflectUsage(aiCheckTime);
			_instruction.SetAbility(this);
		}

		public AbilityType Type => AbilityType.Instant;

		public IAIAbilityInstruction AIAbilityInstruction => _instruction;

		public float Readiness => Mathf.Max(0, _elapsed) / _cooldown;

		public event Action OnActivate;
		public event Action OnDeactivate;

		public bool CanUse()
		{
			return _elapsed < 0;
		}

		public void Init(IActor actor)
		{
			_actor = actor;
		}

		public void Update(float dt)
		{
			_elapsed -= dt;
		}

		public void Use()
		{
			if (_elapsed > 0) return;

			OnActivate?.Invoke();
			bool intercepted = false;
			int overlapCount = Physics2D.OverlapCircleNonAlloc(_actor.Position, Radius, _cachedColliders, _projectilesMask);
			for (int i = 0; i < overlapCount && i < 32; ++i)
			{
				if (_cachedColliders[i].TryGetComponent<Projectile>(out var projectile) && 
					(_actor is ITeamProvider team ? projectile.TeamNumber != team.TeamNumber : projectile.TeamNumber != 0))
				{
					intercepted = true;
					projectile.Parry(_actor, !projectile.MarkedForDestroy ? _actor.Position.GetDirectionNormalized(projectile.Source.Position) : -projectile.Direction);
				}
			}

			_elapsed = _deflectDeniesCooldown && intercepted ? 0f : _cooldown;
		}

		public float Radius => _actor.Scale * _radius;
		public LayerMask ProjectilesMask => _projectilesMask;
	}
}