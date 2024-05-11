using YaEm.Core;

using YaEm.Movement;

using UnityEngine;

namespace YaEm.AI.States
{
	internal class Intimidate : IUtility
	{
		private readonly float _delay;
		private readonly float _slowdownAmount;
		private readonly float _intimidatingTime;
		private float _elapsed;
		private float _elapsed2;
		private bool _running;
		private Vector2 _target;
		private AIController _controller;

		public Intimidate(float delay, float intimidatingTime, float slownDownAmount)
		{
			_delay = _elapsed = delay;
			_intimidatingTime = intimidatingTime;
			_slowdownAmount = slownDownAmount;
		}

		public StateType StateType => StateType.Initimidating;

		public void Execute()
		{
			if(_elapsed2 < 0)
			{
				_running = false;
				_elapsed = _delay;
			}

			_elapsed2 -= Time.deltaTime;
			_target = _controller.TargetTransform != null ? (Vector2)_controller.TargetTransform.position : _target;
			_controller.LookAtPoint(_target);
		}

		public float GetEffectivness()
		{
			if (_running) return 100f;
			_elapsed -= Time.deltaTime;
			return _controller.TargetTransform == null ? -100f : _elapsed < 0 ? _controller.Experience *  Mathf.Abs(_elapsed) : 0;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
		}

		public void PreExecute()
		{
			_elapsed2 = _intimidatingTime;
			_running = true;
			_controller.StopMoving();

			if(_slowdownAmount != 0 && _controller.TargetTransform != null && _controller.CurrentTarget is IProvider<Motor> prov)
			{
				var force = ForceFactory.GetParameterizedForce();
				force[ParameterizedForceKey.Motor] = prov.Value;
				force[ParameterizedForceKey.ElapsedTime] = force[ParameterizedForceKey.MaxTime] = _intimidatingTime * 2f;
				force[ParameterizedForceKey.Strength] = _slowdownAmount;
				force.SetForce(
					(_) =>
					{
						float elapsed = (float)force[ParameterizedForceKey.ElapsedTime];
						elapsed -= Time.deltaTime;
						force[ParameterizedForceKey.ElapsedTime] = elapsed;
						Motor motor = (force[ParameterizedForceKey.Motor] as Motor);

						if (elapsed < 0f)
						{
							motor.RemoveForce(force);
							return Vector2.zero;
						}

						return -motor.MoveDirection * motor.Speed * (float)force[ParameterizedForceKey.Strength];
					});
				prov.Value.AddForce(force);

				if(_controller.Actor is IProvider<Motor> prov2)
				{
					var force2 = ForceFactory.GetParameterizedForce();
					force2[ParameterizedForceKey.Motor] = prov2.Value;
					force2[ParameterizedForceKey.ElapsedTime] = force2[ParameterizedForceKey.MaxTime] = _intimidatingTime * 2f;
					force2[ParameterizedForceKey.Strength] = _slowdownAmount;
					force2.SetForce(
						(_) =>
						{
							float elapsed = (float)force2[ParameterizedForceKey.ElapsedTime];
							elapsed -= Time.deltaTime;
							force2[ParameterizedForceKey.ElapsedTime] = elapsed;
							Motor motor = (force2[ParameterizedForceKey.Motor] as Motor);

							if (elapsed < 0f)
							{
								motor.RemoveForce(force2);
								return Vector2.zero;
							}

							return motor.MoveDirection * motor.Speed * (float)force2[ParameterizedForceKey.Strength];
						});
					prov2.Value.AddForce(force2);
				}
			}
		}

		public void Undo()
		{
		}
	}
}
