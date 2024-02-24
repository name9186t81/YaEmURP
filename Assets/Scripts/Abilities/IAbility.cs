using System;

using YaEm.Core;

namespace YaEm.Ability
{
	public interface IAbility
	{
		AbilityType Type { get; }
		bool CanUse();
		void Use();
		void Update(float dt);
		void Init(IActor actor);
		/// <summary>
		/// Instruction for ai of how to use an ability. Can be null.
		/// </summary>
		IAIAbilityInstruction AIAbilityInstruction { get; }

		event Action OnActivate;
		event Action OnDeactivate;
	}

	public enum AbilityType
	{
		Passive,
		Instant,
		Directional
	}
}