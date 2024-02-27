using Mono.Cecil.Cil;

using System;

using UnityEngine;

using YaEm.Core;
using YaEm.Effects;
using YaEm.Health;
using YaEm.Movement;
using YaEm.Weapons;

namespace YaEm.Ability
{
	public sealed class HealAbility : IAbility, IEffect<IWeapon>, IForce
	{
		private readonly float _healAmmount;
		private readonly float _speedIncrease;
		private readonly float _weaponSpeedIncrease;
		private readonly float _cooldown;
		private readonly float _duration;
		private readonly IAIAbilityInstruction _instruction;

		private IActor _actor;
		private bool _isInCooldown;
		private ForceState _forceState;
		private EffectState _state;
		private bool _used;
		private IHealth _health;
		private IWeapon _weapon;
		private Motor _motor;
		private float _elapsed;
		private float _elapsedDuration;

		public HealAbility(float healAmmount, float speedIncrease, float weaponSpeedIncrease, float cooldown, float duration, IAIAbilityInstruction instruction)
		{
			_healAmmount = healAmmount;
			_speedIncrease = speedIncrease;
			_weaponSpeedIncrease = weaponSpeedIncrease;
			_cooldown = cooldown;
			_duration = duration;
			_instruction = instruction; 
		}

		public AbilityType Type => AbilityType.Instant;

		public IAIAbilityInstruction AIAbilityInstruction => _instruction;

		public float Readiness => _elapsedDuration / _cooldown;

		public EffectState State => _state;

		public Func<Vector2, Vector2> ForceFunc => SolveForce;

		EffectType IEffect<IWeapon>.Type => EffectType.Adrenaline;

		ForceState IForce.State { get => _forceState; set => _forceState = value; }

		public event Action OnActivate;
		public event Action OnDeactivate;

		public void ApplyEffect(in IWeapon obj)
		{
		}

		public bool CanApply(in IWeapon obj)
		{
			return CanUse();
		}

		public bool CanUse()
		{
			return !_used;
		}

		public void Init(IActor actor)
		{
			_actor = actor;
			if (_actor is IProvider<Motor> prov && _speedIncrease != 0)
			{
				_motor = prov.Value;
			}

			_instruction.SetAbility(this);
			if (_actor is IProvider<IHealth> prov2 && _healAmmount != 0)
			{
				_health = prov2.Value;
			}

			if (_actor is IProvider<IWeapon> prov3 && _weaponSpeedIncrease != 0)
			{
				_weapon = prov3.Value;
			}
		}

		public void Update(float dt)
		{
			if (!_used) return;

			_elapsed += dt;
			if(_elapsed > _duration && !_isInCooldown)
			{
				if(_motor != null)
				{
					_motor.RemoveForce(this);
				}
				if(_weapon!= null)
				{
					_weapon.ReloadMultiplier += _weaponSpeedIncrease;
				}
				Debug.Log("Done");
				_isInCooldown = true;
				OnDeactivate?.Invoke();
			}
			if (_isInCooldown)
			{
				_elapsedDuration += dt;
				if(_elapsedDuration > _cooldown)
				{
					_isInCooldown = false;
					_elapsedDuration = 0;
					_elapsed = 0f;
					_used = false;
				}
			}
		}

		public void Use()
		{
			if(!CanUse()) return;

			if(_health != null)
			{
				_health.TakeDamage(new DamageArgs(null, (int)(_health.MaxHealth * _healAmmount), DamageFlags.Heal));
			}
			if(_weapon != null)
			{
				_weapon.ReloadMultiplier -= _weaponSpeedIncrease;
			}
			if(_motor != null)
			{
				_motor.AddForce(this);
			}

			Debug.Log("Used");
			OnActivate?.Invoke();
			_used = true;
		}

		private Vector2 SolveForce(Vector2 world)
		{
			return _motor.MoveDirection * _speedIncrease * _motor.Speed;
		}

		public float HealAmount => _healAmmount;
	}
}