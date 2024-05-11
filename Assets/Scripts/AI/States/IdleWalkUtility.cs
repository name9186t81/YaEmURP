using UnityEngine;

namespace YaEm.AI.States
{
	public sealed class IdleWalkUtility : IUtility
	{
		private AIController _controller;
		private readonly float _walkCooldown;
		private readonly float _pointPickRadius;
		private readonly float _pointPickRadiusMin;

		private Vector2 _point;
		private float _elapsedCooldown;
		private bool _isWalking;

		private const int MAX_POINT_PICK_TRIES = 10;

		public StateType StateType => StateType.Idling;

		public IdleWalkUtility(float walkCooldown, float pointPickRadius, float pointPickRadiusMin)
		{
			_walkCooldown = walkCooldown;
			_pointPickRadius = pointPickRadius;
			_pointPickRadiusMin = pointPickRadiusMin;
		}

		public void Execute()
		{
			_controller.SafeWalk(_point);
			_controller.LookAtPoint(_point);

			if (_controller.Position.DistanceLess(_point, _controller.Actor.Scale * 2f))
			{
				_isWalking = false;
				_elapsedCooldown = _walkCooldown;
			}
		}

		public float GetEffectivness()
		{
			_elapsedCooldown -= Time.deltaTime;
			_elapsedCooldown = Mathf.Clamp(_elapsedCooldown, 0, _walkCooldown);

			return _isWalking ? 2f : _elapsedCooldown == 0 ? 1f : -1f;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
		}

		public void PreExecute()
		{
			_isWalking = true;
			for (int i = 0; i < MAX_POINT_PICK_TRIES; i++)
			{
				if (TryPickPoint()) return;
			}

			_isWalking = false;
			_elapsedCooldown = _walkCooldown;
		}

		private bool TryPickPoint()
		{
			_point = Vector2Extensions.RandomDirection() * PointPickRadius + _controller.Position;
			return _controller.Vision.CanSeePoint(_point, true);
		}

		public void Undo()
		{
			_elapsedCooldown = _walkCooldown;
			_isWalking = false;
		}

		public float PointPickRadius => Random.Range(_pointPickRadiusMin, _pointPickRadius);
	}
}
