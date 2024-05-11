using System;

using UnityEngine;

using YaEm.Core;
using YaEm.Health;

namespace YaEm.Ability
{
	public sealed class ShieldAbility : IAbility, IProvider<IHealth>
	{
		private readonly int _maxHealth;
		private readonly float _breakCooldown;
		private readonly float _damageCooldown;
		private readonly float _regenerationTime;
		private readonly float _regenerationTick;
		private readonly bool _canRegenerate;

		private DamageArgs _cachedArgs;
		private int _healFactor;
		private float _regenerationElapsed;
		private float _damageElapsed;
		private float _deathElapsed;
		private bool _broken;

		private IHealth _health;

		public ShieldAbility(int maxHealth, float breakCooldown, float damageCooldown, float regenerationTime, float tick, bool canRegenerate)
		{
			_maxHealth = maxHealth;
			_breakCooldown = breakCooldown;
			_damageCooldown = damageCooldown;
			_regenerationTime = regenerationTime;
			_canRegenerate = canRegenerate;
			_regenerationTick = tick;

			_healFactor = (int)Mathf.Ceil(_maxHealth / (_regenerationTime * _regenerationTick));
		}

		public AbilityType Type => AbilityType.Passive;

		public IAIAbilityInstruction AIAbilityInstruction => null;

		public float Readiness => _health.Delta();

		public ref IHealth Value => ref _health;

		public event Action OnActivate;
		public event Action OnDeactivate;

		public bool CanUse()
		{
			return true;
		}

		public void Init(IActor actor)
		{
			if(actor is not IProvider<IHealth> prov)
			{
				Debug.LogError("Actor does not have health");
				return;
			}

			_cachedArgs = new DamageArgs(actor, _healFactor, DamageFlags.Heal | DamageFlags.NoWeapon);
			_health = new Health.Health(_maxHealth, _maxHealth, (actor is ITeamProvider pr) ? pr : null);
			prov.Value.OnPreDamage += Damaged;
			_health.OnDeath += ShieldBroke;
			_health.OnDamage += ShieldDamaged;
		}

		private void ShieldDamaged(DamageArgs obj)
		{
			_damageElapsed = _damageCooldown;
		}

		private void ShieldBroke(DamageArgs obj)
		{
			_deathElapsed = _breakCooldown;
			_broken = true;
		}

		private void Damaged(DamageArgs obj)
		{
			if ((obj.DamageFlags & DamageFlags.Heal) != 0) return;

			int remaining = obj.Damage - _health.CurrentHealth;
			_health.TakeDamage(obj);

			remaining = Math.Clamp(remaining, 0, int.MaxValue);
			obj.Damage = remaining;
		}

		public void Update(float dt)
		{
			_damageElapsed -= dt;
			_deathElapsed -= dt;

			if(!_broken && _canRegenerate && _damageElapsed < 0 && _health.Delta() < 1)
			{
				_regenerationElapsed -= dt;
				if(_regenerationElapsed < 0)
				{
					_health.TakeDamage(_cachedArgs);
					_regenerationElapsed = _regenerationTime;
				}
			}

			if(_broken && _deathElapsed < 0)
			{
				//_health.TakeDamage(new DamageArgs(null, _maxHealth, DamageFlags.Heal | DamageFlags.NoWeapon));
				_broken = false;
			}
		}

		public void Use()
		{
		}
	}
}