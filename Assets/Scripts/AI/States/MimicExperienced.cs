using HP.AI;

using System.Collections.Generic;

using UnityEngine;

using YaEm.Core;
using YaEm.Health;

namespace YaEm.AI.States
{
	public sealed class MimicExperienced : IUtility
	{
		private IActor _leader;
		private IHealth _leaderHealth;
		private AIController _leaderController;
		private AIController _controller;
		private AIVision _vision;

		private IReadOnlyList<Vector2> _path;
		private int _index;

		public StateType StateType => StateType.Idling;

		~MimicExperienced()
		{
			_vision.OnScan -= FindExperienced;
		}

		public void Execute()
		{
			if(_leaderController != null && 
				!_leaderController.IsTargetNull && 
				(_controller.IsTargetNull || _controller.CurrentTarget != _leaderController.CurrentTarget) &&
				_leader.Position.DistanceLess(_controller.Position, (_leader.Scale + _controller.Actor.Scale) * 4f))
			{
				float dot = Vector2.Dot(_leader.DesiredMoveDirection, _leader.Position.GetDirectionNormalized(_leaderController.CurrentTarget.Position));

				if (dot > 0.8f)
				{
					_controller.UpdateTarget(_leaderController.CurrentTarget, false);
					if (_vision.CanSeeTarget(_leaderController.CurrentTarget, false))
					{
						_controller.ForgotTarget(true);
					}
					return;
				}
			}

			if(_path != null)
			{
				_controller.SafeWalk(_path[_index]);
				_controller.LookAtPoint(_path[_index]);
				if (_controller.Position.DistanceLess(_path[_index], _controller.Actor.Scale * 2f))
				{
					_index++;
					if(_index >= _path.Count )
					{
						PreExecute();
						return;
					}
				}
			}
			else
			{
				if (!_vision.CanSeeTarget(_leader, true))
				{
					PreExecute();
					return;
				}

				_controller.LookAtPoint(_leader.Position);
				_controller.SafeWalk(_leader.Position);
			}
		}

		public float GetEffectivness()
		{
			return _leader == null ? -100f :
				_leader.Position.DistanceLess(_controller.Position, (_controller.Actor.Scale + _leader.Scale) * 2f) ? -100f :
				(1 - _controller.Experience) * (_leaderController == null ? 0.75f : _leaderController.Experience) * 4f;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
			_vision = controller.Vision;
			_vision.OnScan += FindExperienced;
		}

		private void FindExperienced()
		{
			if (_leader != null) return;

			int count = _vision.AlliesInRangeCount;
			float selfExp = _controller.Experience;

			for(int i = 0; i < count; i++)
			{
				var ally = _vision.AliesInRange[i];
				if(ally.Controller != null)
				{
					float exp = ally.Controller.Type == ControllerType.AI ? (ally.Controller as AIController).Experience : 0.75f;

					if(exp > selfExp)
					{
						_leader = ally;

						if(_leader.Controller.Type == ControllerType.AI)
						{
							_leaderController = _leader.Controller as AIController;
						}

						if(_leader is IProvider<IHealth> prov && prov.Value != null)
						{
							_leaderHealth = prov.Value;
							_leaderHealth.OnDeath += LeaderDied;
						}
						return;
					}
				}
			}
		}

		private void LeaderDied(DamageArgs obj)
		{
			_leaderHealth.OnDeath -= LeaderDied;
			_leader = null;
			_leaderController = null;
		}

		public void PreExecute()
		{
			if(!_vision.CanSeeTarget(_leader, true))
			{
				if(ServiceLocator.TryGet<PathFinding>(out var pathFinding))
				{
					_path = pathFinding.FindPath(_controller.Position, _leader.Position);
					_index = 0;
				}
			}
			else
			{
				_path = null;
				_index = 0;
			}
		}

		public void Undo()
		{

		}
	}
}