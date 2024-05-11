using YaEm.Core;
using YaEm.Effects;
using System;
using System.Collections.Generic;

using UnityEngine;

namespace YaEm.Health
{
	public class Health : IHealth
	{
		private readonly HashSet<EffectType> _types;
		private readonly IList<IEffect<IHealth>> _effects;
		private readonly int _maxHealth;
		private readonly ITeamProvider _selfTeam;
		private HealthFlags _flags;
		private int _currentHealth;
		private IActor _owner;

		public Health(int maxHealth, int currentHealth, ITeamProvider prov = null)
		{
			_maxHealth = maxHealth;
			_currentHealth = currentHealth;
			_effects = new List<IEffect<IHealth>>();
			_flags = HealthFlags.FriendlyFireDisabled;
			_selfTeam = prov;
			_types = new HashSet<EffectType>();

			_owner = null;
			OnDamage = default;
			OnDeath = default;
		}

		public int CurrentHealth => _currentHealth;

		public int MaxHealth => _maxHealth;

		public IReadOnlyList<IEffect<IHealth>> Effects => (IReadOnlyList<IEffect<IHealth>>)_effects;

		public HealthFlags Flags { get => _flags; set => _flags = value; }
		public IActor Actor { set => _owner = value; get => _owner; }

		public event Action<DamageArgs> OnDamage;
		public event Action<DamageArgs> OnDeath;
		public event Action<DamageArgs> OnPreDamage;

		public bool AddEffect(IEffect<IHealth> effect)
		{
			bool res = effect.CanApply(this);
			if (!res || _types.Contains(effect.Type)) return false;

			_effects.Add(effect);
			_types.Add(effect.Type);
			effect.ApplyEffect(this);
			return true;
		}

		public bool CanTakeDamage(DamageArgs args)
		{
			if(_selfTeam != null && (Flags & HealthFlags.FriendlyFireDisabled) != 0)
			{
				return args.Sender == null || (args.Sender != null && args.Sender is ITeamProvider team && team.TeamNumber != _selfTeam.TeamNumber);
			}
			return (Flags & HealthFlags.Invincible) == 0;
		}

		public bool Contains(EffectType type)
		{
			return _types.Contains(type);
		}

		public void TakeDamage(DamageArgs args)
		{
			//if (args.Sender != null && args.Sender is ITeamProvider prov && prov.TeamNumber == _selfTeam?.TeamNumber && (args.DamageFlags & DamageFlags.Heal) == 0) return;

			if ((Flags & HealthFlags.Invincible) != 0) return;

			OnPreDamage?.Invoke(args);

			if (args.Damage <= 0) return;

			if ((args.DamageFlags & DamageFlags.Heal) != 0)
			{
				_currentHealth = Mathf.Min(_currentHealth + args.Damage, _maxHealth);
			}
			else
			{
				_currentHealth -= args.Damage;
			}

			if (_currentHealth <= 0)
			{
				if (ServiceLocator.TryGet<GlobalDeathNotificator>(out var notificator))
				{
					notificator.Die(args, _owner);
				}
				OnDeath?.Invoke(args);
				return;
			}

			OnDamage?.Invoke(args);
		}

		public void UpdateEffector(float delta)
		{
			for (int i = 0; i < Effects.Count; ++i)
			{
				Effects[i].Update(delta);
				if (Effects[i].State == EffectState.Finished)
				{
					_types.Remove(Effects[i].Type);
					_effects.RemoveAt(i);
					i--;
				}
			}
		}
	}
}
