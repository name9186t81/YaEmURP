using HP.AI;

using System.Collections.Generic;

using UnityEngine;

using YaEm.Ability;
using YaEm.Core;
using YaEm.Health;

namespace YaEm.AI.States
{
	public sealed class StayNearMedic : IUtility
	{
		private IActor _medic;
		private AIVision _vision;
		private AIController _controller;
		private IReadOnlyList<Vector2> _path;
		private int _index;
		public StateType StateType => StateType.Idling;

		~StayNearMedic()
		{
			if(_vision != null)
				_vision.OnScan -= SearchMedic;
		}

		public void Execute()
		{
			Vector2 point;
			if(_path != null)
			{
				FollowPath();
				point = _path[_index];
			}
			else
			{
				_controller.SafeWalk(_medic.Position);
				point = _medic.Position;
			}

			//run and gun
			if(!_controller.IsTargetNull)
			{
				_controller.LookAtPoint(_controller.CurrentTarget.Position);

				if(_controller.IsEffectiveToFire(_controller.CurrentTarget.Position))
				{
					_controller.InitCommand(ControllerAction.Fire);
				}
			}
			else
			{
				_controller.LookAtPoint(point);
			}
		}

		private void FollowPath()
		{
			Vector2 point = _path[_index];
			if(point.DistanceLess(_controller.Position, _controller.Actor.Scale))
			{
				_index++;

				if(_index == _path.Count)
				{
					_path = null; //found path try to reach medic again
					PreExecute();
					return;
				}
			}

			_controller.SafeWalk(point);
			_controller.LookAtPoint(point);
		}
		public float GetEffectivness()
		{
			return _medic == null ? -100f :
				_medic.Position.DistanceLess(_controller.Position, (_medic.Scale + _controller.Actor.Scale) * 4f) ? -100f :
				(1 - _controller.Health.Delta()) * 2 * Mathf.Lerp(1, 2f, 1 - _controller.Braveness) * _controller.TeamWork;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
			_vision = controller.Vision;
			_vision.OnScan += SearchMedic;
		}

		private void SearchMedic()
		{
			if (_medic != null) return;

			int count = _vision.AlliesInRangeCount;
			if (count == 0) return;

			for(int i = 0; i < count; i++)
			{
				var ally = _vision.AliesInRange[i];

				if (ally == _controller.Actor) continue;

				if (ally.IsVisible && ally is IProvider<IAbility> prov && prov.Value != null && prov.Value.GetType() == typeof(HealOtherAbility))
				{
					_medic = ally;
					if(_medic is IProvider<IHealth> prov2)
					{
						prov2.Value.OnDeath += MedicDead;
					}
					break;
				}
			}
		}

		private void MedicDead(DamageArgs obj)
		{
			(_medic as IProvider<IHealth>).Value.OnDeath -= MedicDead;
			_medic = null;
		}

		public void PreExecute()
		{
			if(_vision.CanSeeTarget(_medic, true))
			{
				_path = null;
			}
			else
			{
				if(ServiceLocator.TryGet<PathFinding>(out var pathFinding) && _path != null)
				{
					_index = 0;
					_path = pathFinding.FindPath(_controller.Position, _medic.Position);
				}
			}
		}

		public void Undo()
		{

		}
	}
}
