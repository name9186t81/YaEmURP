using UnityEngine;

using YaEm.Core;

namespace YaEm.Ability
{
	[CreateAssetMenu(fileName = "HealOther Ability", menuName = "YaEm/HealOther")]
	public sealed class HealOtherBuilder : AbilityBuilder
	{
		[SerializeField] private float _healTick;
		[SerializeField] private int _healAmount;
		[SerializeField] private float _healRadius;
		[SerializeField] private LayerMask _checkMask;
		[SerializeField] private LayerMask _wallsMask;

		public override IAbility Build(IActor owner)
		{
			var heal = new HealOtherAbility(_healTick, _healAmount, _healRadius, _checkMask, _wallsMask);
			heal.Init(owner); 
			return heal;
		}
	}
}