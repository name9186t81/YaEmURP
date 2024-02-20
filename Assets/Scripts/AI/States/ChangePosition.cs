using YaEm.Core;
using YaEm;

using UnityEngine;

namespace YaEm.AI.States
{
	public sealed class ChangePosition : IUtility
	{
		private const int MAX_POINT_PICK_TRIES = 10;
		private const float MIN_DELAY = 1f;
		private AIController _controller;
		private float _pickRadius;
		private bool _succesful;
		private float _elapsedTime;
		private Vector2 _point;
		public StateType StateType => StateType.Attacking;

		public void Execute()
		{
			if (!_succesful)
			{
				_elapsedTime = MIN_DELAY;
				return;
			}

			if (_controller.IsTargetNull)
			{
				_succesful = false;
				_elapsedTime = MIN_DELAY;
				return;
			}

			_controller.LookAtPoint(_controller.TargetTransform.position);
			_controller.SafeWalk(_point);
			_controller.InitCommand(ControllerAction.Fire);

			if (_controller.Position.DistanceLess(_point, _controller.Actor.Scale))
			{
				_succesful = false;
				_elapsedTime = MIN_DELAY;
				return;
			}

			if(!_point.DistanceLess(_controller.TargetTransform.position, _controller.Weapon.UseRange))
			{
				_succesful = false;
				_elapsedTime = MIN_DELAY;
				return;
			}
		}

		public float GetEffectivness()
		{
			_elapsedTime -= Time.deltaTime;
			return _controller.IsTargetNull ? -100f : Mathf.Abs(Mathf.Min(_elapsedTime, 0)) * _controller.Braveness;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
			_pickRadius = _controller.Weapon != null ? _controller.Weapon.UseRange : 0.1f;
		}

		public void PreExecute()
		{
			if(_controller.TargetTransform == null)
			{
				_succesful = false;
				return;
			}

			for(int i = 0; i < MAX_POINT_PICK_TRIES; i++)
			{
				Vector2 targetPos = _controller.TargetTransform.position;
				Vector2 randomPoint = Vector2Extensions.RandomDirection() * UnityEngine.Random.Range(_pickRadius / 2f, _pickRadius) + targetPos;
				if(_controller.Vision.CanSeePoint(randomPoint, targetPos, true))
				{
					_succesful = true;
					_point = randomPoint;
					Debug.DrawLine(randomPoint, targetPos, Color.green, 2f);
					return;
				}
				Debug.DrawLine(randomPoint, targetPos, Color.red, 2f);
			}
			_elapsedTime = MIN_DELAY;
			_succesful = false;
		}

		public void Undo()
		{
			_controller.StopMoving();
		}
	}
}
