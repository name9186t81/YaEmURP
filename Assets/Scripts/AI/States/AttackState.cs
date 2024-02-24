using System;

using UnityEngine;

namespace YaEm.AI.States
{
	public class AttackState : IUtility
	{
		private UtilityAI _subStates;
		private AIController _controller;

		public StateType StateType => StateType.Attacking;
		public void Execute()
		{
			_subStates.Update();
		}

		public float GetEffectivness()
		{
			return ((_controller.CurrentTarget != null) || (_controller.Memory.TryGetValue(AIMemoryKey.LastTarget, out var target) && target != null)) ? 1f : -100f;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
			_subStates = new UtilityAI(controller, new IUtility[]
			{
				new StandartAttackUtility(),
				new SupressFire(),
				new SeekTarget(),
				new	ChangePosition(),
				new RetreatToSavePosition()
			});
		}

		public void PreExecute()
		{

		}

		public void Undo()
		{
			_subStates.Stop();
		}
	}
}
