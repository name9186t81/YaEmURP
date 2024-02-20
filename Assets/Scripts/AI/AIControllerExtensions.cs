using UnityEngine;

using YaEm.Core;

namespace YaEm.AI
{
	public static class AIControllerExtensions
	{
		public static void Attack(this AIController controller, in Vector2 target)
		{
			controller.LookAtPoint(target);
			if (controller.IsEffectiveToFire(target)) controller.InitCommand(ControllerAction.Fire);
		}
	}
}
