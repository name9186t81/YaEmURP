using YaEm.AI;

namespace YaEm.Ability
{
	public interface IAIAbilityInstruction : IUtility
	{
		void SetAbility(IAbility ability);
	}
}