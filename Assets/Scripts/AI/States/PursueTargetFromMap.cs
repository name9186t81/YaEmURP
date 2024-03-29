﻿using YaEm.Core;

using HP.AI;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace YaEm.AI.States
{
	public sealed class PursueTargetFromMap : IUtility
	{
		private TargetMapProvider _targetMap;
		private PathFinding _pathFinding;
		private AIController _controller;
		private bool _hasPath = false;
		private Vector2 _endPoint;
		private IReadOnlyList<Vector2> _points;
		private int _lastPointIndex = 0;

		StateType IUtility.StateType => StateType.Pursing;

		void IUtility.Execute()
		{
			if (!_hasPath) return;

			if (_lastPointIndex == _points.Count)
			{
				if (_controller.Position.DistanceLess(_endPoint, 0.1f))
				{
					_hasPath = false;
					return;
				}

				_points = _pathFinding.FIndPath(_controller.Position, _endPoint);
				if(_points.Count == 0)
				{
					_hasPath = false;
					return;
				}
				_lastPointIndex = 0;
			}

			var prevPoint = _points[0];
			foreach(var p in _points)
			{
				Debug.DrawLine(prevPoint, p, Color.red);
				prevPoint = p;
			}

			Vector2 point = _points[_lastPointIndex];

			if (_controller.Position.DistanceLess(point, _controller.Actor.Scale)) _lastPointIndex++;

			_controller.SafeWalk(point);
			_controller.LookAtPoint(point);

		}

		float IUtility.GetEffectivness()
		{
			if (_hasPath) return 10f;
			return UnityEngine.Random.Range(0, 1f) < _controller.TeamWork / 10f & _controller.TargetTransform == null && _targetMap != null && _targetMap.GetRandomTarget(_controller.Actor is ITeamProvider prov ? prov.TeamNumber : 0, out _) ? 2f : -2f;
		}

		void IUtility.Init(AIController controller)
		{
			_controller = controller;
			if(!ServiceLocator.TryGet<TargetMapProvider>(out _targetMap))
			{
				Debug.LogError("No target map found");
				return;
			}
			if (!ServiceLocator.TryGet<PathFinding>(out _pathFinding))
			{
				Debug.LogError("No pathfinding found");
				return;
			}
		}

		void IUtility.PreExecute()
		{
			if(_targetMap.GetRandomTarget(_controller.Actor is ITeamProvider prov ? prov.TeamNumber : 0, out Vector2 target))
			{
				_endPoint = target;
				_points = _pathFinding.FIndPath(_controller.Position, target);
				_lastPointIndex = 0;
				_hasPath = true;
			}
		}

		void IUtility.Undo()
		{
		}
	}
}
