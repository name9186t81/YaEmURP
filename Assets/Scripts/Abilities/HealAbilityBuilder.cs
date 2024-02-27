using UnityEngine;

using YaEm.Core;

namespace YaEm.Ability
{
	[CreateAssetMenu(fileName = "Heal ability", menuName = "YaEm/Heal Ability")]
	public sealed class HealAbilityBuilder : AbilityBuilder
	{
		[SerializeField] private float _duration;
		[SerializeField] private float _cooldown;
		[SerializeField] private float _weaponReloadIncrease;
		[SerializeField] private float _speedIncrease;
		[SerializeField] private float _healAmount;

		public override IAbility Build(IActor owner)
		{
			var instruction = new HealItself();
			var ability = new HealAbility(_healAmount, _speedIncrease, _weaponReloadIncrease, _cooldown, _duration, instruction); ;
			ability.Init(owner);
			return ability;
		}
	}
}