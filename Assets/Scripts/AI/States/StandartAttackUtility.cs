using UnityEngine;

using YaEm.Core;

namespace YaEm.AI.States
{
	public class StandartAttackUtility : IUtility
	{
		private AIController _controller;

		public StateType StateType => StateType.Attacking;
		public void Execute()
		{
			var target = _controller.CurrentTarget;
			if (_controller.IsTargetNull) return;

			if (!_controller.Vision.CanSeeTarget(target, false))
			{
				_controller.ForgotTarget(true);
				return;
			}
			Vector2 pos = _controller.PredictShootPoint(target);

			_controller.LookAtPoint(pos);

			if (_controller.IsEffectiveToFire(pos))
			{
				_controller.InitCommand(ControllerAction.Fire);
				if(_controller.Position.DistanceLess(pos, _controller.Weapon.UseRange / 2))
				{
					_controller.MoveToThePoint(-target.Position);
				}
				_controller.StopMoving();
			}
			else
			{
				_controller.MoveToThePoint(target.Position);
			}
			
		}

		public float GetEffectivness()
		{
			return 0f;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
		}

		public void PreExecute()
		{
			_controller.StopMoving();
		}

		public void Undo()
		{
		}
	}
}
