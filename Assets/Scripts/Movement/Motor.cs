using System;
using YaEm.Core;
using System.Collections.Generic;
using UnityEngine;

namespace YaEm.Movement
{
	public sealed class Motor
	{
		private readonly List<IForce> _forces = new List<IForce>();
		private readonly float _speed;
		private readonly float _rotationSpeed;
		private readonly IActor _actor;
		private readonly ITransformProvider _provider;
		private readonly Transform _transform;
		private readonly GlobalTimeModifier _timeModificator;
		private float _timeMod = 1f;
		private Vector2 _lastVelocity;
		private BaseForce _movementForce;
		private IController _controller;
		private float _rotationVelocity;
		public event Action<float> _updateFunc;

		private List<int> _check = new List<int>();
		public Motor(float speed, float rotationSpeed, IActor controller, Transform transform, ITransformProvider provider)
		{
			_speed = speed;
			_rotationSpeed = rotationSpeed;
			_actor = controller;
			_controller = _actor.Controller;
			_actor.OnControllerChange += (IController oldC, IController newC) =>
			{
				_controller = newC;
				_movementForce = new BaseForce((_) => _controller.DesiredMoveDirection * _speed);
			};
			_provider = provider;
			_transform = transform;

			if (_controller == null)
			{
				Debug.LogWarning($"Actor: {controller.Name} does not have controller");
				return;
			}

			if (!ServiceLocator.TryGet<GlobalTimeModifier>(out _timeModificator))
			{
				Debug.LogWarning("Cannot find Global time modificator");
			}
			else
			{
				_timeMod = _controller == null || _controller.IsEffectedBySlowMotion ? _timeModificator.TimeModificator : 1f;
				_timeModificator.OnTimeModificated += TimeModded;
			}

			_movementForce = new BaseForce((_) => _controller.DesiredMoveDirection * _speed);
		}

		~Motor()
		{
			_timeModificator.OnTimeModificated -= TimeModded;
		}

		private void TimeModded()
		{
			_timeMod = _controller == null || _controller.IsEffectedBySlowMotion ? _timeModificator.TimeModificator : 1f;
			Debug.Log(_timeModificator.TimeModificator + " " + _timeMod);
		}

		public void AddRotationVelocity(float velocity)
		{
			_rotationVelocity += velocity;
		}

		public void Update(float deltaTime)
		{
			if (_actor == null) return;
			UpdateForces();
			Vector2 velocity = SummarizeForces();
			_lastVelocity = velocity + _movementForce.ForceFunc(_actor.Position);
			_rotationVelocity *= 0.8f;

			_provider.Velocity = velocity;

			_updateFunc?.Invoke(deltaTime);

			_transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.Euler(0, 0, _actor.DesiredRotation + _rotationVelocity), _rotationSpeed * deltaTime * _timeMod);

			_provider.Velocity = (velocity + _movementForce.ForceFunc(_actor.Position)) * _timeMod;
		}

		private void UpdateForces()
		{
			for(int i = 0; i < _forces.Count; i++)
			{
				if (_forces[i].State == ForceState.Destroyed)
				{
					_forces.RemoveAt(i);
					i--;
				}
			}
		}

		public void AddForce(IForce force)
		{
			_forces.Add(force);
		}

		public bool RemoveForce(IForce force)
		{
			if (_forces.Contains(force))
			{
				force.State = ForceState.Destroyed;
				return true;
			}
			return false;
		}

		private Vector2 SummarizeForces()
		{
			Vector2 result = new Vector2();
			for (int i = 0, length = _forces.Count; i < length; i++)
			{
				result += _forces[i].ForceFunc(_actor.Position);
			}
			return result;
		}

		public Vector2 LastVelocity => _lastVelocity;
		public float Speed => _speed;
		public Vector2 MoveDirection => _controller.DesiredMoveDirection;
	}
}