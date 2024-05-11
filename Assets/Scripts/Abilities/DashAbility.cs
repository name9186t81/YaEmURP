using System;

using UnityEngine;

using YaEm.Core;
using YaEm.Effects;
using YaEm.Health;
using YaEm.Movement;

namespace YaEm.Ability
{
	public sealed class DashAbility : IDirectionalAbility, IForce
	{
		private Invincible _invincible;	
		private IHealth _health;
		private Motor _motor;
		private Vector2 _direction;
		private Vector2 _factor;
		private bool _isActive;
		private bool _cooling;
		private bool _invincibleOnActivate;
		private float _elapsed;
		private float _elapsedCooldown;
		private ForceState _state;
		private float _readiness = 1f;

		private readonly IAIAbilityInstruction _instruction;
		private readonly AnimationCurve _growCurve;
		private readonly float _distance;
		private readonly float _duration;
		private readonly float _cooldown;
		private readonly float _area;

		public DashAbility(AnimationCurve growCurve, float distance, float duration, float cooldown, IAIAbilityInstruction instruction, bool invincibleOnActivate)
		{
			_growCurve = growCurve;
			_distance = distance;
			_duration = duration;
			_area = growCurve.Area();
			_instruction = instruction;
			_cooldown = cooldown;
			_invincibleOnActivate = invincibleOnActivate;
		}

		public Vector2 Direction { set => _direction = value; }

		public AbilityType Type => AbilityType.Directional;

		public IAIAbilityInstruction AIAbilityInstruction => _instruction;

		public Func<Vector2, Vector2> ForceFunc => Evalute;

		public ForceState State { get => _state; set => _state = value; }

		public event Action OnActivate;
		public event Action OnDeactivate;

		private Vector2 Evalute(Vector2 worldPos)
		{
			_elapsed += Time.deltaTime;
			float delta = _elapsed.Delta(_duration);
			if (delta > 1f)
			{
				_isActive = false;
				_state = ForceState.Destroyed;
				_elapsed = 0f;
				_motor.RemoveForce(this);
				OnDeactivate?.Invoke();
				_cooling = true;
			}

			return _factor * _growCurve.Evaluate(delta);
		}

		public bool CanUse()
		{
			return !(_isActive || _cooling);
		}

		public void Init(IActor actor)
		{
			if (actor is IProvider<Motor> prov)
			{
				_motor = prov.Value;
			}
			else
			{
				Debug.LogWarning("Actor " + actor.Name + " is not movable");
			}

			if(actor is IProvider<IHealth> prov2 && _invincibleOnActivate)
			{
				_health = prov2.Value;

				EffectBuilder<IHealth> builder = new EffectBuilder<IHealth>().SetDuration(_duration);
				_invincible = (Invincible)builder.Build(EffectType.Invincible);
			}

			actor.OnAction += ReadAction;
			_instruction.SetAbility(this);
		}

		private void ReadAction(ControllerAction obj)
		{
			if (obj == ControllerAction.UseAbility || obj == ControllerAction.Dash) Use();
		}

		public void Update(float dt)
		{
			if (!_cooling) return;

			if (_invincibleOnActivate)
			{
				_invincible.Update(dt);
			}

			_elapsedCooldown += dt;
			_readiness = _elapsedCooldown / _cooldown;
			if (_elapsedCooldown > _cooldown)
			{
				_cooling = false;
				_elapsedCooldown = 0f;

				if(_invincibleOnActivate && _health != null)
				{
					_invincible.Break();
				}
			}
		}

		public void Use()
		{
			if (!CanUse()) return;

			if (_invincibleOnActivate && _health != null)
			{
				_invincible.ApplyEffect(_health);
			}

			_isActive = true;
			_factor = _direction * _distance / (_area * _duration);
			_state = ForceState.Alive;
			_elapsed = 0f;
			_motor.AddForce(this);
			OnActivate?.Invoke();
		}

		public float Length => _distance;

		public float Readiness => _readiness;
	}
}