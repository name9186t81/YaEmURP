using UnityEngine;

using YaEm.AI;
using YaEm.Health;

namespace YaEm.Ability
{
	public sealed class HealItself : IAIAbilityInstruction
	{
		private HealAbility _heal;
		private AIController _controller;

		public StateType StateType => StateType.Unknown;

		public void Execute()
		{
			_heal.Use();
		}

		public float GetEffectivness()
		{
			if (!_heal.CanUse()) return -100f;
			return 10f;
			return Mathf.Max(0, _heal.HealAmount - _controller.Health.Delta()) / _heal.HealAmount;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
		}

		public void PreExecute()
		{
		}

		public void SetAbility(IAbility ability)
		{
			if(ability is not HealAbility heal)
			{
				Debug.LogError("Expecte heal ability...");
				return;
			}

			_heal = heal;
		}

		public void Undo()
		{
		}
	}
}