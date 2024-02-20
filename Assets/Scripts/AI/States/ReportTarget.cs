using System.Collections.Generic;

using YaEm.AI;
using YaEm.Core;

namespace YaEm.AI.States
{
	public class ReportTargetsInSight : IUtility
	{
		private AIVision _vision;
		private AIController _controller;
		private TargetMapProvider _targetMap;

		public StateType StateType => StateType.Unknown;

		//auto-state: should never be executed

		public void Execute()
		{
		}

		public void Init(AIController controller)
		{
			_vision = controller.Vision;
			_vision.OnScan += Report;
			_controller = controller;
			ServiceLocator.TryGet(out _targetMap);
		}

		private void Report()
		{
			if (_targetMap == null)
			{
				return;
			}

			IReadOnlyList<IActor> targets = _vision.EnemiesInRange;

			foreach (var r in targets)
			{
				_targetMap.AddTarget((_controller.Actor is ITeamProvider prov) ? prov.TeamNumber : 0, r.Position);
			}
		}

		public void PreExecute()
		{
		}

		public void Undo()
		{
		}

		public float GetEffectivness()
		{
			return -1000f;
		}
	}
}