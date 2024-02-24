using YaEm.Ability;

namespace YaEm.AI
{
	public sealed class DashSubState : IAIAbilityInstruction
	{
		public StateType StateType => StateType.Ability;
		private readonly IAIAbilityInstruction[] _instructions;
		private IAIAbilityInstruction _selected;
		private DashAbility _dash;

		public DashSubState(IAIAbilityInstruction[] instructions)
		{
			_instructions = instructions;
		}

		public void Execute()
		{
			_selected.Execute();
		}

		public float GetEffectivness()
		{
			if (!_dash.CanUse()) return -100f;

			_selected = _instructions[0];
			float max = _selected.GetEffectivness();
			for (int i = 1, length = _instructions.Length; i < length; ++i)
			{
				var current = _instructions[i];
				float effect = current.GetEffectivness();
				if (effect > max)
				{
					max = effect;
					_selected = current;
				}
			}

			return max;
		}

		public void Init(AIController controller)
		{
			for(int i = 0; i < _instructions.Length; i++)
			{
				_instructions[i].Init(controller);
			}
		}

		public void PreExecute()
		{
			_selected.PreExecute();
		}

		public void SetAbility(IAbility ability)
		{
			_dash = ability as DashAbility;
			for (int i = 0; i < _instructions.Length; i++)
			{
				_instructions[i].SetAbility(_dash);
			}
		}

		public void Undo()
		{
			_selected.Undo();
		}
	}
}