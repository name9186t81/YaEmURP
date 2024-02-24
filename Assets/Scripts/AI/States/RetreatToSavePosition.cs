using HP.AI;

using System.Collections.Generic;

using UnityEngine;

using YaEm.Health;

namespace YaEm.AI.States
{
	public sealed class RetreatToSavePosition : IUtility
	{
		private AIController _controller;
		private PathFinding _pathFinding;
		private IReadOnlyList<Vector2> _path;
		private bool _isWalking;
		private int _pathIndex = 0;
		public StateType StateType => StateType.RunningAway;

		public void Execute()
		{
			if (!_isWalking) return;

			_isWalking = true;
			if (_pathIndex == _path.Count)
			{
				_isWalking = false;
				return;
			}

			Vector2 prevPos = _controller.Position;
			foreach (var path in _path)
			{
				Debug.DrawLine(prevPos, path, color: Color.green);
				prevPos = path;
			}

			Vector2 dest = _path[_pathIndex];
			if (dest.DistanceLess(_controller.Position, _controller.Actor.Scale))
			{
				_pathIndex++;
			}

			_controller.SafeWalk(dest);

			if (!_controller.IsTargetNull)
			{
				_controller.LookAtPoint(_controller.CurrentTarget.Position);
				if(_controller.IsEffectiveToFire(_controller.CurrentTarget.Position))
				{
					_controller.InitCommand(Core.ControllerAction.Fire);
				}
			}
			else
			{
				_controller.LookAtPoint(dest);
			}

		}

		public float GetEffectivness()
		{
			return _controller.LastSavePosition.HasValue ? (_controller.Experience * ((1 - _controller.Aggresivness) * 1 - (_controller.Health.Delta())
				+ (_controller.Weapon.CanAttack ? 1f : 0f))
				+ (1 - _controller.Aggresivness) * Mathf.Lerp(0, 4, _controller.Vision.EnemiesInRangeCount / 6))
				: 0f;
		}

		public void Init(AIController controller)
		{
			_controller = controller;

			if (!ServiceLocator.TryGet<PathFinding>(out _pathFinding))
			{
				Debug.LogError("Cannot find path finder");
			}
		}

		public void PreExecute()
		{
			_path = _pathFinding.FIndPath(_controller.Position, _controller.LastSavePosition.Value);
			_pathIndex = 0;
			_isWalking = true;
		}

		public void Undo()
		{
			_controller.StopMoving();
		}
	}
}