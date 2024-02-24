using System;

using UnityEngine;

using YaEm.Core;
using YaEm.Weapons;

namespace YaEm.Movement
{
	public sealed class WeaponPushBack : MonoBehaviour, IForce
	{
		[SerializeField] private RangedWeapon _weapon;
		[SerializeField] private float _range;
		[SerializeField] private float _duration;
		[SerializeField] private AnimationCurve _speedGrow;
		private Motor _motor;
		private float _elapsed;
		private float _area;
		private Vector2 _factor;
		private ForceState _state;
		private Vector2 _shootDirection;
		private bool _isPlaying;

		private void Awake()
		{
			_weapon.OnInit += Init;
			_area = _speedGrow.Area();
		}

		private void Init()
		{
			_weapon.OnInit -= Init;

			if(_weapon.Actor == null || _weapon.Actor is not IProvider<Motor> prov)
			{
				Debug.LogWarning("Weapon does not have owner or owner is not movable. Removing weapon recoil...");
				return;
			}

			_motor = prov.Value;
			_weapon.OnFire += Fired;
		}

		private void Fired()
		{
			if (_isPlaying) return;

			_isPlaying = true;
			_motor.AddForce(this);
			_state = ForceState.Alive;
			_shootDirection = _weapon.ShootDirection;
			_factor = -_shootDirection * _range / (_area * _duration);
		}

		public Func<Vector2, Vector2> ForceFunc => Evaluate;

		public ForceState State { get => _state; set => _state = value; }

		private Vector2 Evaluate(Vector2 worldPos)
		{
			_elapsed += Time.deltaTime;
			float delta = _elapsed.Delta(_duration);
			if(delta > 1f)
			{
				_isPlaying = false;
				_state = ForceState.Destroyed;
				_elapsed = 0f;
				_motor.RemoveForce(this);
			}

			return _factor * _speedGrow.Evaluate(delta);
		}
	}
}