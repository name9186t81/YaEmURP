using HP.AI;

using System.Collections.Generic;
using UnityEngine;

namespace YaEm.AI.States
{
	public class WanderInMap : IUtility
	{
		private PathFinding _pathFinding;
		private AIController _controller;
		private float _elapsedCooldown;
		private bool _isWalking;
		private float _maxCooldown = 10f;
		private IReadOnlyList<Vector2> _path;
		private int _pathIndex = 0;
		public StateType StateType => StateType.Idling;

		public void Execute()
		{
			if (!_isWalking) return;

			_isWalking = true;
			if(_pathIndex == _path.Count)
			{
				_isWalking = false;
				_elapsedCooldown = _maxCooldown;
				return;
			}

			Vector2 prevPos = _controller.Position;
			foreach(var path in _path)
			{
				Debug.DrawLine(prevPos, path, color: Color.green);
				prevPos = path;
			}

			Vector2 dest = _path[_pathIndex];
			if(dest.DistanceLess(_controller.Position, _controller.Actor.Scale))
			{
				_pathIndex++;
			}

			_controller.SafeWalk(dest);
			_controller.LookAtPoint(dest);
		}

		public float GetEffectivness()
		{
			_elapsedCooldown -= Time.deltaTime;

			return _isWalking ? 2f : _elapsedCooldown < 0 ? 1f : -1f;
		}

		public void Init(AIController controller)
		{
			if(!ServiceLocator.TryGet<PathFinding>(out _pathFinding))
			{
				Debug.LogError("Cannot find path finder");
			}
			_controller = controller;
		}

		public void PreExecute()
		{
			_path = _pathFinding.GetRandomPath(_controller.Position, out _);
			if (_path == null)
			{
				_elapsedCooldown = _maxCooldown;
				_isWalking = false;
				return;
			}

			_pathIndex = 0;
			_isWalking = true;
		}

		public void Undo()
		{
		}
	}
}