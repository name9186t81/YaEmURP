using System;

using UnityEngine;

using YaEm.Core;
using YaEm.Health;

namespace YaEm.Ability
{
	public sealed class HealOtherAbility : IAbility
	{
		private readonly float _healTick;
		private readonly int _healAmmount;
		private readonly float _healRadius;
		private readonly LayerMask _checkMask;
		private readonly LayerMask _wallsMask;

		private static Collider2D[] _cachedColliders = new Collider2D[32];
		private ITeamProvider _selfProvider;
		private IActor _actor;
		private IHealth _target;
		private IActor _targetActor;
		private float _elapsed;
		private DamageArgs _cachedArgs;
		private DamageArgs _cachedSelfArgs;

		public event Action OnTick;

		public HealOtherAbility(float healTick, int healAmmount, float healRadius, LayerMask checkMask, LayerMask wallsMask)
		{
			_healTick = healTick;
			_healAmmount = healAmmount;
			_healRadius = healRadius;
			_checkMask = checkMask;
			_wallsMask = wallsMask;
		}

		public AbilityType Type => AbilityType.Passive;

		public IAIAbilityInstruction AIAbilityInstruction => null;

		public float Readiness => 1f;

		public event Action OnActivate;
		public event Action OnDeactivate;

		public bool CanUse()
		{
			return true;
		}

		public void Init(IActor actor)
		{
			_actor = actor;

			if (_actor is ITeamProvider prov) _selfProvider = prov;

			_cachedArgs = new DamageArgs(actor, _healAmmount, DamageFlags.Heal | DamageFlags.NoWeapon);
			_cachedSelfArgs = new DamageArgs(actor, Mathf.Max(1, _healAmmount / 4), DamageFlags.Heal | DamageFlags.NoWeapon);
		}

		public void Update(float dt)
		{
			_elapsed += dt;

			if (_elapsed < _healTick) return;

			if(_target != null)
			{
				_elapsed = 0;

				var ray = Physics2D.Linecast(_targetActor.Position, _actor.Position, _wallsMask);
				if (_target.Delta() > 0.99 || ray)
				{
					TargetDied(null);
					OnTick?.Invoke();
					return;
				}

				_target.TakeDamage(_cachedArgs);
				if(_actor is IProvider<IHealth> health)
				{
					health.Value.TakeDamage(_cachedSelfArgs);
				}
				OnTick?.Invoke();
				return;
			}

			var overlap = Physics2D.OverlapCircleNonAlloc(_actor.Position, _healRadius, _cachedColliders, _checkMask);
			if (overlap <= 1)
			{
				OnTick?.Invoke();
				return;
			}

			for(int i = 0; i < overlap; ++i)
			{
				if (!_cachedColliders[i].TryGetComponent<IActor>(out var act) || 
					act is not ITeamProvider prov ||
					act is not IProvider<IHealth> prov2 || prov2.Value == null) continue;

				if(act == _actor || 
					!(_selfProvider == null && prov.TeamNumber == 0 
					|| _selfProvider.TeamNumber == prov.TeamNumber)
					||(prov2.Value != null && prov2.Value.Delta() > 0.95)) continue;

				if (Physics2D.Linecast(_actor.Position, act.Position, _wallsMask)) continue;

				_target = prov2.Value;
				_target.OnDeath += TargetDied;
				_targetActor = act;
				OnTick?.Invoke();
				return;
			}

			OnTick?.Invoke();
		}

		private void TargetDied(DamageArgs obj)
		{
			_target.OnDeath -= TargetDied;
			_target = null;
			_targetActor = null;
		}

		public void Use()
		{
		}

		public IActor Target => _targetActor;
		public IHealth TargetHealth => _target;
	}
}