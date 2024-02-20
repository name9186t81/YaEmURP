namespace YaEm.Health
{
	public static class HealthExtensions
	{
		public static float Delta(this IHealth health) => (float)health.CurrentHealth / health.MaxHealth;
	}
}
