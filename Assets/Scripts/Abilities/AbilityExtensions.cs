namespace YaEm.Ability
{
	public static class AbilityExtensions
	{
		public const float AI_MOVEMENT_USE_DELAY = 0.5f;

		public static bool IsMovementAbility(IAbility ability)
		{
			if(ability == null) return false;

			var type = ability.GetType();

			return type == typeof(DashAbility);
		}
	}
}
