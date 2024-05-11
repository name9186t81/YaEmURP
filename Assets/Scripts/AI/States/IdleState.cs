namespace YaEm.AI.States
{
	public class IdleState : IUtility
	{
		private UtilityAI _subStates;
		private AIController _controller;

		public StateType StateType => StateType.Idling;

		public void Execute()
		{
			_subStates.Update();
		}

		public float GetEffectivness()
		{
			return (_controller.CurrentTarget != null || _controller.Memory.TryGetValue(AIMemoryKey.LastTarget, out _)) ? -100f : 0f ;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
			_subStates = new UtilityAI(controller, new IUtility[] 
			{
				new IdleWalkUtility(2f, 6f, 1f),
				new LookAroundUtility(),
				new WanderInMap(),
				new ReportTargetsInSight(),
				new PursueTargetFromMap(),
				new MimicExperienced(),
				new StayNearMedic()
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
